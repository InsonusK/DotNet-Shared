using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using InsonusK.Shared.Mediator.CommandContext.Handler;
using InsonusK.Shared.Mediator.CommandContext.Interfaces;
using InsonusK.Shared.Mediator.CommandContext.Service;

namespace InsonusK.Shared.Mediator.CommandContext;

public static class CommandContextModuleRegister
{
    public static IServiceCollection Register(this IServiceCollection sc,  IConfiguration config = null)
    {
        sc.AddScoped(typeof(IPipelineBehavior<,>), typeof(CommandWithStringIdHandler<,>));
        sc.AddScoped(typeof(IPipelineBehavior<,>), typeof(CommandWithKeysHandler<,>));
        sc.AddScoped<CommandContextContainer>();
        sc.AddScoped<ICommandContext>(sp => sp.GetRequiredService<CommandContextContainer>());
        return sc;
    }
}
