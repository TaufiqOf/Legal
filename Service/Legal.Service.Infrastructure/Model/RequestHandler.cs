using FluentValidation;
using Legal.Service.Infrastructure.Interface;
using Legal.Service.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using static Legal.Service.Helper.ApplicationHelper;

namespace Legal.Service.Infrastructure.Model;

public class RequestHandler(IServiceProvider serviceProvider)
{
    private static Dictionary<ModuleName, List<Type>> _requestHandlers = new Dictionary<ModuleName, List<Type>>();

    public static void SetRequestHandlers(ModuleName moduleName, IEnumerable<Type> requestHandlers)
    {
        if (_requestHandlers.TryGetValue(moduleName, out var list))
        {
            list.AddRange(requestHandlers);
        }
        else
        {
            _requestHandlers.Add(moduleName, requestHandlers.ToList());
        }
    }

    // Added: expose registered handlers for swagger/documentation generation
    public static IReadOnlyDictionary<ModuleName, IReadOnlyList<Type>> GetRegisteredHandlers()
        => _requestHandlers.ToDictionary(k => k.Key, v => (IReadOnlyList<Type>)v.Value.AsReadOnly());

    public static async Task<List<string>> ListCommands(string moduleName)
    {
        var type = typeof(ACommandHandler<,>);
        return await ListRequestHandlers(moduleName, type);
    }

    public static async Task<List<string>> ListQuery(string moduleName)
    {
        var type = typeof(AQueryHandler<,>);
        return await ListRequestHandlers(moduleName, type);
    }

    public static async Task<string> CommandDetails(string moduleName, string name)
    {
        var handlerType = GetHandlersByModule(moduleName)
            .FirstOrDefault(type => IsSubclassOfRawGeneric(typeof(ACommandHandler<,>), type) && type.Name == $"{name}Handler");

        return await Task.FromResult(Details(name, handlerType));
    }

    public static async Task<string> QueryDetails(string moduleName, string name)
    {
        var handlerType = GetHandlersByModule(moduleName)
            .FirstOrDefault(type => IsSubclassOfRawGeneric(typeof(AQueryHandler<,>), type) && type.Name == $"{name}Handler");

        return await Task.FromResult(Details(name, handlerType));
    }

    public async Task<ResultModel<IResponseModel>> Execute(string moduleName, JObject request, CancellationToken cancellationToken, IFormCollection? forms = null)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var baseCommand = request.ToObject<RequestModel>();
            baseCommand.ReceivedDateTime = DateTimeOffset.UtcNow;
            IRequestHandler handler = GetHandler(moduleName, baseCommand, scope);
            var handlerType = handler.GetType();
            var accessToken = scope.ServiceProvider.GetService<IAccessToken>();
            IHttpContextAccessor httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();

            if (httpContextAccessor.HttpContext?.Request?.Path.StartsWithSegments("/api/public", StringComparison.OrdinalIgnoreCase) is true)
            {
                AllowAnonymousAttribute? allowAnonymousUserAttribute = handlerType.GetCustomAttribute<AllowAnonymousAttribute>()
                                                                    ?? throw new UnauthorizedAccessException();
            }
            if (Attribute.IsDefined(handlerType, typeof(TokenAuthorizeAttribute)))
            {
                if (accessToken is null)
                {
                    throw new UnauthorizedAccessException("Access token is not provided.");
                }
            }

            IParameterModel<IValidator>? parameter = GetParameter(request, handlerType);
            parameter.ModuleName = GetModuleType(moduleName);
            if (parameter != null)
            {
                var validationContext = new ValidationContext<IParameterModel<IValidator>>(parameter);
                var validationResult = await parameter.Validator.ValidateAsync(validationContext, cancellationToken);
                if (!validationResult.IsValid)
                {
                    throw new ValidationException(validationResult.Errors);
                }

                if (parameter is UploadFileParameterModel uploadFileParameterModel)
                {
                    if (forms is not null)
                    {
                        uploadFileParameterModel.FormCollection = forms;
                    }
                }

                var executeMethod = handlerType.GetMethod("ExecuteHandler");

                if (executeMethod == null)
                {
                    throw new InvalidOperationException("ExecuteHandler method not found in handler.");
                }

                var returnType = executeMethod.ReturnType;
                var result = executeMethod.Invoke(handler, [baseCommand, parameter, cancellationToken]);

                if (returnType == typeof(Task) || returnType.IsSubclassOf(typeof(Task)))
                {
                    if (result is Task taskResult)
                    {
                        await taskResult;

                        if (returnType.IsGenericType)
                        {
                            var resultProperty = returnType.GetProperty("Result");
                            return resultProperty?.GetValue(taskResult) as ResultModel<IResponseModel>;
                        }

                        return null; // For Task with no result
                    }
                }
                else
                {
                    return result as ResultModel<IResponseModel>;
                }
            }

            throw new InvalidOperationException("Request model does not contain a Parameter property.");
        }
    }

    public async Task<dynamic> ExecuteString(string moduleName, string request, CancellationToken cancellationToken)
    {
        return await Execute(moduleName, JsonConvert.DeserializeObject<JObject>(request), cancellationToken);
    }

    private IParameterModel<IValidator>? GetParameter(JObject request, Type handlerType)
    {
        var parameterType = handlerType.GetInterfaces()
                           .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IHandler<,>))
                           ?.GetGenericArguments()[0];
        return (IParameterModel<IValidator>)request["Parameter"]?.ToObject(parameterType);
    }

    private IRequestHandler GetHandler(string moduleName, RequestModel request, IServiceScope scope)
    {
        var commandName = GetProperty(request, "RequestName");

        if (commandName != null)
        {
            var handlerType = GetHandlersByModule(moduleName)
                .Where(q => q.Name.Contains("QueryHandler") && IsSubclassOfRawGeneric(typeof(AQueryHandler<,>), q) ||
                            q.Name.Contains("CommandHandler") && IsSubclassOfRawGeneric(typeof(ACommandHandler<,>), q))
                .FirstOrDefault(type => type.Name == $"{commandName}Handler");

            if (handlerType != null)
            {
                // Create a scope to resolve the service

                var instance = scope.ServiceProvider.GetService(handlerType) as IRequestHandler;
                if (instance != null)
                {
                    return instance;
                }

                throw new InvalidOperationException($"Handler not registered for command {commandName}.");
            }

            throw new NotImplementedException($"Handler not found for command {commandName}.");
        }

        throw new InvalidOperationException("CommandName is missing in the request.");
    }

    private static string? GetProperty(dynamic obj, string propertyName)
    {
        var propertyInfo = obj.GetType().GetProperty(propertyName);
        return propertyInfo?.GetValue(obj);
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

            toCheck = toCheck.BaseType;
        }

        return false;
    }

    private static JObject GetPropertiesJson(Type type)
    {
        var propertiesJson = new JObject();

        // Use reflection to get all public properties of the type
        foreach (var property in type.GetProperties())
        {
            propertiesJson[property.Name] = property.PropertyType.Name;
        }

        return propertiesJson;
    }

    private static string Details(string name, Type? handlerType)
    {
        if (handlerType == null)
        {
            throw new InvalidOperationException($"Handler for {name} not found.");
        }

        // Find the interface that implements IHandler<T, R>
        var commandHandlerInterface = handlerType.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IHandler<,>));

        if (commandHandlerInterface == null)
        {
            throw new InvalidOperationException($"Handler for {name} does not implement IHandler<,>.");
        }

        // Extract the generic arguments (parameter type and response type)
        var genericArguments = commandHandlerInterface.GetGenericArguments();

        if (genericArguments.Length < 2)
        {
            throw new InvalidOperationException($"Handler for {name} does not have the correct generic arguments.");
        }

        var parameterType = genericArguments[0];
        var responseType = genericArguments[1];

        // Create JSON representation of the parameter model and response model
        var parameterProperties = GetPropertiesJson(parameterType);
        var responseProperties = GetPropertiesJson(responseType);

        // Create a combined JSON response
        var commandDetailsJson = new JObject
        {
            ["CommandName"] = name,
            ["ParameterModel"] = parameterProperties,
            ["ResponseModel"] = responseProperties
        };

        return commandDetailsJson.ToString();
    }

    private static List<Type> GetHandlersByModule(string moduleName)
    {
        var applicationType = GetModuleType(moduleName);

        if (_requestHandlers.ContainsKey(applicationType))
        {
            return _requestHandlers[applicationType];
        }
        throw new InvalidOperationException($"Invalid module {moduleName}");
    }

    private static ModuleName GetModuleType(string moduleName)
    {
        var upperModuleName = moduleName.ToUpper();
        var applicationType = (ModuleName)Enum.Parse(typeof(ModuleName), upperModuleName);
        return applicationType;
    }

    private static async Task<List<string>> ListRequestHandlers(string moduleName, Type type)
    {
        List<string> handlers = GetHandlersByModule(moduleName)
            .Where(q => IsSubclassOfRawGeneric(type, q))
            .Select(q => q.Name.Replace("Handler", ""))
            .ToList();

        return await Task.FromResult(handlers);
    }
}