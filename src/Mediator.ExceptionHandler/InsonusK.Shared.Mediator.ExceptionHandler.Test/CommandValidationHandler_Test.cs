using Microsoft.Extensions.Logging;
using Divergic.Logging.Xunit;
using Xunit.Abstractions;
using Ardalis.Result;
using MediatR;
using NSubstitute;
using InsonusK.Shared.Mediator.ExceptionHandler.Handler;
using InsonusK.Shared.Mediator.ExceptionHandler.Service;
using InsonusK.Shared.Mediator.ExceptionHandler.Validators;
using InsonusK.Shared.Models.Common;

namespace InsonusK.Shared.Mediator.ExceptionHandler.Test;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class CommandValidationHandler_Test : LoggingTestsBase<CommandValidationHandler_Test>
{
    public CommandValidationHandler_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
    {
    }

    private class TestRequest : IClientActionTimeStamp
    {
        public DateTime ClientActionTimeStamp { get; init; }
    }

    private CommandValidationHandler<TestRequest, Result<string>> CreateHandler()
    {
        var factoryLogger = Substitute.For<ILogger<ArdalisResultReflectionFactory<Result<string>>>>();
        var factory = new ArdalisResultReflectionFactory<Result<string>>(factoryLogger);
        var validator = new CommandValidator();
        return new CommandValidationHandler<TestRequest, Result<string>>(factory, validator);
    }

    [Fact]
    public async Task Handle_WHEN_RequestIsValid_THEN_CallsNext()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var handler = CreateHandler();
        var request = new TestRequest { ClientActionTimeStamp = DateTime.UtcNow };
        bool nextCalled = false;
        RequestHandlerDelegate<Result<string>> next = () =>
        {
            nextCalled = true;
            return Task.FromResult(Result<string>.Success("Success"));
        };

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_result = await handler.Handle(request, next, CancellationToken.None);

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.True(nextCalled);
        Assert.Equal(ResultStatus.Ok, asserted_result.Status);

        #endregion
    }

    [Fact]
    public async Task Handle_WHEN_RequestIsInvalid_THEN_ReturnsInvalidResultWithoutCallingNext()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var handler = CreateHandler();
        var request = new TestRequest { ClientActionTimeStamp = default }; // Invalid, typically default date is invalid
        bool nextCalled = false;
        RequestHandlerDelegate<Result<string>> next = () =>
        {
            nextCalled = true;
            return Task.FromResult(Result<string>.Success("Success"));
        };

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_result = await handler.Handle(request, next, CancellationToken.None);

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.False(nextCalled);
        Assert.Equal(ResultStatus.Invalid, asserted_result.Status);
        Assert.NotEmpty(asserted_result.ValidationErrors);

        #endregion
    }
}
