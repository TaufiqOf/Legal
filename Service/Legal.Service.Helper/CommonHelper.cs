using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Legal.Service.Helper;

public static class CommonHelper
{
    public static IEnumerable<Type> RegisterHandlers(
        this IServiceCollection services,
        Assembly assembly,
        params Type[] types)
    {
        List<Type> handlerTypes = new();

        services
            .Scan(scan => scan
            .FromAssemblies(assembly)
            .AddClasses(classes => classes.Where(c =>
            {
                if (c.IsClass
                    && !c.IsAbstract
                    && c.BaseType?.IsGenericType is true
                    && types.Any(t => c.BaseType.GetGenericTypeDefinition() == t))
                {
                    handlerTypes.Add(c);
                    return true;
                }

                return false;
            }))
            .AsSelf()
            .WithScopedLifetime());

        return handlerTypes;
    }

    public static IServiceCollection RegisterTypes(
        this IServiceCollection services,
        Assembly assembly,
        params Type[] typesToIgnore)
    {
        services
            .Scan(scan => scan
            .FromAssemblies(assembly)
            .AddClasses(classes => classes.Where(c =>
                c.IsClass
                && !c.IsAbstract
                && !typesToIgnore.Any(type => c.IsAssignableTo(type))))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }
}