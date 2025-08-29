using FFMpegCore;
using Legal.Api.WebApi.DataSeeding;
using Legal.Api.WebApi.DelegateHandler;
using Legal.Application.Admin;
using Legal.Service.Infrastructure.Interface;
using Legal.Service.Infrastructure.Model;
using Legal.Service.Infrastructure.Services;
using Legal.Service.Repository;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();

// Add services to the container.
builder.Services.AddExpenseApplicationLayer(builder.Configuration);
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
builder.Services.AddSwaggerGen();
builder.Services
    .AddHttpContextAccessor()
    .AddSingleton(TimeProvider.System)
    .AddSingleton<RequestHandler>()
    .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
    .AddScoped<IAccessToken, AccessToken>()
    .AddScoped(b => b.GetRequiredService<IHttpContextAccessor>().HttpContext.DecodeJwt())
    .AddHostedService<DataSeederService>();

builder.Services.AddTransient<TokenDelegateHandler>();

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
            // Wildcard intent: either allow all WITHOUT credentials OR echo any origin WITH credentials
            // Option 1 (allow all, no credentials):
            // policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();

            // Option 2 (allow all, keep credentials): echo back request origin
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