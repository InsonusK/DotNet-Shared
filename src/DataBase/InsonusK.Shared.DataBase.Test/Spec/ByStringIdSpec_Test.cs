using Divergic.Logging.Xunit;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using InsonusK.Shared.DataBase.Spec;
using InsonusK.Shared.DataBase.Models;

namespace InsonusK.Shared.DataBase.Test.Spec;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class ByStringIdSpec_Test : LoggingTestsBase<ByStringIdSpec_Test>
{
    public ByStringIdSpec_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
    {
    }

    [Fact]
    public void test_TryBuild_WHEN_string_is_valid_guid__THEN_returns_true_and_spec_is_not_null()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        string input = Guid.NewGuid().ToString();

        #endregion


        #region Act
        Logger.LogDebug("Test ACT");

        bool asserted_result = ByStringIdSpec<TestEntity>.TryBuild(input, out var asserted_spec);

        #endregion


        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.True(asserted_result);
        Assert.NotNull(asserted_spec);

        #endregion
    }

    [Fact]
    public void test_TryBuild_WHEN_string_is_valid_int__THEN_returns_true_and_spec_is_not_null()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        string input = "42";

        #endregion


        #region Act
        Logger.LogDebug("Test ACT");

        bool asserted_result = ByStringIdSpec<TestEntity>.TryBuild(input, out var asserted_spec);

        #endregion


        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.True(asserted_result);
        Assert.NotNull(asserted_spec);

        #endregion
    }

    [Fact]
    public void test_TryBuild_WHEN_string_is_not_guid_or_int__THEN_returns_false_and_spec_is_null()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        string input = "not-an-id";

        #endregion


        #region Act
        Logger.LogDebug("Test ACT");

        bool asserted_result = ByStringIdSpec<TestEntity>.TryBuild(input, out var asserted_spec);

        #endregion


        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.False(asserted_result);
        Assert.NotNull(asserted_spec);
        Assert.True(asserted_spec.QueryIsEmpty);

        #endregion
    }

    [Fact]
    public void test_StringConstructor_WHEN_string_is_neither_guid_nor_int_and_tryParse_is_false__THEN_throws_ArgumentException()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        string input = "not-an-id";

        #endregion


        #region Act & Assert
        Logger.LogDebug("Test ACT");

        Assert.Throws<ArgumentException>(() => new ByStringIdSpec<TestEntity>(input, ExceptinoIfNotParsed: true));

        #endregion
    }

    [Fact]
    public void test_StringConstructor_WHEN_string_is_neither_guid_nor_int_and_tryParse_is_true__THEN_QueryIsEmpty_is_true()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        string input = "not-an-id";
        bool expected_queryIsEmpty = true;

        #endregion


        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_spec = new ByStringIdSpec<TestEntity>(input, ExceptinoIfNotParsed: false);

        #endregion


        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.Equal(expected_queryIsEmpty, asserted_spec.QueryIsEmpty);

        #endregion
    }

    [Fact]
    public void test_StringConstructor_WHEN_string_is_valid_guid__THEN_QueryIsEmpty_is_false()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        string input = Guid.NewGuid().ToString();
        bool expected_queryIsEmpty = false;

        #endregion


        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_spec = new ByStringIdSpec<TestEntity>(input);

        #endregion


        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.Equal(expected_queryIsEmpty, asserted_spec.QueryIsEmpty);

        #endregion
    }
}
