using System.Reflection;
using InsonusK.Shared.Command.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InsonusK.Shared.Command.Service.Interface.Helpers;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEntityCommandExtractor(this IServiceCollection sc, IConfiguration config = null, params Assembly[] assembliesToScan)
    {
        // 1. Определяем тип интерфейса
        var interfaceType = typeof(IEntityCommandExtractor<>);

        // 2. Определяем список проектов
        var assemblies = assembliesToScan.Any() ? assembliesToScan : new[] { Assembly.GetCallingAssembly() };

        // 3. Находим все неабстрактные классы, которые реализуют этот интерфейс
        var implementations = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract)
            .SelectMany(t => t.GetInterfaces(), (t, i) => new { Implementation = t, Interface = i })
            .Where(pair => pair.Interface.IsGenericType &&
                        pair.Interface.GetGenericTypeDefinition() == interfaceType);

        foreach (var pair in implementations)
        {
            // 4. Регистрируем каждую найденную пару (Интерфейс<T>, Реализация)
            sc.AddScoped(pair.Interface, pair.Implementation);
        }

        return sc;
    }
}