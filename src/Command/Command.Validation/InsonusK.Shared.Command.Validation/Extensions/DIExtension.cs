using InsonusK.Shared.Command.Validation.Pipeline;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace InsonusK.Shared.Command.Validation.Extensions;
public static class DIExtension
{
    public static void AddCommandValidation(this IServiceCollection services)
    {
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    }
}