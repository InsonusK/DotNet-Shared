using Divergic.Logging.Xunit;
using Microsoft.Extensions.Logging;
using InsonusK.Shared.Models.Validators;
using InsonusK.Shared.Models.Template;
using Xunit.Abstractions;

namespace InsonusK.Shared.Models.Test.Validators;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class FetchRequestValidator_Test : LoggingTestsBase<FetchRequestValidator_Test>
{
    private readonly FetchRequestValidator _validator = new();

    public FetchRequestValidator_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
    {
    }

    private sealed record FetchRequestStub(int Page, int PageSize) : IFetchRequest;

    [Fact]
    public void test_Page_WHEN_page_and_pageSize_are_zero__THEN_validation_passes()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var asserted_request = new FetchRequestStub(Page: 0, PageSize: 0);

        #endregion


        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_result = _validator.Validate(asserted_request);

        #endregion


        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.True(asserted_result.IsValid);

        #endregion
    }

    [Fact]
    public void test_Page_WHEN_page_is_negative__THEN_validation_fails()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var asserted_request = new FetchRequestStub(Page: -1, PageSize: 10);

        #endregion


        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_result = _validator.Validate(asserted_request);

        #endregion


        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.False(asserted_result.IsValid);

        #endregion
    }

    [Fact]
    public void test_PageSize_WHEN_pageSize_is_negative__THEN_validation_fails()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var asserted_request = new FetchRequestStub(Page: 0, PageSize: -5);

        #endregion


        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_result = _validator.Validate(asserted_request);

        #endregion


        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.False(asserted_result.IsValid);

        #endregion
    }

    [Fact]
    public void test_Page_WHEN_page_and_pageSize_are_positive__THEN_validation_passes()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var asserted_request = new FetchRequestStub(Page: 1, PageSize: 10);

        #endregion


        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_result = _validator.Validate(asserted_request);

        #endregion


        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.True(asserted_result.IsValid);

        #endregion
    }
}
