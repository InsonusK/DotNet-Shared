using Divergic.Logging.Xunit;
using Microsoft.Extensions.Logging;
using InsonusK.Shared.Models.Tools;
using Xunit.Abstractions;

namespace InsonusK.Shared.Models.Test.Tools;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class ClientActionTimeStampParser_Test : LoggingTestsBase<ClientActionTimeStampParser_Test>
{
    public ClientActionTimeStampParser_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
    {
    }

    [Fact]
    public void test_ParseUserActionTimeStamp_WHEN_valid_datetimeoffset_string__THEN_returns_parsed_value()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        DateTimeOffset expected_result = new DateTimeOffset(2024, 6, 15, 12, 30, 0, TimeSpan.Zero);
        string input = expected_result.ToString("o");

        #endregion


        #region Act
        Logger.LogDebug("Test ACT");

        DateTimeOffset asserted_result = input.ParseUserActionTimeStamp();

        #endregion


        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.Equal(expected_result, asserted_result);

        #endregion
    }

    [Fact]
    public void test_ParseUserActionTimeStamp_WHEN_null_or_whitespace_string__THEN_returns_default()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        string input = "   ";
        DateTimeOffset expected_result = default;

        #endregion


        #region Act
        Logger.LogDebug("Test ACT");

        DateTimeOffset asserted_result = input.ParseUserActionTimeStamp();

        #endregion


        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.Equal(expected_result, asserted_result);

        #endregion
    }

    [Fact]
    public void test_ParseUserActionTimeStamp_WHEN_unparseable_string__THEN_returns_default()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        string input = "not-a-date";
        DateTimeOffset expected_result = default;

        #endregion


        #region Act
        Logger.LogDebug("Test ACT");

        DateTimeOffset asserted_result = input.ParseUserActionTimeStamp();

        #endregion


        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.Equal(expected_result, asserted_result);

        #endregion
    }
}
