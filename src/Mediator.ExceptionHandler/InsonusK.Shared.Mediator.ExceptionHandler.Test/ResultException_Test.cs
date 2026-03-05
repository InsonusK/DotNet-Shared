using Microsoft.Extensions.Logging;
using Divergic.Logging.Xunit;
using Xunit.Abstractions;
using Ardalis.Result;

namespace InsonusK.Shared.Mediator.ExceptionHandler.Test;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class ResultException_Test : LoggingTestsBase<ResultException_Test>
{
    public ResultException_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
    {
    }

    [Fact]
    public void test_ToResult_WHEN_StatusIsError_THEN_ReturnsResultWithError()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var expected_errors = new[] { "Error 1", "Error 2" };
        var innerResult = Result.Error(new ErrorList(expected_errors));
        var exception = new ResultException(innerResult);

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_result = exception.ToResult();

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.Equal(ResultStatus.Error, asserted_result.Status);
        Assert.Equal(expected_errors, asserted_result.Errors);

        #endregion
    }

    [Fact]
    public void test_ToResultOfT_WHEN_StatusIsInvalid_THEN_ReturnsResultOfTWithValidationErrors()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var expected_validationErrors = new List<ValidationError>
        {
            new ValidationError { Identifier = "Field", ErrorMessage = "Invalid format"}
        };
        var innerResult = Result.Invalid(expected_validationErrors);
        var exception = new ResultException(innerResult);

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_result = exception.ToResultOf<string>();

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.Equal(ResultStatus.Invalid, asserted_result.Status);
        Assert.Single(asserted_result.ValidationErrors);
        Assert.Equal("Field", asserted_result.ValidationErrors.ElementAt(0).Identifier);

        #endregion
    }

    [Theory]
    [InlineData(ResultStatus.Ok)]
    [InlineData(ResultStatus.Created)]
    public void test_ToResult_WHEN_StatusIsOkOrCreated_THEN_ThrowsArgumentOutOfRangeException(ResultStatus status)
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        IResult innerResult = status == ResultStatus.Ok ? Result.Success() : Result.Created("");
        var exception = new ResultException(innerResult);

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        
        var act = () => exception.ToResult();

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        var asserted_exception = Assert.Throws<ArgumentOutOfRangeException>(act);
        Assert.Contains("Result must be not Ok or Created", asserted_exception.Message);

        #endregion
    }

    [Fact]
    public void test_ToResult_WHEN_StatusIsNotFound_THEN_ReturnsResultWithNotFoundStatus()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var expected_errors = new[] { "Not found error" };
        var innerResult = Result.NotFound(expected_errors);
        var exception = new ResultException(innerResult);

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_result = exception.ToResult();

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.Equal(ResultStatus.NotFound, asserted_result.Status);
        Assert.Equal(expected_errors, asserted_result.Errors);

        #endregion
    }
}
