using Microsoft.Extensions.Logging;
using Divergic.Logging.Xunit;
using Xunit.Abstractions;
using Ardalis.Result;
using MediatR;
using NSubstitute;
using Microsoft.Extensions.DependencyInjection;
using InsonusK.Shared.Mediator.CommandContext.Handler;
using InsonusK.Shared.Mediator.CommandContext.Service;
using InsonusK.Shared.Mediator.CommandContext.Interfaces;
using InsonusK.Shared.DataBase.Models;

namespace InsonusK.Shared.Mediator.CommandContext.Test;


[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class CommandWithStringIdHandler_Test : LoggingTestsBase<CommandWithStringIdHandler_Test>
{
    public CommandWithStringIdHandler_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
    {
    }




    [Fact]
    public async Task Handle_WHEN_CalledWithKey_THEN_FillsContextAndCallsNext()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var request = new TestEntityKey1 { EntityStringId = "1" };

        var extractor = Substitute.For<IEntityCommandExtractor<TestEntity1>>();
        extractor.GetAsync(request, Arg.Any<CancellationToken>())
                 .Returns(Task.FromResult(new TestEntity1 { Id = 1 }));

        var services = new ServiceCollection();
        services.AddSingleton(extractor);
        var provider = services.BuildServiceProvider();

        var container = new CommandContextContainer(provider);
        var logger = Substitute.For<ILogger<CommandWithStringIdHandler<TestEntityKey1, Result<string>>>>();

        var handler = new CommandWithStringIdHandler<TestEntityKey1, Result<string>>(logger, container, provider);

        bool nextCalled = false;
        RequestHandlerDelegate<Result<string>> next = (ct) =>
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
}
