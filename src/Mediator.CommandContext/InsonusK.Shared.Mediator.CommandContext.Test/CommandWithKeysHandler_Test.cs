using Microsoft.Extensions.Logging;
using Divergic.Logging.Xunit;
using Xunit.Abstractions;
using Ardalis.Result;
using MediatR;
using NSubstitute;
using Microsoft.Extensions.DependencyInjection;
using InsonusK.Shared.Mediator.CommandContext.Handler;
using InsonusK.Shared.Mediator.CommandContext.Service;
using InsonusK.Shared.Mediator.CommandContext.Command;
using InsonusK.Shared.Mediator.CommandContext.Interfaces;
using InsonusK.Shared.DataBase.Models;

namespace InsonusK.Shared.Mediator.CommandContext.Test;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class CommandWithKeysHandler_Test : LoggingTestsBase<CommandWithKeysHandler_Test>
{
    public CommandWithKeysHandler_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
    {
    }

    private class TestEntity : EntityBase { }

    private class TestEntityKey : IEntityKey
    {
        public string EntityStringId { get; init; } = "1";
        public Type EntityType => typeof(TestEntity);
    }

    private class TestCommand : CommandWithEntityKeys
    {
        public override IEnumerable<IEntityKey> EntityKeys { get; init; } = new List<IEntityKey>();
    }

    [Fact]
    public async Task Handle_WHEN_CalledWithKeys_THEN_FillsContextAndCallsNext()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var key1 = new TestEntityKey { EntityStringId = "1" };
        var key2 = new TestEntityKey { EntityStringId = "2" };
        var request = new TestCommand { EntityKeys = new[] { key1, key2 } };

        var extractor = Substitute.For<IEntityCommandExtractor<TestEntity>>();
        extractor.GetAsync(Arg.Any<IEntityKey>(), Arg.Any<CancellationToken>())
                 .Returns(Task.FromResult(new TestEntity { Id = 1 }));

        var services = new ServiceCollection();
        services.AddSingleton(extractor);
        var provider = services.BuildServiceProvider();

        var container = new CommandContextContainer(provider);
        var logger = Substitute.For<ILogger<CommandWithKeysHandler<TestCommand, Result<string>>>>();
        
        var handler = new CommandWithKeysHandler<TestCommand, Result<string>>(logger, container, provider);

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
