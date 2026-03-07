using Divergic.Logging.Xunit;
using InsonusK.Shared.Command.EntityLoading.Tools;
using InsonusK.Shared.DataBase.Models;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace InsonusK.Shared.Command.EntityLoading.Test.Tools;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class CommandContext_Test : LoggingTestsBase<CommandContext_Test>
{
    public CommandContext_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
    {
    }

    private class TestEntity : EntityBase
    {
        public string Name { get; set; } = string.Empty;
    }

    private class AnotherTestEntity : EntityBase
    {
    }

    /// <summary>
    /// description: Add a new struct or object by Type
    /// input: Type, object
    /// output: none
    /// expected_result: Entity is added without exceptions
    /// </summary>
    [Fact]
    public void test_AddEntity_Object_WHEN_entity_type_not_added_THEN_add_success()
    {
        #region Arrange
        Logger.LogDebug("Test ARRANGE");
        var context = new CommandContext();
        var entity = new TestEntity { Id = 1, Name = "Test" };
        var type = typeof(TestEntity);
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        var exception = Record.Exception(() => context.AddEntity(type, entity));
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        Assert.Null(exception);
        Assert.True(context.Has<TestEntity>());
        #endregion
    }

    /// <summary>
    /// description: Add same Type twice
    /// input: Type, object
    /// output: exception
    /// expected_result: ArgumentException is thrown
    /// </summary>
    [Fact]
    public void test_AddEntity_Object_WHEN_entity_type_already_added_THEN_throw_ArgumentException()
    {
        #region Arrange
        Logger.LogDebug("Test ARRANGE");
        var context = new CommandContext();
        var entity1 = new TestEntity { Id = 1, Name = "Test" };
        var entity2 = new TestEntity { Id = 2, Name = "Duplicate" };
        var type = typeof(TestEntity);
        context.AddEntity(type, entity1);
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        var exception = Record.Exception(() => context.AddEntity(type, entity2));
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        #endregion
    }

    /// <summary>
    /// description: Add a new EntityBase
    /// input: TEntity (EntityBase)
    /// output: none
    /// expected_result: Entity is added without exceptions
    /// </summary>
    [Fact]
    public void test_Add_Generic_WHEN_entity_not_added_THEN_add_success()
    {
        #region Arrange
        Logger.LogDebug("Test ARRANGE");
        var context = new CommandContext();
        var entity = new AnotherTestEntity { Id = 100 };
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        var exception = Record.Exception(() => context.Add(entity));
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        Assert.Null(exception);
        Assert.True(context.Has<AnotherTestEntity>());
        #endregion
    }

    /// <summary>
    /// description: Get existing added entity
    /// input: Type TEntity
    /// output: TEntity
    /// expected_result: The same instance is returned
    /// </summary>
    [Fact]
    public void test_Get_Generic_WHEN_entity_added_THEN_return_entity()
    {
        #region Arrange
        Logger.LogDebug("Test ARRANGE");
        var context = new CommandContext();
        var expectedEntity = new TestEntity { Id = 5, Name = "GetTest" };
        context.Add(expectedEntity);
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        var actualEntity = context.Get<TestEntity>();
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        Assert.NotNull(actualEntity);
        Assert.Equal(expectedEntity, actualEntity);
        #endregion
    }

    /// <summary>
    /// description: Get non-existent entity
    /// input: Type TEntity
    /// output: exception
    /// expected_result: ArgumentException is thrown
    /// </summary>
    [Fact]
    public void test_Get_Generic_WHEN_entity_not_added_THEN_throw_ArgumentException()
    {
        #region Arrange
        Logger.LogDebug("Test ARRANGE");
        var context = new CommandContext();
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        var exception = Record.Exception(() => context.Get<TestEntity>());
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        #endregion
    }

    /// <summary>
    /// description: Try get existing added entity
    /// input: Type TEntity
    /// output: true, out TEntity
    /// expected_result: Returns true and outs the instance
    /// </summary>
    [Fact]
    public void test_TryGet_Generic_WHEN_entity_added_THEN_return_true_and_entity()
    {
        #region Arrange
        Logger.LogDebug("Test ARRANGE");
        var context = new CommandContext();
        var expectedEntity = new TestEntity { Id = 7, Name = "TryGet" };
        context.Add(expectedEntity);
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        var hasEntity = context.TryGet<TestEntity>(out var actualEntity);
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        Assert.True(hasEntity);
        Assert.NotNull(actualEntity);
        Assert.Equal(expectedEntity, actualEntity);
        #endregion
    }

    /// <summary>
    /// description: Try get non-existent entity
    /// input: Type TEntity
    /// output: false, out null
    /// expected_result: Returns false and outs null
    /// </summary>
    [Fact]
    public void test_TryGet_Generic_WHEN_entity_not_added_THEN_return_false_and_null()
    {
        #region Arrange
        Logger.LogDebug("Test ARRANGE");
        var context = new CommandContext();
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        var hasEntity = context.TryGet<TestEntity>(out var actualEntity);
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        Assert.False(hasEntity);
        Assert.Null(actualEntity);
        #endregion
    }

    /// <summary>
    /// description: Check existing added entity
    /// input: Type TEntity
    /// output: true
    /// expected_result: Returns true
    /// </summary>
    [Fact]
    public void test_Has_Generic_WHEN_entity_added_THEN_return_true()
    {
        #region Arrange
        Logger.LogDebug("Test ARRANGE");
        var context = new CommandContext();
        var entity = new TestEntity { Id = 10 };
        context.Add(entity);
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        var hasEntity = context.Has<TestEntity>();
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        Assert.True(hasEntity);
        #endregion
    }

    /// <summary>
    /// description: Check non-existent entity
    /// input: Type TEntity
    /// output: false
    /// expected_result: Returns false
    /// </summary>
    [Fact]
    public void test_Has_Generic_WHEN_entity_not_added_THEN_return_false()
    {
        #region Arrange
        Logger.LogDebug("Test ARRANGE");
        var context = new CommandContext();
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        var hasEntity = context.Has<AnotherTestEntity>();
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        Assert.False(hasEntity);
        #endregion
    }
}
