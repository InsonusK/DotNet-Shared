using Divergic.Logging.Xunit;
using Microsoft.Extensions.Logging;
using InsonusK.Shared.Models.Validators;
using InsonusK.Shared.Models.Interfaces;
using FluentValidation;

namespace InsonusK.Shared.Models.Test.Validators;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class PatchRequestValidator_Test : LoggingTestsBase<PatchRequestValidator_Test>
{
    private readonly PatchRequestValidator _validator = new();

    public PatchRequestValidator_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
    {
    }

    private sealed class PatchRequestStub(bool haveAtLeastOne) : IPatchRequest
    {
        public bool HaveAtLeastOneNonNullField() => haveAtLeastOne;
    }

    [Fact]
    public void test_HaveAtLeastOneNonNullField_WHEN_at_least_one_field_is_not_null__THEN_validation_passes()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var asserted_model = new PatchRequestStub(haveAtLeastOne: true);

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
    public void test_HaveAtLeastOneNonNullField_WHEN_all_fields_are_null__THEN_validation_fails_with_error()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var asserted_model = new PatchRequestStub(haveAtLeastOne: false);
        string expected_errorCode = PatchRequestValidator.AllNullCode;

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
