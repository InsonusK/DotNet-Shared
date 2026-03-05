using Microsoft.Extensions.Logging;
using Divergic.Logging.Xunit;
using Xunit.Abstractions;
using Ardalis.Result;
using MediatR;
using NSubstitute;
using FluentValidation;
using FluentValidation.Results;
using InsonusK.Shared.Mediator.ExceptionHandler.Handler;
using InsonusK.Shared.Mediator.ExceptionHandler.Service;

namespace InsonusK.Shared.Mediator.ExceptionHandler.Test;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class ExceptionHandler_Test : LoggingTestsBase<ExceptionHandler_Test>
{
    public ExceptionHandler_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
    {
    }

    private ExceptionHandler<string, Result<string>> CreateHandler()
    {
        var factoryLogger = Substitute.For<ILogger<ArdalisResultReflectionFactory<Result<string>>>>();
        var factory = new ArdalisResultReflectionFactory<Result<string>>(factoryLogger);
        var handlerLogger = Substitute.For<ILogger<ExceptionHandler<string, Result<string>>>>();
        return new ExceptionHandler<string, Result<string>>(factory, handlerLogger);
    }

    [Fact]
    public async Task Handle_WHEN_NextReturnsSuccessfully_THEN_ReturnsResult()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var handler = CreateHandler();
        RequestHandlerDelegate<Result<string>> next = (ct) => Task.FromResult(Result<string>.Success("Success"));

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_result = await handler.Handle("Request", next, CancellationToken.None);

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.Equal(ResultStatus.Ok, asserted_result.Status);
        Assert.Equal("Success", asserted_result.Value);

        #endregion
    }

    [Fact]
    public async Task Handle_WHEN_ResultExceptionThrown_THEN_ReturnsMappedResult()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var handler = CreateHandler();
        var innerResult = Result.NotFound("Item not found");
        var exception = new ResultException(innerResult);
        RequestHandlerDelegate<Result<string>> next = (ct) => throw exception;

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_result = await handler.Handle("Request", next, CancellationToken.None);

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.Equal(ResultStatus.NotFound, asserted_result.Status);
        Assert.Contains("Item not found", asserted_result.Errors);

        #endregion
    }

    [Fact]
    public async Task Handle_WHEN_ValidationExceptionThrown_THEN_ReturnsInvalidResult()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var handler = CreateHandler();
        var validationFailures = new List<ValidationFailure> { new ValidationFailure("Prop", "Error msg") };
        var exception = new ValidationException(validationFailures);
        RequestHandlerDelegate<Result<string>> next = (ct) => throw exception;

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_result = await handler.Handle("Request", next, CancellationToken.None);

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.Equal(ResultStatus.Invalid, asserted_result.Status);
        Assert.Single(asserted_result.ValidationErrors);
        Assert.Equal("Prop", asserted_result.ValidationErrors.ElementAt(0).Identifier);

        #endregion
    }

    [Fact]
    public async Task Handle_WHEN_InvalidOperationExceptionThrown_THEN_ReturnsCriticalErrorResult()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var handler = CreateHandler();
        var exception = new InvalidOperationException("Some invalid op");
        RequestHandlerDelegate<Result<string>> next = (ct) => throw exception;

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_result = await handler.Handle("Request", next, CancellationToken.None);

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.Equal(ResultStatus.CriticalError, asserted_result.Status);
        Assert.Contains("Get unexpected error please contact support", asserted_result.Errors);

        #endregion
    }

    [Fact]
    public async Task Handle_WHEN_UnhandledExceptionThrown_THEN_ReturnsCriticalErrorResult()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var handler = CreateHandler();
        var exception = new Exception("General exception");
        RequestHandlerDelegate<Result<string>> next = (ct) => throw exception;

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_result = await handler.Handle("Request", next, CancellationToken.None);

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.Equal(ResultStatus.CriticalError, asserted_result.Status);
        Assert.Contains("Get unexpected error please contact support", asserted_result.Errors);

        #endregion
    }
}
