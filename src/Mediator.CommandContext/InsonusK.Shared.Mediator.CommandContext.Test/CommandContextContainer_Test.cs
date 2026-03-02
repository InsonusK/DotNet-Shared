using Microsoft.Extensions.Logging;
using Divergic.Logging.Xunit;
using Xunit.Abstractions;
using NSubstitute;
using Microsoft.Extensions.DependencyInjection;
using InsonusK.Shared.Mediator.CommandContext.Service;
using InsonusK.Shared.Mediator.CommandContext.Interfaces;
using InsonusK.Shared.DataBase.Models;
using Ardalis.Specification;
using InsonusK.Shared.Mediator.ExceptionHandler;
using InsonusK.Shared.Models.Common;

namespace InsonusK.Shared.Mediator.CommandContext.Test;
public class TestEntity : EntityBase
    {
    }

    public class TestGuidEntity : EntityBase, IGuidModel
    {
        public Guid Guid { get; set; } = Guid.NewGuid();
    }

    public class TestVersionedEntity : EntityBase, IVersionatedModel
    {
        public uint Version { get; set; }
    }

    public class TestEntityKey : IEntityKey
    {
        public string EntityStringId { get; init; } = "";
        public Type EntityType => typeof(TestEntity);
    }

    public class TestEntityKeyWithVersion : IEntityKeyWithVersion
    {
        public string EntityStringId { get; init; } = "";
        public Type EntityType => typeof(TestVersionedEntity);
        public uint Version { get; init; }
    }
    
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class CommandContextContainer_Test : LoggingTestsBase<CommandContextContainer_Test>
{
    public CommandContextContainer_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
    {
    }

    

    private CommandContextContainer CreateContainer(IServiceProvider provider)
    {
        return new CommandContextContainer(provider);
    }

    [Fact]
    public async Task AddEntityAsync_WHEN_CalledWithId_THEN_FetchesAndAddsEntity()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var entity = new TestEntity { Id = 1 };
        var repo = Substitute.For<IReadRepositoryBase<TestEntity>>();
        repo.GetByIdAsync(1).Returns(Task.FromResult((TestEntity?)entity));

        var services = new ServiceCollection();
        services.AddSingleton(repo);
        var provider = services.BuildServiceProvider();

        var container = CreateContainer(provider);

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        await container.AddEntityAsync<TestEntity>(1);

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        var asserted_entity = container.Entity<TestEntity>();
        Assert.Equal(entity, asserted_entity);

        #endregion
    }

    [Fact]
    public async Task AddEntityFromEntityKeyAsync_WHEN_ValidKey_THEN_FetchesAndAddsEntity()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var key = new TestEntityKey();
        var entity = new TestEntity { Id = 2 };
        
        var extractor = Substitute.For<IEntityCommandExtractor<TestEntity>>();
        extractor.GetAsync(key, Arg.Any<CancellationToken>()).Returns(Task.FromResult(entity));

        var services = new ServiceCollection();
        services.AddSingleton(extractor);
        var provider = services.BuildServiceProvider();

        var container = CreateContainer(provider);

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        await container.AddEntityFromEntityKeyAsync(key);

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        var asserted_entity = container.Entity<TestEntity>();
        Assert.Equal(entity, asserted_entity);

        #endregion
    }

    [Fact]
    public async Task AddEntityFromEntityKeyAsync_WHEN_VersionMismatch_THEN_ThrowsResultExceptionConflict()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var key = new TestEntityKeyWithVersion { Version = 1 };
        var entity = new TestVersionedEntity { Id = 3, Version = 2 }; // Mismatch
        
        var extractor = Substitute.For<IEntityCommandExtractor<TestVersionedEntity>>();
        extractor.GetAsync(key, Arg.Any<CancellationToken>()).Returns(Task.FromResult(entity));

        var services = new ServiceCollection();
        services.AddSingleton(extractor);
        var provider = services.BuildServiceProvider();

        var container = CreateContainer(provider);

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        var act = async () => await container.AddEntityFromEntityKeyAsync(key);

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        var asserted_exception = await Assert.ThrowsAsync<ResultException>(act);
        Assert.Equal(Ardalis.Result.ResultStatus.Conflict, asserted_exception.innerResult.Status);

        #endregion
    }

    [Fact]
    public void AddEntity_WHEN_EntityAlreadyAddedWithDifferentId_THEN_ThrowsArgumentException()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var provider = new ServiceCollection().BuildServiceProvider();
        var container = CreateContainer(provider);
        var entity1 = new TestEntity { Id = 1 };
        var entity2 = new TestEntity { Id = 2 };
        container.AddEntity(entity1);

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        var act = () => container.AddEntity(entity2);

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        var asserted_exception = Assert.Throws<ArgumentException>(act);
        Assert.Contains("already added", asserted_exception.Message);

        #endregion
    }

    [Fact]
    public void AddEntity_WHEN_GuidEntityAlreadyAddedWithDifferentGuid_THEN_ThrowsArgumentException()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var provider = new ServiceCollection().BuildServiceProvider();
        var container = CreateContainer(provider);
        var entity1 = new TestGuidEntity { Id = 0, Guid = Guid.NewGuid() };
        var entity2 = new TestGuidEntity { Id = 0, Guid = Guid.NewGuid() };
        container.AddEntity(entity1);

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        var act = () => container.AddEntity(entity2);

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        var asserted_exception = Assert.Throws<ArgumentException>(act);
        Assert.Contains("already added", asserted_exception.Message);

        #endregion
    }

    [Fact]
    public void Entity_WHEN_EntityNotFound_THEN_ThrowsArgumentException()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var provider = new ServiceCollection().BuildServiceProvider();
        var container = CreateContainer(provider);

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        var act = () => container.Entity<TestEntity>();

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        var asserted_exception = Assert.Throws<ArgumentException>(act);
        Assert.Contains("not found", asserted_exception.Message);

        #endregion
    }

    [Fact]
    public void TryGetEntity_WHEN_EntityExists_THEN_ReturnsTrueAndEntity()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var provider = new ServiceCollection().BuildServiceProvider();
        var container = CreateContainer(provider);
        var expected_entity = new TestEntity { Id = 1 };
        container.AddEntity(expected_entity);

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        var success = container.TryGetEntity<TestEntity>(out var asserted_entity);

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.True(success);
        Assert.Equal(expected_entity, asserted_entity);

        #endregion
    }

    [Fact]
    public void TryGetEntity_WHEN_EntityDoesNotExist_THEN_ReturnsFalse()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var provider = new ServiceCollection().BuildServiceProvider();
        var container = CreateContainer(provider);

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        var success = container.TryGetEntity<TestEntity>(out var asserted_entity);

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.False(success);
        Assert.Null(asserted_entity);

        #endregion
    }
}
