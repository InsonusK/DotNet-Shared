using Microsoft.Extensions.Logging;
using Divergic.Logging.Xunit;
using Xunit.Abstractions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using InsonusK.Shared.Mediator.CommandContext;
using InsonusK.Shared.Mediator.CommandContext.Handler;
using InsonusK.Shared.Mediator.CommandContext.Service;
using InsonusK.Shared.Mediator.CommandContext.Interfaces;

namespace InsonusK.Shared.Mediator.CommandContext.Test;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class CommandContextModuleRegister_Test : LoggingTestsBase<CommandContextModuleRegister_Test>
{
    public CommandContextModuleRegister_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
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

        Assert.Contains(services, s => s.ServiceType == typeof(IPipelineBehavior<,>) && s.ImplementationType == typeof(CommandWithStringIdHandler<,>));
        Assert.Contains(services, s => s.ServiceType == typeof(IPipelineBehavior<,>) && s.ImplementationType == typeof(CommandWithKeysHandler<,>));
        Assert.Contains(services, s => s.ServiceType == typeof(CommandContextContainer) && s.ImplementationType == typeof(CommandContextContainer));
        
        // Ensure ICommandContext is registered
        Assert.Contains(services, s => s.ServiceType == typeof(ICommandContext));

        #endregion
    }
}
