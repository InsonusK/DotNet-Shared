using Divergic.Logging.Xunit;
using FluentValidation;
using InsonusK.Shared.Command.Validation.Validators;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace InsonusK.Shared.Command.Validation.Test.Validators;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class CompositeValidator_Test : LoggingTestsBase<CompositeValidator_Test>
{
    public CompositeValidator_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
    {
    }

    private sealed record TestModel(string Value);

    private class AlwaysFailValidator : AbstractValidator<TestModel>
    {
        public AlwaysFailValidator()
        {
            RuleFor(x => x.Value).Must(_ => false).WithErrorCode("AlwaysFail").WithMessage("Always fails.");
        }
    }

    private class AlwaysPassValidator : AbstractValidator<TestModel>
    {
        public AlwaysPassValidator()
        {
            RuleFor(x => x.Value).Must(_ => true);
        }
    }

    /// <summary>
    /// description: No validators are provided
    /// input: empty validator list
    /// output: ValidationResult passes
    /// expected_result: IsValid is true
    /// </summary>
    [Fact]
    public void test_Validate_WHEN_no_validators__THEN_passes()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var composite = new CompositeValidator<TestModel>(Enumerable.Empty<IValidator<TestModel>>());
        var asserted_model = new TestModel("anything");

        #endregion


        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_result = composite.Validate(asserted_model);

        #endregion


        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.True(asserted_result.IsValid);

        #endregion
    }

    /// <summary>
    /// description: One failing validator is provided
    /// input: one validator that always fails
    /// output: ValidationResult fails
    /// expected_result: IsValid is false, contains "AlwaysFail" error
    /// </summary>
    [Fact]
    public void test_Validate_WHEN_one_validator_fails__THEN_fails()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var composite = new CompositeValidator<TestModel>(new IValidator<TestModel>[] { new AlwaysFailValidator() });
        var asserted_model = new TestModel("value");
        string expected_errorCode = "AlwaysFail";

        #endregion


        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_result = composite.Validate(asserted_model);

        #endregion


        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.False(asserted_result.IsValid);
        Assert.Contains(asserted_result.Errors, e => e.ErrorCode == expected_errorCode);

        #endregion
    }

    /// <summary>
    /// description: Multiple validators all pass
    /// input: two always-pass validators
    /// output: ValidationResult passes
    /// expected_result: IsValid is true
    /// </summary>
    [Fact]
    public void test_Validate_WHEN_multiple_validators_all_pass__THEN_passes()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var composite = new CompositeValidator<TestModel>(new IValidator<TestModel>[]
        {
            new AlwaysPassValidator(),
            new AlwaysPassValidator()
        });
        var asserted_model = new TestModel("value");

        #endregion


        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_result = composite.Validate(asserted_model);

        #endregion


        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.True(asserted_result.IsValid);

        #endregion
    }

    /// <summary>
    /// description: Multiple validators with one failing
    /// input: one pass validator and one fail validator
    /// output: ValidationResult fails
    /// expected_result: IsValid is false, contains "AlwaysFail" error
    /// </summary>
    [Fact]
    public void test_Validate_WHEN_multiple_validators_and_one_fails__THEN_fails()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var composite = new CompositeValidator<TestModel>(new IValidator<TestModel>[]
        {
            new AlwaysPassValidator(),
            new AlwaysFailValidator()
        });
        var asserted_model = new TestModel("value");
        string expected_errorCode = "AlwaysFail";

        #endregion


        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_result = composite.Validate(asserted_model);

        #endregion


        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.False(asserted_result.IsValid);
        Assert.Contains(asserted_result.Errors, e => e.ErrorCode == expected_errorCode);

        #endregion
    }
}
