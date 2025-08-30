using FFMpegCore;
using Legal.Api.WebApi.DataSeeding;
using Legal.Api.WebApi.DelegateHandler;
using Legal.Application.Admin;
using Legal.Service.Infrastructure.Interface;
using Legal.Service.Infrastructure.Model;
using Legal.Service.Infrastructure.Services;
using Legal.Service.Repository;
using System.Reflection;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();

// Add services to the container.
builder.Services.AddAdminApplicationLayer(builder.Configuration);
builder.Services
    .AddTransient(typeof(IRepository<,>), typeof(PostgresRepository<,>))
    .AddTransient(typeof(IDomainRepository<,>), typeof(DomainPostgresRepository<,>))
    .AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

object value = builder.Services
    .AddControllers()
    .AddNewtonsoftJson();

// await builder.Services.RegisterSignalR(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Legal API",
        Version = "v1",
        Description = "Dynamic CQRS endpoints. Command/Query handlers are executed via /api/Command/Execute/{module} and /api/Query/Execute/{module}. Use ListAll & Detail endpoints to discover handlers."
    });

    // Add a custom document filter to append handlers list into description
    c.DocumentFilter<HandlersDocumentFilter>();
});

builder.Services
    .AddHttpContextAccessor()
    .AddSingleton(TimeProvider.System)
    .AddSingleton<RequestHandler>()
    .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
    .AddScoped<IAccessToken, AccessToken>()
    .AddScoped(b => b.GetRequiredService<IHttpContextAccessor>().HttpContext.DecodeJwt())
    .AddHostedService<DataSeederService>();

builder.Services.AddTransient<TokenDelegateHandler>();

// Register swagger helper filter dependency
builder.Services.AddSingleton<HandlersDocumentFilter>();

// CORS configuration fix
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        if (allowedOrigins.Length > 0 && !allowedOrigins.Contains("*"))
        {
            // Explicit allow-list with credentials
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        }
        else
        {
            policy.SetIsOriginAllowed(_ => true)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        }
    });
});


builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = Convert.ToInt64(builder.Configuration["MaxRequestBodySize"]);
});

string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();

// Swagger document filter to list handlers grouped by module
public class HandlersDocumentFilter : Swashbuckle.AspNetCore.SwaggerGen.IDocumentFilter
{
    public void Apply(Microsoft.OpenApi.Models.OpenApiDocument swaggerDoc, Swashbuckle.AspNetCore.SwaggerGen.DocumentFilterContext context)
    {
        // Build a markdown table of handlers
        var registered = RequestHandler.GetRegisteredHandlers();
        if (registered.Count == 0) return;

        var lines = new List<string>();
        lines.Add("Handlers registered by module (Commands & Queries):\n");
        foreach (var module in registered.OrderBy(m => m.Key))
        {
            lines.Add($"### {module.Key}");
            foreach (var handler in module.Value.OrderBy(t => t.Name))
            {
                // Identify type
                var isCommand = IsSubclassOfRawGeneric(typeof(ACommandHandler<,>), handler);
                var isQuery = IsSubclassOfRawGeneric(typeof(AQueryHandler<,>), handler);
                var type = isCommand ? "Command" : isQuery ? "Query" : string.Empty;
                lines.Add($"- {type}: {handler.Name.Replace("Handler", string.Empty)}");
            }
            lines.Add(string.Empty);
        }

        var appendix = string.Join("\n", lines);
        if (swaggerDoc.Info.Description is null)
            swaggerDoc.Info.Description = appendix;
        else
            swaggerDoc.Info.Description += "\n\n" + appendix;
    }

    private static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
    {
        while (toCheck != null && toCheck != typeof(object))
        {
            var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
            if (cur == generic)
            {
                return true;
            }

            toCheck = toCheck.BaseType!;
        }
        return false;
    }
}