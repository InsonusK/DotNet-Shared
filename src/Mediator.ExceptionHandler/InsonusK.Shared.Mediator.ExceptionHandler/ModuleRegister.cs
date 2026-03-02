using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using InsonusK.Shared.Mediator.ExceptionHandler.Handler;
using InsonusK.Shared.Mediator.ExceptionHandler.Service;

namespace InsonusK.Shared.Mediator.ExceptionHandler;

public static class ExceptionHandlerModuleRegister
{
    public static IServiceCollection Register(this IServiceCollection sc, IConfiguration config = null)
    {
        var assembly = typeof(ExceptionHandlerModuleRegister).Assembly;
        sc.AddScoped(typeof(IPipelineBehavior<,>), typeof(CommandValidationHandler<,>));
        sc.AddScoped(typeof(IPipelineBehavior<,>), typeof(ExceptionHandler<,>));
        sc.AddScoped(typeof(ArdalisResultReflectionFactory<>));
        sc.AddValidatorsFromAssembly(assembly);
        return sc;
    }
}
