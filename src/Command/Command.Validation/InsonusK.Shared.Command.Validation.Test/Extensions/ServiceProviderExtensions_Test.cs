using Divergic.Logging.Xunit;
using InsonusK.Shared.Command.Validation.Extensions;
using InsonusK.Shared.Command.Validation.Pipeline;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace InsonusK.Shared.Command.Validation.Test.Extensions;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class ServiceProviderExtensions_Test : LoggingTestsBase<ServiceProviderExtensions_Test>
{
    public ServiceProviderExtensions_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
    {
    }

    #region AddCommandValidation

    [Fact]
    public void AddCommandValidation_WHEN_called_THEN_registers_ValidationBehavior_as_pipeline_behavior()
    {
        Logger.LogDebug("Test ARRAY");
        var services = new ServiceCollection();

        Logger.LogDebug("Test ACT");
        services.AddCommandValidation();

        Logger.LogDebug("Test ASSERT");
        var registeredBehavior = services.FirstOrDefault(sd => sd.ServiceType == typeof(IPipelineBehavior<,>));
        Assert.NotNull(registeredBehavior);
        Assert.Equal(typeof(ValidationBehavior<,>), registeredBehavior.ImplementationType);
        Assert.Equal(ServiceLifetime.Scoped, registeredBehavior.Lifetime);
    }

    #endregion
}
