using Microsoft.Extensions.Logging;
using Divergic.Logging.Xunit;
using Xunit.Abstractions;
using Ardalis.Result;
using FluentValidation.Results;
using NSubstitute;
using InsonusK.Shared.Mediator.ExceptionHandler.Service;

namespace InsonusK.Shared.Mediator.ExceptionHandler.Test;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class ArdalisResultReflectionFactory_Test : LoggingTestsBase<ArdalisResultReflectionFactory_Test>
{
    public ArdalisResultReflectionFactory_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
    {
    }

    private ArdalisResultReflectionFactory<Result<string>> CreateStringResultFactory()
    {
        var logger = Substitute.For<ILogger<ArdalisResultReflectionFactory<Result<string>>>>();
        return new ArdalisResultReflectionFactory<Result<string>>(logger);
    }
    
    private ArdalisResultReflectionFactory<Result> CreateResultFactory()
    {
        var logger = Substitute.For<ILogger<ArdalisResultReflectionFactory<Result>>>();
        return new ArdalisResultReflectionFactory<Result>(logger);
    }

    [Theory]
    [InlineData(ResultStatus.NotFound, "Not Found Error")]
    [InlineData(ResultStatus.Error, "General Error")]
    [InlineData(ResultStatus.Forbidden, "Forbidden Error")]
    [InlineData(ResultStatus.Unauthorized, "Unauthorized Error")]
    [InlineData(ResultStatus.Unavailable, "Unavailable Error")]
    [InlineData(ResultStatus.Conflict, "Conflict Error")]
    [InlineData(ResultStatus.CriticalError, "Critical Error")]
    public void Map_WHEN_GenericResultHasVaryingStatus_THEN_ReturnsMappedGenericResult(ResultStatus status, string errorMessage)
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var factory = CreateStringResultFactory();
        IResult innerResult = status switch
        {
            ResultStatus.NotFound => Result<int>.NotFound(errorMessage),
            ResultStatus.Error => Result<int>.Error(errorMessage),
            ResultStatus.Forbidden => Result<int>.Forbidden(errorMessage),
            ResultStatus.Unauthorized => Result<int>.Unauthorized(errorMessage),
            ResultStatus.Unavailable => Result<int>.Unavailable(errorMessage),
            ResultStatus.Conflict => Result<int>.Conflict(errorMessage),
            ResultStatus.CriticalError => Result<int>.CriticalError(errorMessage),
            _ => throw new ArgumentOutOfRangeException()
        };

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_result = factory.Map(innerResult);

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.Equal(status, asserted_result.Status);
        Assert.Contains(errorMessage, asserted_result.Errors);

        #endregion
    }

    [Fact]
    public void Map_WHEN_ResultHasNoContentStatus_THEN_ReturnsNoContentResult()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var factory = CreateStringResultFactory();
        var innerResult = Result<int>.NoContent();

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_result = factory.Map(innerResult);

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.Equal(ResultStatus.NoContent, asserted_result.Status);

        #endregion
    }
    
    [Fact]
    public void CreateInvalidResult_WHEN_GivenValidationFailures_THEN_ReturnsInvalidResultWithErrors()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var factory = CreateStringResultFactory();
        var failures = new List<ValidationFailure> { new ValidationFailure("Prop", "Error msg") };

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_result = factory.CreateInvalidResult(failures);

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.Equal(ResultStatus.Invalid, asserted_result.Status);
        Assert.Single(asserted_result.ValidationErrors);
        Assert.Equal("Prop", asserted_result.ValidationErrors.First().Identifier);
        Assert.Equal("Error msg", asserted_result.ValidationErrors.First().ErrorMessage);

        #endregion
    }
    
    [Fact]
    public void CreateCriticalErrorResult_WHEN_GivenMessages_THEN_ReturnsCriticalErrorResult()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var factory = CreateStringResultFactory();

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_result = factory.CreateCriticalErrorResult("Critical MSG 1", "Critical MSG 2");

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.Equal(ResultStatus.CriticalError, asserted_result.Status);
        Assert.Contains("Critical MSG 1", asserted_result.Errors);
        Assert.Contains("Critical MSG 2", asserted_result.Errors);

        #endregion
    }

    [Fact]
    public void Map_WHEN_StatusIsOkOrCreated_THEN_ThrowsArgumentOutOfRangeException()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var factory = CreateStringResultFactory();
        var innerResult = Result<int>.Success(1);

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        var act = () => factory.Map(innerResult);

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        var asserted_exception = Assert.Throws<ArgumentOutOfRangeException>(act);
        Assert.Contains("Result must be not Ok or Created", asserted_exception.Message);

        #endregion
    }
    
    [Fact]
    public void FromException_WHEN_ResultExceptionGiven_THEN_ReturnsMappedResult()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var factory = CreateStringResultFactory();
        var innerResult = Result<int>.NotFound("Not found test");
        var resultException = new ResultException(innerResult);

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_result = factory.FromException(resultException);

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.Equal(ResultStatus.NotFound, asserted_result.Status);
        Assert.Contains("Not found test", asserted_result.Errors);

        #endregion
    }
}
