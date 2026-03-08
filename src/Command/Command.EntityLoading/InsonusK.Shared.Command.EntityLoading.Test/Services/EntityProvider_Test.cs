using Divergic.Logging.Xunit;
using InsonusK.Shared.Command.EntityLoading.Services;
using InsonusK.Shared.Command.Interfaces;
using InsonusK.Shared.DataBase.Models;
using InsonusK.Shared.Models.Common;
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit.Abstractions;
using Ardalis.Specification;
using InsonusK.Shared.Command.Services.Interfaces;

namespace InsonusK.Shared.Command.EntityLoading.Test.Services;

public class TestEntity : EntityBase
{
    public string Name { get; set; } = string.Empty;
}

public class TestGuidEntity : EntityBase, IGuidModel
{
    public Guid Guid { get; set; }
}

public class TestNormalClass
{
}

public class TestEntityKey : IEntityKey
{
    public Type EntityType { get; set; } = typeof(object);
    public string StringId { get; set; } = string.Empty;
}

public class TestCommand : ICommandWithEntityKeys, IBaseRequest
{
    public IReadOnlyCollection<IEntityKey> EntityKeys { get; set; } = Array.Empty<IEntityKey>();
}
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class EntityProvider_Test : LoggingTestsBase<EntityProvider_Test>
{
    public EntityProvider_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
    {
    }



    /// <summary>
    /// description: Call Resolve where IEntityCommandExtractor is in DI
    /// input: IEntityKey (for TEntity)
    /// output: TEntity
    /// expected_result: Returns entity from extractor mock
    /// </summary>
    [Fact]
    public async Task test_Resolve_Generic_CustomExtractor_WHEN_extractor_registered_THEN_return_entity_from_extractor()
    {
        #region Arrange
        Logger.LogDebug("Test ARRANGE");
        var serviceProvider = Substitute.For<IServiceProvider>();
        var extractor = Substitute.For<IEntityCommandExtractor<TestNormalClass>>();
        var expectedEntity = new TestNormalClass();
        var entityKey = new TestEntityKey { EntityType = typeof(TestNormalClass), StringId = "custom_id" };
        var ct = CancellationToken.None;

        serviceProvider.GetService(typeof(IEntityCommandExtractor<TestNormalClass>)).Returns(extractor);
        extractor.TryGetAsync(entityKey, ct).Returns(expectedEntity);

        var provider = new EntityProvider(serviceProvider, Output.BuildLoggerFor<EntityProvider>());
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        var actualEntity = await provider.Resolve<TestNormalClass>(entityKey, ct);
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        Assert.NotNull(actualEntity);
        Assert.Equal(expectedEntity, actualEntity);
        #endregion
    }

    /// <summary>
    /// description: Call Resolve where TEntity is IGuidModel (no extractor)
    /// input: IEntityKey (Guid string)
    /// output: TEntity
    /// expected_result: Returns entity from IReadRepositoryBase using string id spec
    /// </summary>
    [Fact]
    public async Task test_Resolve_Generic_GuidResolver_WHEN_entity_is_IGuidModel_THEN_return_entity_from_guid_repo()
    {
        #region Arrange
        Logger.LogDebug("Test ARRANGE");
        var serviceProvider = Substitute.For<IServiceProvider>();
        var repo = Substitute.For<IReadRepositoryBase<TestGuidEntity>>();
        var expectedEntity = new TestGuidEntity { Guid = Guid.NewGuid() };
        var entityKey = new TestEntityKey { EntityType = typeof(TestGuidEntity), StringId = expectedEntity.Guid.ToString() };
        var ct = CancellationToken.None;

        serviceProvider.GetService(typeof(IEntityCommandExtractor<TestGuidEntity>)).Returns((object?)null);
        serviceProvider.GetService(typeof(IReadRepositoryBase<TestGuidEntity>)).Returns(repo);
        repo.SingleOrDefaultAsync(Arg.Any<ISingleResultSpecification<TestGuidEntity>>(), ct).Returns(expectedEntity);

        var provider = new EntityProvider(serviceProvider, Output.BuildLoggerFor<EntityProvider>());
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        var actualEntity = await provider.Resolve<TestGuidEntity>(entityKey, ct);
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        Assert.NotNull(actualEntity);
        Assert.Equal(expectedEntity, actualEntity);
        #endregion
    }

    /// <summary>
    /// description: Call Resolve where TEntity is EntityBase (not IGuidModel, no extractor)
    /// input: IEntityKey (int string)
    /// output: TEntity
    /// expected_result: Returns entity from IReadRepositoryBase using int id
    /// </summary>
    [Fact]
    public async Task test_Resolve_Generic_IntResolver_WHEN_entity_is_EntityBase_THEN_return_entity_from_int_repo()
    {
        #region Arrange
        Logger.LogDebug("Test ARRANGE");
        var serviceProvider = Substitute.For<IServiceProvider>();
        var repo = Substitute.For<IReadRepositoryBase<TestEntity>>();
        var expectedEntity = new TestEntity { Id = 10 };
        var entityKey = new TestEntityKey { EntityType = typeof(TestEntity), StringId = "10" };
        var ct = CancellationToken.None;

        serviceProvider.GetService(typeof(IEntityCommandExtractor<TestEntity>)).Returns((object?)null);
        serviceProvider.GetService(typeof(IReadRepositoryBase<TestEntity>)).Returns(repo);
        repo.GetByIdAsync(10, ct).Returns(expectedEntity);

        var provider = new EntityProvider(serviceProvider, Output.BuildLoggerFor<EntityProvider>());
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        var actualEntity = await provider.Resolve<TestEntity>(entityKey, ct);
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        Assert.NotNull(actualEntity);
        Assert.Equal(expectedEntity, actualEntity);
        #endregion
    }

    /// <summary>
    /// description: Call Resolve for normal class without extractor, not IGuidModel/EntityBase
    /// input: IEntityKey
    /// output: null
    /// expected_result: Returns null
    /// </summary>
    [Fact]
    public async Task test_Resolve_Generic_Unresolvable_WHEN_entity_does_not_match_patterns_THEN_return_null()
    {
        #region Arrange
        Logger.LogDebug("Test ARRANGE");
        var serviceProvider = Substitute.For<IServiceProvider>();
        var entityKey = new TestEntityKey { EntityType = typeof(TestNormalClass), StringId = "unknown" };
        var ct = CancellationToken.None;

        serviceProvider.GetService(typeof(IEntityCommandExtractor<TestNormalClass>)).Returns((object?)null);

        var provider = new EntityProvider(serviceProvider, Output.BuildLoggerFor<EntityProvider>());
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        var actualEntity = await provider.Resolve<TestNormalClass>(entityKey, ct);
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        Assert.Null(actualEntity);
        #endregion
    }

    /// <summary>
    /// description: Call Resolve for EntityBase but stringId is not valid int
    /// input: IEntityKey (not int)
    /// output: null
    /// expected_result: Returns null
    /// </summary>
    [Fact]
    public async Task test_Resolve_Generic_IntResolver_WHEN_id_is_not_int_THEN_return_null()
    {
        #region Arrange
        Logger.LogDebug("Test ARRANGE");
        var serviceProvider = Substitute.For<IServiceProvider>();
        var repo = Substitute.For<IReadRepositoryBase<TestEntity>>();
        var entityKey = new TestEntityKey { EntityType = typeof(TestEntity), StringId = "not_an_int" };
        var ct = CancellationToken.None;

        serviceProvider.GetService(typeof(IEntityCommandExtractor<TestEntity>)).Returns((object?)null);
        serviceProvider.GetService(typeof(IReadRepositoryBase<TestEntity>)).Returns(repo);

        var provider = new EntityProvider(serviceProvider, Output.BuildLoggerFor<EntityProvider>());
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        var actualEntity = await provider.Resolve<TestEntity>(entityKey, ct);
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        Assert.Null(actualEntity);
        #endregion
    }

    /// <summary>
    /// description: Dynamically resolve entity by IEntityKey
    /// input: IEntityKey
    /// output: object
    /// expected_result: Returns the resolved object using underlying generic method
    /// </summary>
    [Fact]
    public async Task test_Resolve_Dynamic_Generic_WHEN_entity_found_THEN_return_object()
    {
        #region Arrange
        Logger.LogDebug("Test ARRANGE");
        var serviceProvider = Substitute.For<IServiceProvider>();
        var repo = Substitute.For<IReadRepositoryBase<TestEntity>>();
        var expectedEntity = new TestEntity { Id = 15 };
        var entityKey = new TestEntityKey { EntityType = typeof(TestEntity), StringId = "15" };
        var ct = CancellationToken.None;

        serviceProvider.GetService(typeof(IEntityCommandExtractor<TestEntity>)).Returns((object?)null);
        serviceProvider.GetService(typeof(IReadRepositoryBase<TestEntity>)).Returns(repo);
        repo.GetByIdAsync(15, ct).Returns(expectedEntity);

        var provider = new EntityProvider(serviceProvider, Output.BuildLoggerFor<EntityProvider>());
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        var actualEntity = await provider.Resolve(entityKey, ct);
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        Assert.NotNull(actualEntity);
        Assert.Equal(expectedEntity, actualEntity);
        #endregion
    }
}
