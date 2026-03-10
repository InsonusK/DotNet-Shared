using Divergic.Logging.Xunit;
using FluentValidation;
using InsonusK.Shared.Model.Validator.Properties;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace InsonusK.Shared.Model.Test.Validators;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class IsUtcValidationExtensions_Test : LoggingTestsBase<IsUtcValidationExtensions_Test>
{
    public IsUtcValidationExtensions_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug)
        : base(output, logLevel) { }

    // ──────────────────────── DateTime ────────────────────────

    private sealed record DateTimeStub(DateTime Value);
    private class DateTimeStubValidator : AbstractValidator<DateTimeStub>
    {
        public DateTimeStubValidator() => RuleFor(x => x.Value).IsUtc();
    }
    private readonly DateTimeStubValidator _dateTimeValidator = new();

    [Fact]
    public void test_DateTime_WHEN_kind_is_unspecified__THEN_fails_with_IsNotUtc_code()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");
        var asserted_model = new DateTimeStub(new DateTime(2024, 6, 1, 12, 0, 0)); // Unspecified
        string expected_errorCode = IsUtcValidationExtensions.Code;
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        var asserted_result = _dateTimeValidator.Validate(asserted_model);
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        Assert.False(asserted_result.IsValid);
        Assert.Contains(asserted_result.Errors, e => e.ErrorCode == expected_errorCode && e.Severity == Severity.Error);
        #endregion
    }

    [Fact]
    public void test_DateTime_WHEN_kind_is_local__THEN_passes()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");
        var asserted_model = new DateTimeStub(DateTime.Now); // Local — has timezone info
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        var asserted_result = _dateTimeValidator.Validate(asserted_model);
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        Assert.True(asserted_result.IsValid);
        #endregion
    }

    [Fact]
    public void test_DateTime_WHEN_kind_is_utc__THEN_passes()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");
        var asserted_model = new DateTimeStub(DateTime.UtcNow);
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        var asserted_result = _dateTimeValidator.Validate(asserted_model);
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        Assert.True(asserted_result.IsValid);
        #endregion
    }

    // ──────────────────────── DateTimeOffset ────────────────────────
    // DateTimeOffset always embeds an offset by the type's contract, so
    // timezone information is always present. IsUtc() has no overload for
    // DateTimeOffset — there is nothing to validate.
}
