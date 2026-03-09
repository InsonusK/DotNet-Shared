using Microsoft.Extensions.Logging;
using Divergic.Logging.Xunit;
using Xunit.Abstractions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using InsonusK.Shared.Mediator.ExceptionHandler.Handler;
using InsonusK.Shared.Mediator.ExceptionHandler.Service;
using InsonusK.Shared.Mediator.ExceptionHandler.Validators;

namespace InsonusK.Shared.Mediator.ExceptionHandler.Test;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class ExceptionHandlerModuleRegister_Test : LoggingTestsBase<ExceptionHandlerModuleRegister_Test>
{
    public ExceptionHandlerModuleRegister_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
    {
    }

    [Fact]
    public void Register_WHEN_Called_THEN_RegistersExpectedServices()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var services = new ServiceCollection();

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        services.Register();

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.Contains(services, s => s.ServiceType == typeof(IPipelineBehavior<,>) && s.ImplementationType == typeof(ExceptionHandler<,>));
        Assert.Contains(services, s => s.ServiceType == typeof(ArdalisResultReflectionFactory<>) && s.ImplementationType == typeof(ArdalisResultReflectionFactory<>));
        
        // Check for validator
        Assert.Contains(services, s => s.ImplementationType == typeof(CommandValidator));

        #endregion
    }
}
