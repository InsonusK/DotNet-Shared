using FluentValidation;
using InsonusK.Shared.Command.Validation.Pipeline;
using InsonusK.Shared.Command.Validation.Validators;
using Microsoft.Extensions.Configuration;
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
    /// <param name="config">The configuration (optional).</param>
    public static void AddCommandValidation(this IServiceCollection services, IConfiguration? config = null)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(ValidationBehavior<,>).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>), ServiceLifetime.Scoped);
        });
        services.AddTransient(typeof(IValidator<>), typeof(CommandWithEntityKeysValidator<>));
        services.AddTransient(typeof(IValidator<>), typeof(CommandWithBodyValidator<>));
    }
}