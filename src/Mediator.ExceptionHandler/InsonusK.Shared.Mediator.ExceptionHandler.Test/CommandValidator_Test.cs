using Microsoft.Extensions.Logging;
using Divergic.Logging.Xunit;
using Xunit.Abstractions;
using InsonusK.Shared.Mediator.ExceptionHandler.Validators;
using InsonusK.Shared.Models.Common;

namespace InsonusK.Shared.Mediator.ExceptionHandler.Test;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class CommandValidator_Test : LoggingTestsBase<CommandValidator_Test>
{
    public CommandValidator_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
    {
    }

    private class TestRequest : IClientActionTimeStamp
    {
        public DateTime ClientActionTimeStamp { get; init; }
    }

    [Fact]
    public void Validate_WHEN_ClientActionTimeStampIsDefault_THEN_ReturnsInvalid()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var validator = new CommandValidator();
        var request = new TestRequest { ClientActionTimeStamp = default };

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_result = validator.Validate(request);

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.False(asserted_result.IsValid);
        Assert.NotEmpty(asserted_result.Errors);

        #endregion
    }

    [Fact]
    public void Validate_WHEN_ClientActionTimeStampIsValid_THEN_ReturnsValid()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var validator = new CommandValidator();
        var request = new TestRequest { ClientActionTimeStamp = DateTime.UtcNow };

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_result = validator.Validate(request);

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.True(asserted_result.IsValid);
        Assert.Empty(asserted_result.Errors);

        #endregion
    }
}
