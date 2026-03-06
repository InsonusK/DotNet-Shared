using Divergic.Logging.Xunit;
using FluentValidation.TestHelper;
using InsonusK.Shared.Command.Validation.Test.Mocks;
using InsonusK.Shared.Command.Validation.Validators.Properties;
using InsonusK.Shared.Command.Validation.Extensions;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using FluentValidation;
using InsonusK.Shared.Command.Interfaces;
using InsonusK.Shared.DataBase.Models;
using System.Collections.Generic;
using InsonusK.Shared.Command.EntityLoading.Interfaces;
using NSubstitute;
using InsonusK.Shared.Command.Interfaces.Models;

namespace InsonusK.Shared.Command.Validation.Test.Validators.Properties;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class EntityKeyValidator_Test : LoggingTestsBase<EntityKeyValidator_Test>
{
    private readonly EntityKeyValidator _validator;

    public EntityKeyValidator_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
    {
        _validator = new EntityKeyValidator();
    }

    private ValidationContext<IEntityKey> CreateContextWithEntity<TEntity>(IEntityKey key, EntityBase entity) where TEntity : EntityBase
    {
        var context = new ValidationContext<IEntityKey>(key);
        ICommandContext cmdCtx = Substitute.For<ICommandContext>();
        if (entity != null)
        {
            cmdCtx.Get<TEntity>().Returns(entity);
        }
        context.SetEntitiesContext(cmdCtx);
        return context;
    }

    /// <summary>
    /// description: Entity cannot be found in ValidationContext
    /// input: context.GetEntityFromContext returns null
    /// output: ValidationResult with error
    /// expected_result: Contains error with ErrorCode "EntityNotFound"
    /// </summary>
    [Fact]
    public void test_Validate_WHEN_EntityNotInContext_THEN_Error()
    {
        #region Arrange
        Logger.LogDebug("Test ARRANGE");
        var key = new EntityKey<MockVersionedEntityModel>("1", "1", true);
        var context = CreateContextWithEntity<MockEntityModel>(key, null);
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        var result = _validator.Validate(context);
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorCode == "EntityNotFound");
        #endregion
    }

    /// <summary>
    /// description: Entity is found but does not implement IVersionatedModel, while VersionRequired is true
    /// input: context.GetEntityFromContext returns IEntityModel but not IVersionatedModel
    /// output: ValidationResult with error
    /// expected_result: Contains error with ErrorCode "VersionPropertyMissing"
    /// </summary>
    [Fact]
    public void test_Validate_WHEN_EntityMissingVersionProperty_THEN_Error()
    {
        #region Arrange
        Logger.LogDebug("Test ARRANGE");
        var key = new EntityKey<MockEntityModel>("1", "1", true);
        var entity = new MockEntityModel(); // Non-versioned entity model
        var context = CreateContextWithEntity<MockEntityModel>(key, entity);
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        var result = _validator.Validate(context);
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorCode == "VersionPropertyMissing");
        #endregion
    }

    /// <summary>
    /// description: Entity is found, IVersionatedModel implemented, but key version string is not a positive integer
    /// input: key.Version = "not_a_number"
    /// output: ValidationResult with error
    /// expected_result: Contains error with ErrorCode "InvalidVersionFormat"
    /// </summary>
    [Fact]
    public void test_Validate_WHEN_VersionIsInvalidFormat_THEN_Error()
    {
        #region Arrange
        Logger.LogDebug("Test ARRANGE");
        var key = new EntityKey<MockVersionedEntityModel>("1", "not_a_number", true);
        var entity = new MockVersionedEntityModel { Id = 1, Version = 2 };
        var context = CreateContextWithEntity<MockVersionedEntityModel>(key, entity);
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        var result = _validator.Validate(context);
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorCode == "InvalidVersionFormat");
        #endregion
    }

    /// <summary>
    /// description: Entity version does not match key version
    /// input: key.Version = "1", existEntity.Version = 2
    /// output: ValidationResult with error
    /// expected_result: Contains error with ErrorCode "VersionMismatch"
    /// </summary>
    [Fact]
    public void test_Validate_WHEN_VersionMismatches_THEN_Error()
    {
        #region Arrange
        Logger.LogDebug("Test ARRANGE");
        var key = new EntityKey<MockVersionedEntityModel>("1", "1", true);
        var entity = new MockVersionedEntityModel { Id = 1, Version = 2 };
        var context = CreateContextWithEntity<MockVersionedEntityModel>(key, entity);
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        var result = _validator.Validate(context);
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorCode == "VersionMismatch");
        #endregion
    }

    /// <summary>
    /// description: Entity version matches key version
    /// input: key.Version = "2", existEntity.Version = 2
    /// output: ValidationResult without error
    /// expected_result: No validation errors
    /// </summary>
    [Fact]
    public void test_Validate_WHEN_VersionMatches_THEN_Success()
    {
        #region Arrange
        Logger.LogDebug("Test ARRANGE");
        var key = new EntityKey<MockVersionedEntityModel>("1", "2", true);
        var entity = new MockVersionedEntityModel { Id = 1, Version = 2 };
        var context = CreateContextWithEntity<MockVersionedEntityModel>(key, entity);
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        var result = _validator.Validate(context);
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        Assert.True(result.IsValid);
        #endregion
    }
}
