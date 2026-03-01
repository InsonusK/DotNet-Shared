using Divergic.Logging.Xunit;
using Microsoft.Extensions.Logging;
using InsonusK.Shared.Models.Validators;
using FluentValidation;
using Xunit.Abstractions;

namespace InsonusK.Shared.Models.Test.Validators;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class StirngIdFormatValidator_Test : LoggingTestsBase<StirngIdFormatValidator_Test>
{
    public StirngIdFormatValidator_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
    {
    }

    private sealed record StringIdStub(string StringId);

    private class StringIdStubValidator : AbstractValidator<StringIdStub>
    {
        public StringIdStubValidator()
        {
            RuleFor(x => x.StringId).StringIdIdFormatIsValid();
        }
    }

    private readonly StringIdStubValidator _validator = new();

    [Fact]
    public void test_StringId_WHEN_value_is_valid_integer_string__THEN_validation_passes()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var asserted_model = new StringIdStub(StringId: "123");

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
    public void test_StringId_WHEN_value_is_valid_guid_string__THEN_validation_passes()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var asserted_model = new StringIdStub(StringId: Guid.NewGuid().ToString());

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
    public void test_StringId_WHEN_value_is_arbitrary_string__THEN_validation_fails()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var asserted_model = new StringIdStub(StringId: "not-an-id");

        #endregion


        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_result = _validator.Validate(asserted_model);

        #endregion


        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.False(asserted_result.IsValid);

        #endregion
    }
}
