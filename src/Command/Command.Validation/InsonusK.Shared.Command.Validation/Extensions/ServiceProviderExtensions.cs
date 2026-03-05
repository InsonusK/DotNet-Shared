using InsonusK.Shared.Command.Validation.Pipeline;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace InsonusK.Shared.Command.Validation.Extensions;
/// <summary>
/// Dependency injection extensions for adding command validation.
/// </summary>
public static class ServiceProviderExtensions
{
    /// <summary>
    /// Adds validation behavior to MediatR pipeline. 
    /// This enables automatic validation of commands before their execution.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    public static void AddCommandValidation(this IServiceCollection services)
    {
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    }
}