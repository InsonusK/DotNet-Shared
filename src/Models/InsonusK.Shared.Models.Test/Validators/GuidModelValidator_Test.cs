using Divergic.Logging.Xunit;
using Microsoft.Extensions.Logging;
using InsonusK.Shared.Models.Validators;
using InsonusK.Shared.Models.Common;
using FluentValidation;

namespace InsonusK.Shared.Models.Test.Validators;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class GuidModelValidator_Test : LoggingTestsBase<GuidModelValidator_Test>
{
    private readonly GuidModelValidator _validator = new();

    public GuidModelValidator_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
    {
    }

    private sealed record GuidModelStub(Guid Guid) : IGuidModel;

    [Fact]
    public void test_Guid_WHEN_guid_is_empty__THEN_validation_fails_with_error()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var asserted_model = new GuidModelStub(Guid.Empty);
        string expected_errorCode = GuidModelValidator.GuidEmptyCode;

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
    public void test_Guid_WHEN_guid_is_valid__THEN_validation_passes()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var asserted_model = new GuidModelStub(Guid.NewGuid());

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
