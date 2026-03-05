using InsonusK.Shared.Command.EntityLoading.Interfaces;
using InsonusK.Shared.Command.EntityLoading.Pipeline;
using InsonusK.Shared.Command.EntityLoading.Services;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InsonusK.Shared.Command.EntityLoading.Helper;
public static class ServiceProviderExtensions
{
    /// <summary>
    /// Регистрирует зависимости для работы с загрузкой контекста сущностей: PipelineBehavior, CommandContextManager и EntityProvider.
    /// </summary>
    /// <param name="sc">Коллекция сервисов.</param>
    /// <param name="config">Конфигурация (опционально).</param>
    /// <returns>Коллекция сервисов.</returns>
    public static IServiceCollection AddCommandEntityLoading(this IServiceCollection sc,  IConfiguration? config = null)
    {
        sc.AddScoped(typeof(IPipelineBehavior<,>), typeof(EntityLoadingBehavior<,>));
        sc.AddScoped<CommandContextManager>();
        sc.AddScoped<ICommandContextSource>(sp => sp.GetRequiredService<CommandContextManager>());
        sc.AddScoped<EntityProvider>();
        return sc;
    }
}