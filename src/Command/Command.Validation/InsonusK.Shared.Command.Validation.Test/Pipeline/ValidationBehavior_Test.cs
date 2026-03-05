using Divergic.Logging.Xunit;
using FluentValidation;
using FluentValidation.Results;
using InsonusK.Shared.Command.EntityLoading.Interfaces;
using InsonusK.Shared.Command.Interfaces;
using InsonusK.Shared.Command.Validation.Pipeline;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit.Abstractions;
using InsonusK.Shared.Command.EntityLoading.Services;
using Ardalis.Specification;
using InsonusK.Shared.Command.Validation.Test.Extensions;

namespace InsonusK.Shared.Command.Validation.Test.Pipeline;

public class TestCommand : ICommandWithEntityKeys, IBaseRequest
{
    public IReadOnlyCollection<IEntityKey> EntityKeys => Array.Empty<IEntityKey>();
}

public class TestForcableCommand : IForcableValidatableCommand, IBaseRequest
{
    public IReadOnlyCollection<IEntityKey> EntityKeys => Array.Empty<IEntityKey>();
    public bool Force { get; set; }
}

public class TestResponse { }

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class ValidationBehavior_Test : LoggingTestsBase<ValidationBehavior_Test>
{
    public ValidationBehavior_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
    {
    }

    #region Constructor

    [Fact]
    public void Constructor_WHEN_ICommandContextSource_is_registered_THEN_it_is_used()
    {
        Logger.LogDebug("Test ARRAY");
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<ILogger<ValidationBehavior<TestCommand, TestResponse>>>(Output.BuildLoggerFor<ValidationBehavior<TestCommand, TestResponse>>());

        var sourceMock = Substitute.For<ICommandContextSource>();
        serviceCollection.AddSingleton(sourceMock);

        var serviceProvider = serviceCollection.BuildServiceProvider();
        var validators = Enumerable.Empty<IValidator<TestCommand>>();

        Logger.LogDebug("Test ACT");
        var behavior = new ValidationBehavior<TestCommand, TestResponse>(validators, serviceProvider);

        Logger.LogDebug("Test ASSERT");
        Assert.NotNull(behavior);
    }

    [Fact]
    public void Constructor_WHEN_ICommandContextSource_is_not_registered_THEN_EntityProvider_is_used()
    {
        Logger.LogDebug("Test ARRAY");
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<ILogger<ValidationBehavior<TestCommand, TestResponse>>>(Output.BuildLoggerFor<ValidationBehavior<TestCommand, TestResponse>>());

        // Register EntityProvider instead of ICommandContextSource
        var entityProvider = Substitute.For<EntityProvider>(serviceCollection.BuildServiceProvider(), Output.BuildLoggerFor<EntityProvider>());
        serviceCollection.AddSingleton(entityProvider);

        var serviceProvider = serviceCollection.BuildServiceProvider();
        var validators = Enumerable.Empty<IValidator<TestCommand>>();

        Logger.LogDebug("Test ACT");
        var behavior = new ValidationBehavior<TestCommand, TestResponse>(validators, serviceProvider);

        Logger.LogDebug("Test ASSERT");
        Assert.NotNull(behavior);
    }

    #endregion

    #region Handle NoValidators

    [Fact]
    public async Task Handle_WHEN_no_validators_THEN_call_next_without_fetching_context()
    {
        Logger.LogDebug("Test ARRAY");
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<ILogger<ValidationBehavior<TestCommand, TestResponse>>>(Output.BuildLoggerFor<ValidationBehavior<TestCommand, TestResponse>>());
        var sourceMock = Substitute.For<ICommandContextSource>();
        serviceCollection.AddSingleton(sourceMock);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var validators = Enumerable.Empty<IValidator<TestCommand>>();
        var behavior = new ValidationBehavior<TestCommand, TestResponse>(validators, serviceProvider);

        var command = new TestCommand();
        var nextMock = Substitute.For<RequestHandlerDelegate<TestResponse>>();
        nextMock.Invoke().Returns(new TestResponse());

        Logger.LogDebug("Test ACT");
        var result = await behavior.Handle(command, nextMock, CancellationToken.None);

        Logger.LogDebug("Test ASSERT");
        await nextMock.Received(1).Invoke();
        await sourceMock.DidNotReceiveWithAnyArgs().GetForAsync(Arg.Any<TestCommand>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region Handle Execution

    [Fact]
    public async Task Handle_WHEN_validators_present_THEN_context_is_fetched_and_validators_called()
    {
        Logger.LogDebug("Test ARRAY");
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<ILogger<ValidationBehavior<TestCommand, TestResponse>>>(Output.BuildLoggerFor<ValidationBehavior<TestCommand, TestResponse>>());
        var sourceMock = Substitute.For<ICommandContextSource>();
        var cmdCtxMock = Substitute.For<ICommandContext>();
        sourceMock.GetForAsync(Arg.Any<TestCommand>(), Arg.Any<CancellationToken>()).Returns(cmdCtxMock);
        serviceCollection.AddSingleton(sourceMock);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var validatorMock = Substitute.For<IValidator<TestCommand>>();
        validatorMock.ValidateAsync(Arg.Any<IValidationContext>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        var validators = new[] { validatorMock };
        var behavior = new ValidationBehavior<TestCommand, TestResponse>(validators, serviceProvider);

        var command = new TestCommand();
        var nextMock = Substitute.For<RequestHandlerDelegate<TestResponse>>();

        Logger.LogDebug("Test ACT");
        await behavior.Handle(command, nextMock, CancellationToken.None);

        Logger.LogDebug("Test ASSERT");
        await sourceMock.Received(1).GetForAsync(command, CancellationToken.None);
        await validatorMock.Received(1).ValidateAsync(Arg.Any<IValidationContext>(), CancellationToken.None);
        await nextMock.Received(1).Invoke();
    }

    #endregion

    #region Handle Errors

    [Fact]
    public async Task Handle_WHEN_validator_returns_error_THEN_ValidationException_is_thrown()
    {
        Logger.LogDebug("Test ARRAY");
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<ILogger<ValidationBehavior<TestCommand, TestResponse>>>(Output.BuildLoggerFor<ValidationBehavior<TestCommand, TestResponse>>());
        var sourceMock = Substitute.For<ICommandContextSource>();
        serviceCollection.AddSingleton(sourceMock);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var validatorMock = Substitute.For<IValidator<TestCommand>>();
        var validationFailure = new ValidationFailure("Prop", "Error") { Severity = Severity.Error };
        validatorMock.ValidateAsync(Arg.Any<IValidationContext>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(new[] { validationFailure }));

        var validators = new[] { validatorMock };
        var behavior = new ValidationBehavior<TestCommand, TestResponse>(validators, serviceProvider);
        var command = new TestCommand();
        var nextMock = Substitute.For<RequestHandlerDelegate<TestResponse>>();

        Logger.LogDebug("Test ACT");
        var act = async () => await behavior.Handle(command, nextMock, CancellationToken.None);

        Logger.LogDebug("Test ASSERT");
        await Assert.ThrowsAsync<ValidationException>(act);
        await nextMock.DidNotReceive().Invoke();
    }

    #endregion

    #region Handle Warnings

    [Fact]
    public async Task Handle_WHEN_validator_returns_warning_AND_command_is_not_IForcableValidatableCommand_THEN_ValidationException_is_thrown()
    {
        Logger.LogDebug("Test ARRAY");
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<ILogger<ValidationBehavior<TestCommand, TestResponse>>>(Output.BuildLoggerFor<ValidationBehavior<TestCommand, TestResponse>>());
        var sourceMock = Substitute.For<ICommandContextSource>();
        serviceCollection.AddSingleton(sourceMock);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var validatorMock = Substitute.For<IValidator<TestCommand>>();
        var validationFailure = new ValidationFailure("Prop", "Warning") { Severity = Severity.Warning };
        validatorMock.ValidateAsync(Arg.Any<IValidationContext>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(new[] { validationFailure }));

        var validators = new[] { validatorMock };
        var behavior = new ValidationBehavior<TestCommand, TestResponse>(validators, serviceProvider);
        var command = new TestCommand();
        var nextMock = Substitute.For<RequestHandlerDelegate<TestResponse>>();

        Logger.LogDebug("Test ACT");
        var act = async () => await behavior.Handle(command, nextMock, CancellationToken.None);

        Logger.LogDebug("Test ASSERT");
        await Assert.ThrowsAsync<ValidationException>(act);
        await nextMock.DidNotReceive().Invoke();
    }

    [Fact]
    public async Task Handle_WHEN_validator_returns_warning_AND_command_is_IForcableValidatableCommand_with_Force_false_THEN_ValidationException_is_thrown()
    {
        Logger.LogDebug("Test ARRAY");
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<ILogger<ValidationBehavior<TestForcableCommand, TestResponse>>>(Output.BuildLoggerFor<ValidationBehavior<TestForcableCommand, TestResponse>>());
        var sourceMock = Substitute.For<ICommandContextSource>();
        serviceCollection.AddSingleton(sourceMock);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var validatorMock = Substitute.For<IValidator<TestForcableCommand>>();
        var validationFailure = new ValidationFailure("Prop", "Warning") { Severity = Severity.Warning };
        validatorMock.ValidateAsync(Arg.Any<IValidationContext>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(new[] { validationFailure }));

        var validators = new[] { validatorMock };
        var behavior = new ValidationBehavior<TestForcableCommand, TestResponse>(validators, serviceProvider);
        var command = new TestForcableCommand { Force = false };
        var nextMock = Substitute.For<RequestHandlerDelegate<TestResponse>>();

        Logger.LogDebug("Test ACT");
        var act = async () => await behavior.Handle(command, nextMock, CancellationToken.None);

        Logger.LogDebug("Test ASSERT");
        await Assert.ThrowsAsync<ValidationException>(act);
        await nextMock.DidNotReceive().Invoke();
    }

    [Fact]
    public async Task Handle_WHEN_validator_returns_warning_AND_command_is_IForcableValidatableCommand_with_Force_true_THEN_call_next()
    {
        Logger.LogDebug("Test ARRAY");
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<ILogger<ValidationBehavior<TestForcableCommand, TestResponse>>>(Output.BuildLoggerFor<ValidationBehavior<TestForcableCommand, TestResponse>>());
        var sourceMock = Substitute.For<ICommandContextSource>();
        serviceCollection.AddSingleton(sourceMock);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var validatorMock = Substitute.For<IValidator<TestForcableCommand>>();
        var validationFailure = new ValidationFailure("Prop", "Warning") { Severity = Severity.Warning };
        validatorMock.ValidateAsync(Arg.Any<IValidationContext>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(new[] { validationFailure }));

        var validators = new[] { validatorMock };
        var behavior = new ValidationBehavior<TestForcableCommand, TestResponse>(validators, serviceProvider);
        var command = new TestForcableCommand { Force = true };
        var nextMock = Substitute.For<RequestHandlerDelegate<TestResponse>>();

        Logger.LogDebug("Test ACT");
        await behavior.Handle(command, nextMock, CancellationToken.None);

        Logger.LogDebug("Test ASSERT");
        await nextMock.Received(1).Invoke();
    }

    [Fact]
    public async Task Handle_WHEN_validator_returns_warning_AND_error_THEN_ValidationException_is_thrown()
    {
        Logger.LogDebug("Test ARRAY");
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<ILogger<ValidationBehavior<TestForcableCommand, TestResponse>>>(Output.BuildLoggerFor<ValidationBehavior<TestForcableCommand, TestResponse>>());
        var sourceMock = Substitute.For<ICommandContextSource>();
        serviceCollection.AddSingleton(sourceMock);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var validatorMock = Substitute.For<IValidator<TestForcableCommand>>();
        var validationFailure1 = new ValidationFailure("Prop", "Warning") { Severity = Severity.Warning };
        var validationFailure2 = new ValidationFailure("Prop", "Error") { Severity = Severity.Error };
        validatorMock.ValidateAsync(Arg.Any<IValidationContext>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(new[] { validationFailure1, validationFailure2 }));

        var validators = new[] { validatorMock };
        var behavior = new ValidationBehavior<TestForcableCommand, TestResponse>(validators, serviceProvider);
        var command = new TestForcableCommand { Force = true }; // Force is true, but there is still an Error severity
        var nextMock = Substitute.For<RequestHandlerDelegate<TestResponse>>();

        Logger.LogDebug("Test ACT");
        var act = async () => await behavior.Handle(command, nextMock, CancellationToken.None);

        Logger.LogDebug("Test ASSERT");
        await Assert.ThrowsAsync<ValidationException>(act);
        await nextMock.DidNotReceive().Invoke();
    }

    #endregion
}
