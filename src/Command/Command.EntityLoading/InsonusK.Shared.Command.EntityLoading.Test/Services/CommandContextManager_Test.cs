using Divergic.Logging.Xunit;
using InsonusK.Shared.Command.EntityLoading.Services;
using InsonusK.Shared.Command.EntityLoading.Tools;
using InsonusK.Shared.Command.Interfaces;
using InsonusK.Shared.Command.Interfaces.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace InsonusK.Shared.Command.EntityLoading.Test.Services;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class CommandContextManager_Test : LoggingTestsBase<CommandContextManager_Test>
{
    public CommandContextManager_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
    {
    }

    private class TestCommand : ICommandWithEntityKeys, IRequest
    {
        public IEnumerable<IEntityKey> EntityKeys { get; set; } = Array.Empty<IEntityKey>();
    }

    /// <summary>
    /// description: Start explicit context for command
    /// input: IRequest, CommandContext
    /// output: none
    /// expected_result: Context is tracked
    /// </summary>
    [Fact]
    public void test_StartFor_WithContext_WHEN_command_not_started_THEN_add_success()
    {
        #region Arrange
        Logger.LogDebug("Test ARRANGE");
        var manager = new CommandContextManager();
        var command = new TestCommand();
        var context = new CommandContext();
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        var exception = Record.Exception(() => manager.StartFor(command, context));
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        Assert.Null(exception);
        #endregion
    }

    /// <summary>
    /// description: Start explicit context twice for same command
    /// input: IRequest, CommandContext
    /// output: exception
    /// expected_result: InvalidOperationException is thrown
    /// </summary>
    [Fact]
    public void test_StartFor_WithContext_WHEN_command_already_started_THEN_throw_InvalidOperationException()
    {
        #region Arrange
        Logger.LogDebug("Test ARRANGE");
        var manager = new CommandContextManager();
        var command = new TestCommand();
        var context1 = new CommandContext();
        var context2 = new CommandContext();
        manager.StartFor(command, context1);
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        var exception = Record.Exception(() => manager.StartFor(command, context2));
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
        #endregion
    }

    /// <summary>
    /// description: Start new context for command
    /// input: IRequest
    /// output: CommandContext
    /// expected_result: New context is tracked and returned
    /// </summary>
    [Fact]
    public void test_StartFor_WithoutContext_WHEN_command_not_started_THEN_create_new_and_add_success()
    {
        #region Arrange
        Logger.LogDebug("Test ARRANGE");
        var manager = new CommandContextManager();
        var command = new TestCommand();
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        var context = manager.StartFor(command);
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        Assert.NotNull(context);
        #endregion
    }

    /// <summary>
    /// description: End existing tracked command
    /// input: IRequest
    /// output: none
    /// expected_result: Context tracking is removed
    /// </summary>
    [Fact]
    public void test_EndFor_Generic_WHEN_command_started_THEN_remove_success()
    {
        #region Arrange
        Logger.LogDebug("Test ARRANGE");
        var manager = new CommandContextManager();
        var command = new TestCommand();
        manager.StartFor(command);
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        var exception = Record.Exception(() => manager.EndFor(command));
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        Assert.Null(exception);
        #endregion
    }

    /// <summary>
    /// description: End non-tracked command
    /// input: IRequest
    /// output: exception
    /// expected_result: InvalidOperationException is thrown
    /// </summary>
    [Fact]
    public void test_EndFor_Generic_WHEN_command_not_started_THEN_throw_InvalidOperationException()
    {
        #region Arrange
        Logger.LogDebug("Test ARRANGE");
        var manager = new CommandContextManager();
        var command = new TestCommand();
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        var exception = Record.Exception(() => manager.EndFor(command));
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
        #endregion
    }

    /// <summary>
    /// description: Get context for tracked command
    /// input: IRequest
    /// output: CommandContext
    /// expected_result: Returns the tracked context
    /// </summary>
    [Fact]
    public async Task test_GetForAsync_Generic_WHEN_command_started_THEN_return_context()
    {
        #region Arrange
        Logger.LogDebug("Test ARRANGE");
        var manager = new CommandContextManager();
        var command = new TestCommand();
        var expectedContext = new CommandContext();
        manager.StartFor(command, expectedContext);
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        var actualContext = await manager.GetForAsync(command);
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        Assert.NotNull(actualContext);
        Assert.Equal(expectedContext, actualContext);
        #endregion
    }
}
