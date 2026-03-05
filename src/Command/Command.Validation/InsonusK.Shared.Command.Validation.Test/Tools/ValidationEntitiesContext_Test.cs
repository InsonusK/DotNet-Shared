using System;
using Divergic.Logging.Xunit;
using InsonusK.Shared.Command.Validation.Tools;
using InsonusK.Shared.DataBase.Models;
using InsonusK.Shared.Models.Common;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace InsonusK.Shared.Command.Validation.Test.Tools;

public class ContextTestEntity : EntityBase
{
}

public class ContextTestGuidEntity : EntityBase, IGuidModel
{
    public Guid Guid { get; set; }
}

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class ValidationEntitiesContext_Test : LoggingTestsBase<ValidationEntitiesContext_Test>
{
    public ValidationEntitiesContext_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
    {
    }

    /// <summary>
    /// description: Verifies exception when adding a different entity of the same type via Int ID
    /// input: Context already contains an entity with Id=1. Adding same type with Id=2.
    /// output: ArgumentException thrown
    /// expected_result: ArgumentException states "has been already added"
    /// </summary>
    /// <returns></returns>
    [Fact]
    public void test_IntId_WHEN_EntityIdIsDifferent_THEN_ThrowsArgumentException()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var context = new ValidationEntitiesContext();
        var entity1 = new ContextTestEntity { Id = 1 };
        var entity2 = new ContextTestEntity { Id = 2 };
        context.AddEntity(entity1);

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        // Act is combined with Assert below

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        var ex = Assert.Throws<ArgumentException>(() => context.AddEntity(entity2));
        Assert.Contains("has been already added", ex.Message);

        #endregion
    }

    /// <summary>
    /// description: Verifies that adding the exact same entity by Int ID raise exception
    /// input: Context already contains an entity with Id=1. Adding same type with Id=1.
    /// output: Method returns
    /// expected_result: ArgumentException states "has been already added"
    /// </summary>
    /// <returns></returns>
    [Fact]
    public void test_IntId_WHEN_EntityIdIsSame_THEN_ThrowsArgumentException()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var context = new ValidationEntitiesContext();
        var entity1 = new ContextTestEntity { Id = 1 };
        var entity2 = new ContextTestEntity { Id = 1 };
        context.AddEntity(entity1);

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");


        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        var ex = Assert.Throws<ArgumentException>(() => context.AddEntity(entity2));
        Assert.Contains("has been already added", ex.Message);

        #endregion
    }

    /// <summary>
    /// description: Verifies exception when adding a different IGuidModel entity of the same type
    /// input: Context already contains an IGuidModel entity with Guid=A. Adding same type with Guid=B.
    /// output: ArgumentException thrown
    /// expected_result: ArgumentException states "has been already added"
    /// </summary>
    /// <returns></returns>
    [Fact]
    public void test_GuidModel_WHEN_EntityGuidIsDifferent_THEN_ThrowsArgumentException()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var context = new ValidationEntitiesContext();
        var entity1 = new ContextTestGuidEntity { Id = 0, Guid = Guid.NewGuid() };
        var entity2 = new ContextTestGuidEntity { Id = 0, Guid = Guid.NewGuid() };
        context.AddEntity(entity1);

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        // Act is combined with Assert below

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        var ex = Assert.Throws<ArgumentException>(() => context.AddEntity(entity2));
        Assert.Contains("has been already added", ex.Message);

        #endregion
    }

    /// <summary>
    /// description: Verifies that adding the exact same IGuidModel entity by GUID raise exception
    /// input: Context already contains an IGuidModel entity with Guid=A. Adding same type with Guid=A.
    /// output: Method returns
    /// expected_result: ArgumentException states "has been already added"
    /// </summary>
    /// <returns></returns>
    [Fact]
    public void test_GuidModel_WHEN_EntityGuidIsSame_THEN_ThrowsArgumentException()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var guid = Guid.NewGuid();
        var context = new ValidationEntitiesContext();
        var entity1 = new ContextTestGuidEntity { Id = 0, Guid = guid };
        var entity2 = new ContextTestGuidEntity { Id = 0, Guid = guid };
        context.AddEntity(entity1);

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        var ex = Assert.Throws<ArgumentException>(() => context.AddEntity(entity2));
        Assert.Contains("has been already added", ex.Message);

        #endregion
    }

    /// <summary>
    /// description: Verifies that an existing entity can be retrieved
    /// input: Context contains entity T
    /// output: Entity T is returned
    /// expected_result: Returned entity matches inserted entity
    /// </summary>
    /// <returns></returns>
    [Fact]
    public void test_Retrieval_WHEN_EntityExists_THEN_ReturnsEntity()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var context = new ValidationEntitiesContext();
        var entity = new ContextTestEntity { Id = 1 };
        context.AddEntity(entity);

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_result = context.GetEntity<ContextTestEntity>();

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.Equal(entity, asserted_result);

        #endregion
    }

    /// <summary>
    /// description: Verifies exception when retrieving non-existent entity
    /// input: Context is empty
    /// output: ArgumentException thrown
    /// expected_result: ArgumentException states "not found"
    /// </summary>
    /// <returns></returns>
    [Fact]
    public void test_Retrieval_WHEN_EntityDoesNotExist_THEN_ThrowsArgumentException()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var context = new ValidationEntitiesContext();

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        // Act is combined with Assert below

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        var ex = Assert.Throws<ArgumentException>(() => context.GetEntity<ContextTestEntity>());
        Assert.Contains("not found", ex.Message);

        #endregion
    }

    /// <summary>
    /// description: Verifies TryGetEntity returns true and the entity
    /// input: Context contains entity T
    /// output: Method returns true and entity T out parameter
    /// expected_result: out entity matches inserted entity
    /// </summary>
    /// <returns></returns>
    [Fact]
    public void test_TryRetrieval_WHEN_EntityExists_THEN_ReturnsTrueAndEntity()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var context = new ValidationEntitiesContext();
        var entity = new ContextTestEntity { Id = 1 };
        context.AddEntity(entity);

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_bool = context.TryGetEntity<ContextTestEntity>(out var asserted_entity);

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.True(asserted_bool);
        Assert.Equal(entity, asserted_entity);

        #endregion
    }

    /// <summary>
    /// description: Verifies TryGetEntity returns false for missing entity
    /// input: Context is empty
    /// output: Method returns false and null out parameter
    /// expected_result: out entity is null
    /// </summary>
    /// <returns></returns>
    [Fact]
    public void test_TryRetrieval_WHEN_EntityDoesNotExist_THEN_ReturnsFalseAndNull()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var context = new ValidationEntitiesContext();

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_bool = context.TryGetEntity<ContextTestEntity>(out var asserted_entity);

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.False(asserted_bool);
        Assert.Null(asserted_entity);

        #endregion
    }

    /// <summary>
    /// description: Verifies HasEntity returns true
    /// input: Context contains entity T
    /// output: Method returns true
    /// expected_result: Result is true
    /// </summary>
    /// <returns></returns>
    [Fact]
    public void test_Check_WHEN_EntityExists_THEN_ReturnsTrue()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var context = new ValidationEntitiesContext();
        var entity = new ContextTestEntity { Id = 1 };
        context.AddEntity(entity);

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_result = context.HasEntity<ContextTestEntity>();

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.True(asserted_result);

        #endregion
    }

    /// <summary>
    /// description: Verifies HasEntity returns false
    /// input: Context is empty
    /// output: Method returns false
    /// expected_result: Result is false
    /// </summary>
    /// <returns></returns>
    [Fact]
    public void test_Check_WHEN_EntityDoesNotExist_THEN_ReturnsFalse()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var context = new ValidationEntitiesContext();

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_result = context.HasEntity<ContextTestEntity>();

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.False(asserted_result);

        #endregion
    }
}
