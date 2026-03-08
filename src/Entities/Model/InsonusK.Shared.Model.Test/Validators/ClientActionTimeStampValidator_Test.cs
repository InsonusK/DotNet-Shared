using Divergic.Logging.Xunit;
using Microsoft.Extensions.Logging;
using InsonusK.Shared.Models.Validators;
using InsonusK.Shared.Models.Common;
using FluentValidation;
using Xunit.Abstractions;

namespace InsonusK.Shared.Models.Test.Validators;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class ClientActionTimeStampValidator_Test : LoggingTestsBase<ClientActionTimeStampValidator_Test>
{
    private readonly ClientActionTimeStampValidator _validator = new();

    public ClientActionTimeStampValidator_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
    {
    }

    private sealed record ClientActionTimeStampStub(DateTimeOffset ActionTimeStamp) : IClientActionTimeStamp;

    [Fact]
    public void test_ActionTimeStamp_WHEN_actionTimeStamp_is_default__THEN_validation_fails_with_error()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var asserted_model = new ClientActionTimeStampStub(default(DateTimeOffset));
        string expected_errorCode = ClientActionTimeStampValidator.ActionTimeStampEmptyCode;

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
    public void test_ActionTimeStamp_WHEN_actionTimeStamp_is_valid__THEN_validation_passes()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var asserted_model = new ClientActionTimeStampStub(DateTimeOffset.UtcNow);

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
