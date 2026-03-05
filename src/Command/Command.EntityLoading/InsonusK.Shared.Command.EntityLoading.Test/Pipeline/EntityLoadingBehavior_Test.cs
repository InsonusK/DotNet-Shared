using Divergic.Logging.Xunit;
using InsonusK.Shared.Command.EntityLoading.Pipeline;
using InsonusK.Shared.Command.EntityLoading.Services;
using InsonusK.Shared.Command.Interfaces;
using InsonusK.Shared.Command.Interfaces.Models;
using InsonusK.Shared.DataBase.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit.Abstractions;
using Ardalis.Specification;

namespace InsonusK.Shared.Command.EntityLoading.Test.Pipeline;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class EntityLoadingBehavior_Test : LoggingTestsBase<EntityLoadingBehavior_Test>
{
    public EntityLoadingBehavior_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
    {
    }

    private class TestResponse
    {
    }

    private class TestCommand : ICommandWithEntityKeys, IRequest<TestResponse>
    {
        public IEnumerable<IEntityKey> EntityKeys { get; set; } = Array.Empty<IEntityKey>();
    }

    private class TestEntity : EntityBase
    {
    }

    private class TestEntityKey : IEntityKey
    {
        public Type EntityType { get; set; } = typeof(object);
        public string StringId { get; set; } = string.Empty;
    }

    /// <summary>
    /// description: Call pipeline handle
    /// input: ICommandWithEntityKeys command, RequestHandlerDelegate next
    /// output: TResponse
    /// expected_result: Provider resolves entities, Manager starts/ends tracking, next() is called
    /// </summary>
    [Fact]
    public async Task test_Handle_Generic_WHEN_called_THEN_start_context_call_next_end_context()
    {
        #region Arrange
        Logger.LogDebug("Test ARRANGE");
        var commandContextManager = new CommandContextManager();
        var serviceProvider = Substitute.For<IServiceProvider>();
        
        var repo = Substitute.For<IReadRepositoryBase<TestEntity>>();
        serviceProvider.GetService(typeof(IReadRepositoryBase<TestEntity>)).Returns(repo);
        repo.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(new TestEntity { Id = 1 });

        var entityProvider = new EntityProvider(serviceProvider, Logger);

        var behavior = new EntityLoadingBehavior<TestCommand, TestResponse>(Logger, commandContextManager, entityProvider);

        var command = new TestCommand
        {
            EntityKeys = new List<IEntityKey>
            {
                new TestEntityKey { EntityType = typeof(TestEntity), StringId = "1" }
            }
        };

        var expectedResponse = new TestResponse();
        var nextDelegate = Substitute.For<RequestHandlerDelegate<TestResponse>>();
        nextDelegate.Invoke().Returns(Task.FromResult(expectedResponse));
        
        var ct = CancellationToken.None;
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        var response = await behavior.Handle(command, nextDelegate, ct);
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        Assert.Equal(expectedResponse, response);
        await nextDelegate.Received(1).Invoke();

        // Check that Context no longer managed after EndFor
        var endForException = Record.Exception(() => commandContextManager.EndFor(command));
        Assert.NotNull(endForException);
        Assert.IsType<InvalidOperationException>(endForException);
        #endregion
    }

    /// <summary>
    /// description: Call pipeline handle where next() throws
    /// input: ICommandWithEntityKeys command, RequestHandlerDelegate next throwing exception
    /// output: exception
    /// expected_result: Manager ends tracking despite the exception
    /// </summary>
    [Fact]
    public async Task test_Handle_Generic_WHEN_exception_in_next_THEN_end_context_still_called()
    {
        #region Arrange
        Logger.LogDebug("Test ARRANGE");
        var commandContextManager = new CommandContextManager();
        var serviceProvider = Substitute.For<IServiceProvider>();
        
        var entityProvider = new EntityProvider(serviceProvider, Logger);

        var behavior = new EntityLoadingBehavior<TestCommand, TestResponse>(Logger, commandContextManager, entityProvider);

        var command = new TestCommand(); // empty keys, so provider won't fail

        var expectedException = new InvalidOperationException("Test inner exception");
        var nextDelegate = Substitute.For<RequestHandlerDelegate<TestResponse>>();
        nextDelegate.Invoke().Returns(Task.FromException<TestResponse>(expectedException));
        
        var ct = CancellationToken.None;
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        var exception = await Record.ExceptionAsync(() => behavior.Handle(command, nextDelegate, ct));
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        Assert.NotNull(exception);
        Assert.Same(expectedException, exception);

        // Check that Context no longer managed after EndFor happens inside finally block
        var endForException = Record.Exception(() => commandContextManager.EndFor(command));
        Assert.NotNull(endForException);
        Assert.IsType<InvalidOperationException>(endForException);
        #endregion
    }
}
