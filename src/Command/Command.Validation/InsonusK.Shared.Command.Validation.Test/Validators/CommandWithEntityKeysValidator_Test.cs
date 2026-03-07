using Divergic.Logging.Xunit;
using FluentValidation.TestHelper;
using InsonusK.Shared.Command.Validation.Test.Mocks;
using InsonusK.Shared.Command.Validation.Validators;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using InsonusK.Shared.Command.Interfaces;
using System.Collections.Generic;
using InsonusK.Shared.Command.Interfaces.Models;
using FluentValidation;
using InsonusK.Shared.Command.EntityLoading.Interfaces;
using NSubstitute;
using InsonusK.Shared.Command.Validation.Extensions;
using InsonusK.Shared.DataBase.Models;

namespace InsonusK.Shared.Command.Validation.Test.Validators;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class CommandWithEntityKeysValidator_Test : LoggingTestsBase<CommandWithEntityKeysValidator_Test>
{
    private readonly CommandWithEntityKeysValidator<MockCommandWithEntityKeys> _validator;

    public CommandWithEntityKeysValidator_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
    {
        _validator = new CommandWithEntityKeysValidator<MockCommandWithEntityKeys>();
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
    /// description: Command EntityKeys is null
    /// input: command.EntityKeys = null
    /// output: ValidationResult with error
    /// expected_result: Contains error with ErrorCode "EntityKeysRequired"
    /// </summary>
    [Fact]
    public void test_Validate_WHEN_EntityKeysIsNull_THEN_Error()
    {
        #region Arrange
        Logger.LogDebug("Test ARRANGE");
        var command = new MockCommandWithEntityKeys { EntityKeys = null };
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        var result = _validator.TestValidate(command);
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        result.ShouldHaveValidationErrorFor(cmd => cmd.EntityKeys)
              .WithErrorCode("EntityKeysRequired");
        #endregion
    }

    /// <summary>
    /// description: Command EntityKeys is not null
    /// input: command.EntityKeys = validKeysList
    /// output: ValidationResult without error
    /// expected_result: No "EntityKeysRequired" error, delegates to EntityKeyValidator
    /// </summary>
    [Fact]
    public void test_Validate_WHEN_EntityKeysIsNotNull_THEN_Success()
    {
        #region Arrange
        Logger.LogDebug("Test ARRANGE");
        var command = new MockCommandWithEntityKeys
        {
            EntityKeys = new List<IEntityKey> { new EntityKey<MockEntityModel>("1", "1",false) }
        };
        var validationContext = new ValidationContext<MockCommandWithEntityKeys>(command);
        var cmdCtx = Substitute.For<ICommandContext>();
        cmdCtx.Get<MockEntityModel>().Returns(new MockEntityModel { Id = 1 });
        validationContext.SetEntitiesContext(cmdCtx);
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        var result = _validator.TestValidate(validationContext);
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        // We only assert that the NotNull rule passes. 
        // We don't assert the inner test logic here since it requires a ValidationContext. 
        // It's tested comprehensively in EntityKeyValidator_Test.
        result.ShouldNotHaveValidationErrorFor(cmd => cmd.EntityKeys);
        #endregion
    }
}
