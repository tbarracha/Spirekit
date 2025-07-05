using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace SpireCore.Services;

public static partial class GenericServiceRegistrationExtensions
{
    /// <summary>
    /// Registers all subclasses of the specified open generic base type across all loaded assemblies.
    /// </summary>
    public static IServiceCollection AddScopedImplementationsOfGenericType(
        this IServiceCollection services,
        Type baseGenericType)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var assembly in assemblies)
        {
            RegisterFromAssembly(services, baseGenericType, assembly);
        }

        return services;
    }

    /// <summary>
    /// Registers all subclasses of the specified open generic base type from a specific assembly.
    /// </summary>
    public static IServiceCollection AddRepositorySubclassesOf<TBase>(
        this IServiceCollection services,
        Assembly assembly)
    {
        var baseType = typeof(TBase);
        RegisterFromAssembly(services, baseType, assembly);
        return services;
    }

    private static void RegisterFromAssembly(
    IServiceCollection services,
    Type baseGenericType,
    Assembly assembly)
    {
        var concreteTypes = SafeGetTypes(assembly)
            .Where(t =>
                !t.IsAbstract &&
                !t.IsInterface &&
                t.IsClass &&
                InheritsFromGenericType(t, baseGenericType))
            .ToList();

        foreach (var impl in concreteTypes)
        {
            services.AddScoped(impl);

            var baseType = GetClosedGenericBase(impl, baseGenericType);
            if (baseType != null)
                services.AddScoped(baseType, impl);
        }
    }

    private static IEnumerable<Type> SafeGetTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.Where(t => t != null)!;
        }
    }

    private static bool InheritsFromGenericType(Type type, Type generic)
    {
        return GetClosedGenericBase(type, generic) != null;
    }

    private static Type? GetClosedGenericBase(Type type, Type generic)
    {
        // Check interfaces
        foreach (var i in type.GetInterfaces())
        {
            if (i.IsGenericType && i.GetGenericTypeDefinition() == generic)
                return i;
        }

        // Check base class chain
        while (type != null && type != typeof(object))
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == generic)
                return type;

            type = type.BaseType;
        }

        return null;
    }
}
