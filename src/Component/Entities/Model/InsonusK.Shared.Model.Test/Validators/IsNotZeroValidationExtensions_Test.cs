using Divergic.Logging.Xunit;
using FluentValidation;
using InsonusK.Shared.Model.Validator.Properties;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace InsonusK.Shared.Model.Test.Validators;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class IsNotZeroValidationExtensions_Test : LoggingTestsBase<IsNotZeroValidationExtensions_Test>
{
    public IsNotZeroValidationExtensions_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug)
        : base(output, logLevel) { }

    private sealed record IntStub(int Value);
    private class IntStubValidator : AbstractValidator<IntStub>
    {
        public IntStubValidator() => RuleFor(x => x.Value).IsNotZero();
    }
    private readonly IntStubValidator _validator = new();

    [Fact]
    public void test_Int_WHEN_value_is_zero__THEN_fails_with_IsZero_code()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");
        var asserted_model = new IntStub(0);
        string expected_errorCode = IsNotZeroValidationExtensions.Code;
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        var asserted_result = _validator.Validate(asserted_model);
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        Assert.False(asserted_result.IsValid);
        Assert.Contains(asserted_result.Errors, e => e.ErrorCode == expected_errorCode && e.Severity == Severity.Error);
        #endregion
    }

    [Fact]
    public void test_Int_WHEN_value_is_positive__THEN_passes()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");
        var asserted_model = new IntStub(5);
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        var asserted_result = _validator.Validate(asserted_model);
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        Assert.True(asserted_result.IsValid);
        #endregion
    }

    [Fact]
    public void test_Int_WHEN_value_is_negative__THEN_passes()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");
        var asserted_model = new IntStub(-5);
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        var asserted_result = _validator.Validate(asserted_model);
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        Assert.True(asserted_result.IsValid);
        #endregion
    }
}
