using Divergic.Logging.Xunit;
using FluentValidation.TestHelper;
using InsonusK.Shared.Command.Validation.Test.Mocks;
using InsonusK.Shared.Command.Validation.Validators;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using FluentValidation;

namespace InsonusK.Shared.Command.Validation.Test.Validators;

public class MockBodyValidator : AbstractValidator<string>
{
    public MockBodyValidator()
    {
        RuleFor(x => x).NotEmpty();
    }
}

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class CommandWithBodyValidator_Test : LoggingTestsBase<CommandWithBodyValidator_Test>
{
    private readonly CommandWithBodyValidator<MockCommandWithBody, string> _validator;

    public CommandWithBodyValidator_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
    {
        _validator = new CommandWithBodyValidator<MockCommandWithBody, string>(new[] { new MockBodyValidator() });
    }

    /// <summary>
    /// description: Command body is null but BodyRequired is true
    /// input: command.Body = null, command.BodyRequired = true
    /// output: ValidationResult with error
    /// expected_result: Contains error with ErrorCode "BodyRequired"
    /// </summary>
    [Fact]
    public void test_Validate_WHEN_BodyIsNull_And_Required_THEN_Error()
    {
        #region Arrange
        Logger.LogDebug("Test ARRANGE");
        var command = new MockCommandWithBody { Body = null, BodyRequired = true };
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        var result = _validator.TestValidate(command);
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        result.ShouldHaveValidationErrorFor(cmd => cmd.Body)
              .WithErrorCode("BodyRequired");
        #endregion
    }

    /// <summary>
    /// description: Command body is not null and BodyRequired is true
    /// input: command.Body = "valid body", command.BodyRequired = true
    /// output: ValidationResult without error
    /// expected_result: No validation errors
    /// </summary>
    [Fact]
    public void test_Validate_WHEN_BodyIsNotNull_And_Required_THEN_Success()
    {
        #region Arrange
        Logger.LogDebug("Test ARRANGE");
        var command = new MockCommandWithBody { Body = "valid body", BodyRequired = true };
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        var result = _validator.TestValidate(command);
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        result.ShouldNotHaveValidationErrorFor(cmd => cmd.Body);
        #endregion
    }

    /// <summary>
    /// description: Command body is null but BodyRequired is false
    /// input: command.Body = null, command.BodyRequired = false
    /// output: ValidationResult without error
    /// expected_result: No validation errors
    /// </summary>
    [Fact]
    public void test_Validate_WHEN_BodyIsNull_And_NotRequired_THEN_Success()
    {
        #region Arrange
        Logger.LogDebug("Test ARRANGE");
        var command = new MockCommandWithBody { Body = null, BodyRequired = false };
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        var result = _validator.TestValidate(command);
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        result.ShouldNotHaveValidationErrorFor(cmd => cmd.Body);
        #endregion
    }
}
