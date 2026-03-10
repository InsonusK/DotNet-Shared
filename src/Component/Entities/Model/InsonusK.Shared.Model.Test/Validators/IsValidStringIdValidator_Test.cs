using Divergic.Logging.Xunit;
using FluentValidation;
using InsonusK.Shared.Model.Validator.Properties;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace InsonusK.Shared.Model.Test.Validators;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class IsValidStringIdValidator_Test : LoggingTestsBase<IsValidStringIdValidator_Test>
{
    public IsValidStringIdValidator_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug)
        : base(output, logLevel) { }

    private sealed record StringIdStub(string Id);
    private class StringIdStubValidator : AbstractValidator<StringIdStub>
    {
        public StringIdStubValidator() => RuleFor(x => x.Id).IsValidStringId();
    }
    private readonly StringIdStubValidator _validator = new();

    [Fact]
    public void test_StringId_WHEN_value_is_valid_integer__THEN_passes()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");
        var asserted_model = new StringIdStub("42");
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
    public void test_StringId_WHEN_value_is_valid_guid__THEN_passes()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");
        var asserted_model = new StringIdStub(Guid.NewGuid().ToString());
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
    public void test_StringId_WHEN_value_is_not_integer_or_guid__THEN_fails_with_IsNotValidStringId_code()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");
        var asserted_model = new StringIdStub("not-valid-id");
        string expected_errorCode = IsValidStringIdValidator<StringIdStub>.Code;
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
}
