using Microsoft.Extensions.Logging;
using Divergic.Logging.Xunit;
using Xunit.Abstractions;
using InsonusK.Shared.Mediator.CommandContext.Models;
using InsonusK.Shared.Mediator.CommandContext.Command;
using InsonusK.Shared.DataBase.Models;
using InsonusK.Shared.Mediator.CommandContext.Interfaces;

namespace InsonusK.Shared.Mediator.CommandContext.Test;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class CommandModels_Test : LoggingTestsBase<CommandModels_Test>
{
    public CommandModels_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
    {
    }

    private class TestEntity : EntityBase { }

    private class TestCommandWithStringIdEntity : CommandWithStringIdEntity<TestEntity>
    {
        public override string EntityStringId { get; init; } = "TestId";
    }

    private class TestCommandWithEntityKeys : CommandWithEntityKeys
    {
        public override IEnumerable<IEntityKey> EntityKeys { get; init; } = Array.Empty<IEntityKey>();
    }

    [Fact]
    public void EntityKey_WHEN_Created_THEN_PropertiesAreSetCorrectly()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var expected_id = "123";

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_key = new EntityKey<TestEntity> { EntityStringId = expected_id };

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.Equal(expected_id, asserted_key.EntityStringId);
        Assert.Equal(typeof(TestEntity), asserted_key.EntityType);

        #endregion
    }

    [Fact]
    public void CommandWithStringIdEntity_WHEN_Created_THEN_PropertiesAreSetCorrectly()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var expected_id = "TestId";

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_command = new TestCommandWithStringIdEntity();

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.Equal(expected_id, asserted_command.EntityStringId);
        Assert.Equal(typeof(TestEntity), asserted_command.EntityType);

        #endregion
    }

    [Fact]
    public void CommandWithEntityKeys_WHEN_Created_THEN_PropertiesAreSetCorrectly()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_command = new TestCommandWithEntityKeys();

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.Empty(asserted_command.EntityKeys);

        #endregion
    }
}
