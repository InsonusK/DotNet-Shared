using Divergic.Logging.Xunit;
using InsonusK.Shared.Command.EntityLoading.Helper;
using InsonusK.Shared.Command.EntityLoading.Services;
using InsonusK.Shared.Command.Interfaces;
using InsonusK.Shared.DataBase.Models;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit.Abstractions;
using Ardalis.Specification;
using Ardalis.GuardClauses;

namespace InsonusK.Shared.Command.EntityLoading.Test.Helper;

public class TestEntity1 : EntityBase
{
}

public class TestEntity2 : EntityBase
{
}

public class TestEntityKey : IEntityKey
{
    public Type EntityType { get; set; } = typeof(object);
    public string StringId { get; set; } = string.Empty;
}

public class TestCommand : ICommandWithEntityKeys
{
    public IReadOnlyCollection<IEntityKey> EntityKeys { get; set; } = Array.Empty<IEntityKey>();
}

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class CommandContextExtensions_Test : LoggingTestsBase<CommandContextExtensions_Test>
{
    public CommandContextExtensions_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
    {
    }



    /// <summary>
    /// description: Call CreateNewFor with command having multiple entity keys that are all resolved
    /// input: ICommandWithEntityKeys
    /// output: CommandContext
    /// expected_result: Returns context populated with all resolved entities
    /// </summary>
    [Fact]
    public async Task test_CreateNewFor_Generic_WHEN_all_entities_resolved_THEN_return_context()
    {
        #region Arrange
        Logger.LogDebug("Test ARRANGE");
        var serviceProvider = Substitute.For<IServiceProvider>();

        var repo1 = Substitute.For<IReadRepositoryBase<TestEntity1>>();
        var repo2 = Substitute.For<IReadRepositoryBase<TestEntity2>>();
        var entity1 = new TestEntity1 { Id = 1 };
        var entity2 = new TestEntity2 { Id = 2 };

        serviceProvider.GetService(typeof(IReadRepositoryBase<TestEntity1>)).Returns(repo1);
        serviceProvider.GetService(typeof(IReadRepositoryBase<TestEntity2>)).Returns(repo2);

        repo1.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(entity1);
        repo2.GetByIdAsync(2, Arg.Any<CancellationToken>()).Returns(entity2);

        var provider = new EntityProvider(serviceProvider, Output.BuildLoggerFor<EntityProvider>());

        var command = new TestCommand
        {
            EntityKeys = new List<IEntityKey>
            {
                new TestEntityKey { EntityType = typeof(TestEntity1), StringId = "1" },
                new TestEntityKey { EntityType = typeof(TestEntity2), StringId = "2" }
            }
        };
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        var context = await provider.CreateNewFor(command, CancellationToken.None);
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        Assert.NotNull(context);
        Assert.True(context.Has<TestEntity1>());
        Assert.True(context.Has<TestEntity2>());
        Assert.Equal(entity1, context.Get<TestEntity1>());
        Assert.Equal(entity2, context.Get<TestEntity2>());
        #endregion
    }

    /// <summary>
    /// description: Call CreateNewFor where provider returns null for an entity key
    /// input: ICommandWithEntityKeys
    /// output: exception
    /// expected_result: NotFoundException is thrown
    /// </summary>
    [Fact]
    public async Task test_CreateNewFor_Generic_WHEN_entity_not_resolved_THEN_throw_NotFoundException()
    {
        #region Arrange
        Logger.LogDebug("Test ARRANGE");
        var serviceProvider = Substitute.For<IServiceProvider>();
        var repo1 = Substitute.For<IReadRepositoryBase<TestEntity1>>();

        serviceProvider.GetService(typeof(IReadRepositoryBase<TestEntity1>)).Returns(repo1);

        // Return null to simulate not found
        repo1.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns((TestEntity1?)null);

        var provider = new EntityProvider(serviceProvider, Output.BuildLoggerFor<EntityProvider>());

        var command = new TestCommand
        {
            EntityKeys = new List<IEntityKey>
            {
                new TestEntityKey { EntityType = typeof(TestEntity1), StringId = "1" }
            }
        };
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        var exception = await Record.ExceptionAsync(() => provider.CreateNewFor(command, CancellationToken.None));
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        Assert.NotNull(exception);
        Assert.IsType<NotFoundException>(exception);
        #endregion
    }
}
