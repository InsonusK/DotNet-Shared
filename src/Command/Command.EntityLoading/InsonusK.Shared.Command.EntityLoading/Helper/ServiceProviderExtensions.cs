using InsonusK.Shared.Command.EntityLoading.Interfaces;
using InsonusK.Shared.Command.EntityLoading.Pipeline;
using InsonusK.Shared.Command.EntityLoading.Services;
using InsonusK.Shared.Command.EntityLoading.Tools;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InsonusK.Shared.Command.EntityLoading.Helper;
public static class ServiceProviderExtensions
{
    public static IServiceCollection AddCommandEntityLoading(this IServiceCollection sc,  IConfiguration? config = null)
    {
        sc.AddScoped(typeof(IPipelineBehavior<,>), typeof(EntityLoadingBehavior<,>));
        sc.AddScoped<CommandContextManager>();
        sc.AddScoped<ICommandContextSource>(sp => sp.GetRequiredService<CommandContextManager>());
        sc.AddScoped<EntityProvider>();
        return sc;
    }
}