using Divergic.Logging.Xunit;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using InsonusK.Shared.DataBase.Spec;
using InsonusK.Shared.DataBase.Models;

namespace InsonusK.Shared.DataBase.Test.Spec;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class ByStringIdsSpec_Test : LoggingTestsBase<ByStringIdsSpec_Test>
{
    public ByStringIdsSpec_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
    {
    }

    [Fact]
    public void test_WrongFormat_WHEN_all_strings_are_valid_guids__THEN_WrongFormat_is_false()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var expected_wrongFormat = false;
        var input = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };

        #endregion


        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_spec = new ByStringIdsSpec<TestEntity>(input);

        #endregion


        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.Equal(expected_wrongFormat, asserted_spec.WrongFormat);
        Assert.Empty(asserted_spec.WrongFormatList);

        #endregion
    }

    [Fact]
    public void test_WrongFormat_WHEN_all_strings_are_valid_ints__THEN_WrongFormat_is_false()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var expected_wrongFormat = false;
        var input = new[] { "1", "2", "3" };

        #endregion


        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_spec = new ByStringIdsSpec<TestEntity>(input);

        #endregion


        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.Equal(expected_wrongFormat, asserted_spec.WrongFormat);
        Assert.Empty(asserted_spec.WrongFormatList);

        #endregion
    }

    [Fact]
    public void test_WrongFormat_WHEN_some_strings_are_invalid__THEN_WrongFormat_is_true_and_list_contains_invalid_entries()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        string expected_invalidEntry = "bad-id";
        var input = new[] { "1", expected_invalidEntry };

        #endregion


        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_spec = new ByStringIdsSpec<TestEntity>(input);

        #endregion


        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.True(asserted_spec.WrongFormat);
        Assert.Contains(expected_invalidEntry, asserted_spec.WrongFormatList);

        #endregion
    }

    [Fact]
    public void test_Constructor_WHEN_all_strings_are_invalid__THEN_throws_ArgumentException()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var input = new[] { "bad-id-1", "bad-id-2" };

        #endregion


        #region Act & Assert
        Logger.LogDebug("Test ACT");

        Assert.Throws<ArgumentException>(() => new ByStringIdsSpec<TestEntity>(input));

        #endregion
    }

    [Fact]
    public void test_Constructor_WHEN_passed_empty_collection__THEN_throws()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var input = Array.Empty<string>();

        #endregion


        #region Act & Assert
        Logger.LogDebug("Test ACT");

        Assert.ThrowsAny<Exception>(() => new ByStringIdsSpec<TestEntity>(input));

        #endregion
    }
}
