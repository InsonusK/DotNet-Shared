using Divergic.Logging.Xunit;
using Microsoft.Extensions.Logging;
using InsonusK.Shared.Models.Tools;
using Xunit.Abstractions;

namespace InsonusK.Shared.Models.Test.Tools;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class StringIdExtension_Test : LoggingTestsBase<StringIdExtension_Test>
{
    public StringIdExtension_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
    {
    }

    [Fact]
    public void test_ToId_WHEN_valid_integer_string__THEN_returns_true_and_sets_id()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        string input = "42";
        int expected_id = 42;
        Guid expected_guid = Guid.Empty;

        #endregion


        #region Act
        Logger.LogDebug("Test ACT");

        bool asserted_result = input.ToId(out int asserted_id, out Guid asserted_guid);

        #endregion


        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.True(asserted_result);
        Assert.Equal(expected_id, asserted_id);
        Assert.Equal(expected_guid, asserted_guid);

        #endregion
    }

    [Fact]
    public void test_ToId_WHEN_valid_guid_string__THEN_returns_true_and_sets_guid()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        Guid expected_guid = Guid.NewGuid();
        string input = expected_guid.ToString();
        int expected_id = -1;

        #endregion


        #region Act
        Logger.LogDebug("Test ACT");

        bool asserted_result = input.ToId(out int asserted_id, out Guid asserted_guid);

        #endregion


        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.True(asserted_result);
        Assert.Equal(expected_id, asserted_id);
        Assert.Equal(expected_guid, asserted_guid);

        #endregion
    }

    [Fact]
    public void test_ToId_WHEN_null_or_whitespace_string__THEN_returns_false()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        string input = "   ";

        #endregion


        #region Act
        Logger.LogDebug("Test ACT");

        bool asserted_result = input.ToId(out _, out _);

        #endregion


        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.False(asserted_result);

        #endregion
    }

    [Fact]
    public void test_ToId_WHEN_non_id_string__THEN_returns_false()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        string input = "not-a-valid-id";

        #endregion


        #region Act
        Logger.LogDebug("Test ACT");

        bool asserted_result = input.ToId(out _, out _);

        #endregion


        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.False(asserted_result);

        #endregion
    }
}
