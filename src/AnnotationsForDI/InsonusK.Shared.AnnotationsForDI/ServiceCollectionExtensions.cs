using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace InsonusK.Shared.AnnotationsForDI;


public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Автоматическая регистрация сервисов по атрибуту ServiceAttribute
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="assembly">Сборка для сканирования</param>
    /// <param name="namespaceFilter">Опциональный фильтр по namespace</param>
    public static void AddAnnotatedServices(
        this IServiceCollection services,
        Assembly assembly,
        string? namespaceFilter = null)
    {
        var types = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.GetCustomAttribute<ServiceAttribute>() != null);

        if (!string.IsNullOrEmpty(namespaceFilter))
        {
            types = types.Where(t => t.Namespace != null && t.Namespace.StartsWith(namespaceFilter));
        }

        foreach (var type in types)
        {
            var attr = type.GetCustomAttribute<ServiceAttribute>()!;

            // Если указаны интерфейсы, регистрируем под ними
            if (attr.Interfaces.Length > 0)
            {
                foreach (var iface in attr.Interfaces)
                {
                    if (!iface.IsAssignableFrom(type))
                        throw new InvalidOperationException(
                            $"{type.FullName} не реализует интерфейс {iface.FullName}");

                    // Проверка дубликатов
                    if (services.Any(sd => sd.ServiceType == iface))
                        throw new InvalidOperationException($"Тип {iface.FullName} уже зарегистрирован в коллекции сервисов");

                    services.Add(new ServiceDescriptor(iface, type, attr.Lifetime));
                }
            }

            // Регистрируем как самостоятельный класс
            if (services.Any(sd => sd.ServiceType == type))
                throw new InvalidOperationException($"Тип {type.FullName} уже зарегистрирован в коллекции сервисов");
            services.Add(new ServiceDescriptor(type, type, attr.Lifetime));

        }
    }
}
