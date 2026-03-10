using Divergic.Logging.Xunit;
using FluentValidation;
using InsonusK.Shared.Model.Validator.Properties;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace InsonusK.Shared.Model.Test.Validators;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class IsNotTooLongValidationExtensions_Test : LoggingTestsBase<IsNotTooLongValidationExtensions_Test>
{
    private const int MaxLength = 10;

    public IsNotTooLongValidationExtensions_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug)
        : base(output, logLevel) { }

    private sealed record StringStub(string Value);
    private class StringStubValidator : AbstractValidator<StringStub>
    {
        public StringStubValidator() => RuleFor(x => x.Value).IsNotTooLong(MaxLength);
    }
    private readonly StringStubValidator _validator = new();

    [Fact]
    public void test_String_WHEN_value_exceeds_max_length__THEN_fails_with_IsTooLong_code()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");
        var asserted_model = new StringStub(new string('x', MaxLength + 1));
        string expected_errorCode = IsNotTooLongValidationExtensions.Code;
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
    public void test_String_WHEN_value_is_exactly_max_length__THEN_passes()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");
        var asserted_model = new StringStub(new string('x', MaxLength));
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
    public void test_String_WHEN_value_is_within_max_length__THEN_passes()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");
        var asserted_model = new StringStub("hello");
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
