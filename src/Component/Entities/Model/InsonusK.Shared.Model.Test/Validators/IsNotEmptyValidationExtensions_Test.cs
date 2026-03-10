using Divergic.Logging.Xunit;
using FluentValidation;
using InsonusK.Shared.Model.Validator.Properties;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace InsonusK.Shared.Model.Test.Validators;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class IsNotEmptyValidationExtensions_Test : LoggingTestsBase<IsNotEmptyValidationExtensions_Test>
{
    public IsNotEmptyValidationExtensions_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug)
        : base(output, logLevel) { }

    // ──────────────────────── String ────────────────────────

    private sealed record StringStub(string Value);
    private class StringStubValidator : AbstractValidator<StringStub>
    {
        public StringStubValidator() => RuleFor(x => x.Value).IsNotEmpty();
    }
    private readonly StringStubValidator _stringValidator = new();

    [Fact]
    public void test_String_WHEN_value_is_empty__THEN_fails_with_IsEmpty_code()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");
        var asserted_model = new StringStub("");
        string expected_errorCode = IsNotEmptyValidationExtensions.Code;
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        var asserted_result = _stringValidator.Validate(asserted_model);
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        Assert.False(asserted_result.IsValid);
        Assert.Contains(asserted_result.Errors, e => e.ErrorCode == expected_errorCode && e.Severity == Severity.Error);
        #endregion
    }

    [Fact]
    public void test_String_WHEN_value_is_valid__THEN_passes()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");
        var asserted_model = new StringStub("hello");
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        var asserted_result = _stringValidator.Validate(asserted_model);
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        Assert.True(asserted_result.IsValid);
        #endregion
    }

    // ──────────────────────── Guid ────────────────────────

    private sealed record GuidStub(Guid Value);
    private class GuidStubValidator : AbstractValidator<GuidStub>
    {
        public GuidStubValidator() => RuleFor(x => x.Value).IsNotEmpty();
    }
    private readonly GuidStubValidator _guidValidator = new();

    [Fact]
    public void test_Guid_WHEN_value_is_empty_guid__THEN_fails_with_IsEmpty_code()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");
        var asserted_model = new GuidStub(Guid.Empty);
        string expected_errorCode = IsNotEmptyValidationExtensions.Code;
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        var asserted_result = _guidValidator.Validate(asserted_model);
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        Assert.False(asserted_result.IsValid);
        Assert.Contains(asserted_result.Errors, e => e.ErrorCode == expected_errorCode && e.Severity == Severity.Error);
        #endregion
    }

    [Fact]
    public void test_Guid_WHEN_value_is_valid__THEN_passes()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");
        var asserted_model = new GuidStub(Guid.NewGuid());
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        var asserted_result = _guidValidator.Validate(asserted_model);
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        Assert.True(asserted_result.IsValid);
        #endregion
    }

    // ──────────────────────── DateTime ────────────────────────

    private sealed record DateTimeStub(DateTime Value);
    private class DateTimeStubValidator : AbstractValidator<DateTimeStub>
    {
        public DateTimeStubValidator() => RuleFor(x => x.Value).IsNotEmpty();
    }
    private readonly DateTimeStubValidator _dateTimeValidator = new();

    [Fact]
    public void test_DateTime_WHEN_value_is_default__THEN_fails_with_IsEmpty_code()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");
        var asserted_model = new DateTimeStub(default);
        string expected_errorCode = IsNotEmptyValidationExtensions.Code;
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
    public void test_DateTime_WHEN_value_is_valid__THEN_passes()
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

    private sealed record DateTimeOffsetStub(DateTimeOffset Value);
    private class DateTimeOffsetStubValidator : AbstractValidator<DateTimeOffsetStub>
    {
        public DateTimeOffsetStubValidator() => RuleFor(x => x.Value).IsNotEmpty();
    }
    private readonly DateTimeOffsetStubValidator _dateTimeOffsetValidator = new();

    [Fact]
    public void test_DateTimeOffset_WHEN_value_is_default__THEN_fails_with_IsEmpty_code()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");
        var asserted_model = new DateTimeOffsetStub(default);
        string expected_errorCode = IsNotEmptyValidationExtensions.Code;
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        var asserted_result = _dateTimeOffsetValidator.Validate(asserted_model);
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        Assert.False(asserted_result.IsValid);
        Assert.Contains(asserted_result.Errors, e => e.ErrorCode == expected_errorCode && e.Severity == Severity.Error);
        #endregion
    }

    [Fact]
    public void test_DateTimeOffset_WHEN_value_is_valid__THEN_passes()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");
        var asserted_model = new DateTimeOffsetStub(DateTimeOffset.UtcNow);
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        var asserted_result = _dateTimeOffsetValidator.Validate(asserted_model);
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        Assert.True(asserted_result.IsValid);
        #endregion
    }
}
