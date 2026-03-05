using Microsoft.Extensions.Logging;
using Divergic.Logging.Xunit;
using Xunit.Abstractions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using InsonusK.Shared.Command.Validation.Pipeline;
using InsonusK.Shared.Command.Validation.Interfaces;
using InsonusK.Shared.Command.Validation.Extensions;
using InsonusK.Shared.Command.Validation.Tools;
using InsonusK.Shared.DataBase.Models;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using Xunit;
using Ardalis.Specification;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;
using InsonusK.Shared.Models.Common;

namespace InsonusK.Shared.Command.Validation.Test.Pipeline;

public class TestEntity : EntityBase
{
}

public class TestGuidEntity : EntityBase, IGuidModel
{
    public Guid Guid { get; set; }
}

public class TestValidatableCommand : IValidatableCommand, IRequest<bool>
{
    public Dictionary<Type, string> ModifiableEntityKeys { get; } = new();
    public IReadOnlyDictionary<Type, string> EntityKeys => ModifiableEntityKeys;
}

public class TestStandardCommand : IRequest<bool>
{
}

public class TestForcableCommand : IValidatableCommand, IForcableValidatableCommand, IRequest<bool>
{
    public Dictionary<Type, string> ModifiableEntityKeys { get; } = new();
    public IReadOnlyDictionary<Type, string> EntityKeys => ModifiableEntityKeys;
    public bool Force { get; set; }
}

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class ValidationBehavior_Test : LoggingTestsBase<ValidationBehavior_Test>
{
    public ValidationBehavior_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
    {
    }

    /// <summary>
    /// description: Validates that commands implementing IValidatableCommand trigger the validator
    /// input: Command implementing IValidatableCommand
    /// output: Execution continues to next delegate
    /// expected_result: Validator's ValidateAsync is called once
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task test_CommandValidation_WHEN_CommandHasIValidatableCommand_THEN_RunsValidation()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var services = new ServiceCollection();
        services.AddSingleton<ILogger<ValidationBehavior<TestValidatableCommand, bool>>>(
            sp => Output.BuildLoggerFor<ValidationBehavior<TestValidatableCommand, bool>>()
        );

        var mockValidator = Substitute.For<IValidator<TestValidatableCommand>>();
        mockValidator.ValidateAsync(Arg.Any<ValidationContext<TestValidatableCommand>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new ValidationResult()));
        services.AddSingleton(mockValidator);

        var serviceProvider = services.BuildServiceProvider();
        var behavior = new ValidationBehavior<TestValidatableCommand, bool>(new[] { mockValidator }, serviceProvider);

        var request = new TestValidatableCommand();
        RequestHandlerDelegate<bool> next = (ct) => Task.FromResult(true);

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_result = await behavior.Handle(request, next, CancellationToken.None);

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.True(asserted_result);
        await mockValidator.Received(1).ValidateAsync(Arg.Any<ValidationContext<TestValidatableCommand>>(), Arg.Any<CancellationToken>());

        #endregion
    }

    /// <summary>
    /// description: Validates that standard commands do not trigger the validation behavior
    /// input: Command without IValidatableCommand
    /// output: Execution continues to next delegate
    /// expected_result: Validation behavior is not resolved in pipeline (since the service collection filter avoids it or it skips logic)
    /// </summary>
    /// <returns></returns>
    [Fact]
    public void test_CommandValidation_WHEN_CommandWithoutIValidatableCommand_THEN_DoesNotRunValidation()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var services = new ServiceCollection();
        services.AddCommandValidation();
        var serviceProvider = services.BuildServiceProvider();

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        var behaviors = serviceProvider.GetServices<IPipelineBehavior<TestStandardCommand, bool>>();

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.Empty(behaviors);

        #endregion
    }

    /// <summary>
    /// description: Verifies that an int-id based entity is loaded from repo and added to context
    /// input: Valid entity type and string represented integer ID
    /// output: Entity is resolved from DB
    /// expected_result: Context contains the loaded entity
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task test_EntityResolution_IntId_WHEN_EntityKeyExistsAndFound_THEN_AddsToContext()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var services = new ServiceCollection();
        services.AddSingleton<ILogger<ValidationBehavior<TestValidatableCommand, bool>>>(
            sp => Output.BuildLoggerFor<ValidationBehavior<TestValidatableCommand, bool>>()
        );

        var mockValidator = Substitute.For<IValidator<TestValidatableCommand>>();
        ValidationContext<TestValidatableCommand>? capturedContext = null;
        mockValidator.ValidateAsync(Arg.Do<ValidationContext<TestValidatableCommand>>(ctx => capturedContext = ctx), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new ValidationResult()));
        services.AddSingleton(mockValidator);

        var mockRepo = Substitute.For<IReadRepositoryBase<TestEntity>>();
        var entityInstance = new TestEntity { Id = 123 };
        mockRepo.GetByIdAsync(Arg.Is<int>(123), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<TestEntity?>(entityInstance));
        services.AddSingleton(mockRepo);

        var serviceProvider = services.BuildServiceProvider();
        var behavior = new ValidationBehavior<TestValidatableCommand, bool>(new[] { mockValidator }, serviceProvider);

        var request = new TestValidatableCommand();
        request.ModifiableEntityKeys.Add(typeof(TestEntity), "123");

        RequestHandlerDelegate<bool> next = (ct) => Task.FromResult(true);

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_result = await behavior.Handle(request, next, CancellationToken.None);

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.True(asserted_result);
        Assert.NotNull(capturedContext);
        var entitiesContext = capturedContext.GetEntitiesContext();
        Assert.True(entitiesContext.HasEntity<TestEntity>());
        Assert.Equal(entityInstance, entitiesContext.GetEntity<TestEntity>());

        #endregion
    }

    /// <summary>
    /// description: Verifies that an exception is thrown when an int-id based entity does not exist
    /// input: Valid entity type and string represented integer ID, but repo returns null
    /// output: ValidationException thrown
    /// expected_result: ValidationException contains "not found" message
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task test_EntityResolution_IntId_WHEN_EntityKeyExistsAndNotFound_THEN_ThrowsValidationException()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var services = new ServiceCollection();
        services.AddSingleton<ILogger<ValidationBehavior<TestValidatableCommand, bool>>>(
            sp => Output.BuildLoggerFor<ValidationBehavior<TestValidatableCommand, bool>>()
        );

        var mockValidator = Substitute.For<IValidator<TestValidatableCommand>>();
        mockValidator.ValidateAsync(Arg.Any<ValidationContext<TestValidatableCommand>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new ValidationResult()));
        services.AddSingleton(mockValidator);

        var mockRepo = Substitute.For<IReadRepositoryBase<TestEntity>>();
        mockRepo.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<TestEntity?>(null));
        services.AddSingleton(mockRepo);

        var serviceProvider = services.BuildServiceProvider();
        var behavior = new ValidationBehavior<TestValidatableCommand, bool>(new[] { mockValidator }, serviceProvider);

        var request = new TestValidatableCommand();
        request.ModifiableEntityKeys.Add(typeof(TestEntity), "999");

        RequestHandlerDelegate<bool> next = (ct) => Task.FromResult(true);

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        // Act is combined with Assert below

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        var ex = await Assert.ThrowsAsync<ValidationException>(() => behavior.Handle(request, next, CancellationToken.None));
        Assert.Contains("Entity of type TestEntity with id 999 not found", ex.Message);

        #endregion
    }

    /// <summary>
    /// description: Verifies that an IGuidModel entity is queried using ByStringIdSpec and added to context
    /// input: Valid IGuidModel entity type and string represented GUID
    /// output: Entity is resolved using SingleOrDefaultAsync and Spec
    /// expected_result: Context contains the loaded IGuidModel entity
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task test_EntityResolution_Guid_WHEN_EntityIsIGuidModelAndFound_THEN_AddsToContext()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var services = new ServiceCollection();
        services.AddSingleton<ILogger<ValidationBehavior<TestValidatableCommand, bool>>>(
            sp => Output.BuildLoggerFor<ValidationBehavior<TestValidatableCommand, bool>>()
        );

        var mockValidator = Substitute.For<IValidator<TestValidatableCommand>>();
        ValidationContext<TestValidatableCommand>? capturedContext = null;
        mockValidator.ValidateAsync(Arg.Do<ValidationContext<TestValidatableCommand>>(ctx => capturedContext = ctx), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new ValidationResult()));
        services.AddSingleton(mockValidator);

        var expectedGuid = Guid.NewGuid();
        var strGuid = expectedGuid.ToString();

        var mockRepo = Substitute.For<IReadRepositoryBase<TestGuidEntity>>();
        var entityInstance = new TestGuidEntity { Id = 123, Guid = expectedGuid };
        mockRepo.SingleOrDefaultAsync(Arg.Any<ISingleResultSpecification<TestGuidEntity>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<TestGuidEntity?>(entityInstance));
        services.AddSingleton(mockRepo);

        var serviceProvider = services.BuildServiceProvider();
        var behavior = new ValidationBehavior<TestValidatableCommand, bool>(new[] { mockValidator }, serviceProvider);

        var request = new TestValidatableCommand();
        request.ModifiableEntityKeys.Add(typeof(TestGuidEntity), strGuid);

        RequestHandlerDelegate<bool> next = (ct) => Task.FromResult(true);

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_result = await behavior.Handle(request, next, CancellationToken.None);

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.True(asserted_result);
        Assert.NotNull(capturedContext);
        var entitiesContext = capturedContext.GetEntitiesContext();
        Assert.True(entitiesContext.HasEntity<TestGuidEntity>());
        Assert.Equal(entityInstance, entitiesContext.GetEntity<TestGuidEntity>());

        #endregion
    }

    /// <summary>
    /// description: Verifies that an exception is thrown when an IGuidModel entity does not exist
    /// input: Valid IGuidModel entity type and string represented GUID, but repo returns null
    /// output: ValidationException thrown
    /// expected_result: ValidationException contains "not found" message
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task test_EntityResolution_Guid_WHEN_EntityIsIGuidModelAndNotFound_THEN_ThrowsValidationException()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var services = new ServiceCollection();
        services.AddSingleton<ILogger<ValidationBehavior<TestValidatableCommand, bool>>>(
            sp => Output.BuildLoggerFor<ValidationBehavior<TestValidatableCommand, bool>>()
        );

        var mockValidator = Substitute.For<IValidator<TestValidatableCommand>>();
        mockValidator.ValidateAsync(Arg.Any<ValidationContext<TestValidatableCommand>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new ValidationResult()));
        services.AddSingleton(mockValidator);

        var mockRepo = Substitute.For<IReadRepositoryBase<TestGuidEntity>>();
        mockRepo.SingleOrDefaultAsync(Arg.Any<ISingleResultSpecification<TestGuidEntity>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<TestGuidEntity?>(null));
        services.AddSingleton(mockRepo);

        var serviceProvider = services.BuildServiceProvider();
        var behavior = new ValidationBehavior<TestValidatableCommand, bool>(new[] { mockValidator }, serviceProvider);

        var strGuid = Guid.NewGuid().ToString();
        var request = new TestValidatableCommand();
        request.ModifiableEntityKeys.Add(typeof(TestGuidEntity), strGuid);

        RequestHandlerDelegate<bool> next = (ct) => Task.FromResult(true);

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        // Act is combined with Assert below

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        var ex = await Assert.ThrowsAsync<ValidationException>(() => behavior.Handle(request, next, CancellationToken.None));
        Assert.Contains($"Entity of type TestGuidEntity with id {strGuid} not found", ex.Message);

        #endregion
    }

    /// <summary>
    /// description: Verifies that an IGuidModel entity is queried using ByStringIdSpec and added to context using string represented int ID
    /// input: Valid IGuidModel entity type and string represented integer ID
    /// output: Entity is resolved using SingleOrDefaultAsync and Spec
    /// expected_result: Context contains the loaded IGuidModel entity
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task test_EntityResolution_Guid_WHEN_EntityIsIGuidModelAndIntIdFound_THEN_AddsToContext()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var services = new ServiceCollection();
        services.AddSingleton<ILogger<ValidationBehavior<TestValidatableCommand, bool>>>(
            sp => Output.BuildLoggerFor<ValidationBehavior<TestValidatableCommand, bool>>()
        );

        var mockValidator = Substitute.For<IValidator<TestValidatableCommand>>();
        ValidationContext<TestValidatableCommand>? capturedContext = null;
        mockValidator.ValidateAsync(Arg.Do<ValidationContext<TestValidatableCommand>>(ctx => capturedContext = ctx), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new ValidationResult()));
        services.AddSingleton(mockValidator);

        var mockRepo = Substitute.For<IReadRepositoryBase<TestGuidEntity>>();
        var entityInstance = new TestGuidEntity { Id = 123, Guid = Guid.NewGuid() };
        mockRepo.SingleOrDefaultAsync(Arg.Any<ISingleResultSpecification<TestGuidEntity>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<TestGuidEntity?>(entityInstance));
        services.AddSingleton(mockRepo);

        var serviceProvider = services.BuildServiceProvider();
        var behavior = new ValidationBehavior<TestValidatableCommand, bool>(new[] { mockValidator }, serviceProvider);

        var request = new TestValidatableCommand();
        request.ModifiableEntityKeys.Add(typeof(TestGuidEntity), "123");

        RequestHandlerDelegate<bool> next = (ct) => Task.FromResult(true);

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_result = await behavior.Handle(request, next, CancellationToken.None);

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.True(asserted_result);
        Assert.NotNull(capturedContext);
        var entitiesContext = capturedContext.GetEntitiesContext();
        Assert.True(entitiesContext.HasEntity<TestGuidEntity>());
        Assert.Equal(entityInstance, entitiesContext.GetEntity<TestGuidEntity>());

        #endregion
    }

    /// <summary>
    /// description: Verifies that an exception is thrown when an IGuidModel entity does not exist using string represented int ID
    /// input: Valid IGuidModel entity type and string represented integer ID, but repo returns null
    /// output: ValidationException thrown
    /// expected_result: ValidationException contains "not found" message
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task test_EntityResolution_Guid_WHEN_EntityIsIGuidModelAndIntIdNotFound_THEN_ThrowsValidationException()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var services = new ServiceCollection();
        services.AddSingleton<ILogger<ValidationBehavior<TestValidatableCommand, bool>>>(
            sp => Output.BuildLoggerFor<ValidationBehavior<TestValidatableCommand, bool>>()
        );

        var mockValidator = Substitute.For<IValidator<TestValidatableCommand>>();
        mockValidator.ValidateAsync(Arg.Any<ValidationContext<TestValidatableCommand>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new ValidationResult()));
        services.AddSingleton(mockValidator);

        var mockRepo = Substitute.For<IReadRepositoryBase<TestGuidEntity>>();
        mockRepo.SingleOrDefaultAsync(Arg.Any<ISingleResultSpecification<TestGuidEntity>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<TestGuidEntity?>(null));
        services.AddSingleton(mockRepo);

        var serviceProvider = services.BuildServiceProvider();
        var behavior = new ValidationBehavior<TestValidatableCommand, bool>(new[] { mockValidator }, serviceProvider);

        var request = new TestValidatableCommand();
        request.ModifiableEntityKeys.Add(typeof(TestGuidEntity), "123");

        RequestHandlerDelegate<bool> next = (ct) => Task.FromResult(true);

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        // Act is combined with Assert below

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        var ex = await Assert.ThrowsAsync<ValidationException>(() => behavior.Handle(request, next, CancellationToken.None));
        Assert.Contains($"Entity of type TestGuidEntity with id 123 not found", ex.Message);

        #endregion
    }

    /// <summary>
    /// description: Verifies throwing on severity Error
    /// input: Validators return errors (Severity.Error)
    /// output: ValidationException thrown
    /// expected_result: ValidationException contains error messages
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task test_ValidationResult_WHEN_ValidationErrors_THEN_ThrowsValidationException()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var services = new ServiceCollection();
        services.AddSingleton<ILogger<ValidationBehavior<TestValidatableCommand, bool>>>(
            sp => Output.BuildLoggerFor<ValidationBehavior<TestValidatableCommand, bool>>()
        );

        var mockValidator = Substitute.For<IValidator<TestValidatableCommand>>();
        mockValidator.ValidateAsync(Arg.Any<ValidationContext<TestValidatableCommand>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new ValidationResult(new[] { new ValidationFailure("Prop", "ErrorMsg") { Severity = Severity.Error } })));
        services.AddSingleton(mockValidator);

        var serviceProvider = services.BuildServiceProvider();
        var behavior = new ValidationBehavior<TestValidatableCommand, bool>(new[] { mockValidator }, serviceProvider);

        var request = new TestValidatableCommand();
        RequestHandlerDelegate<bool> next = (ct) => Task.FromResult(true);

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        // Act is combined with Assert below

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        var ex = await Assert.ThrowsAsync<ValidationException>(() => behavior.Handle(request, next, CancellationToken.None));
        Assert.Contains("ErrorMsg", ex.Errors.First().ErrorMessage);

        #endregion
    }

    /// <summary>
    /// description: Verifies that warnings do not throw if the command is not IForcableValidatableCommand
    /// input: Validators return warnings, command is normal IValidatableCommand
    /// output: Execution continues
    /// expected_result: Next delegate is called successfully
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task test_ValidationResult_WHEN_ValidationWarningsAndNotForcable_THEN_DoesNotThrow()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var services = new ServiceCollection();
        services.AddSingleton<ILogger<ValidationBehavior<TestValidatableCommand, bool>>>(
            sp => Output.BuildLoggerFor<ValidationBehavior<TestValidatableCommand, bool>>()
        );

        var mockValidator = Substitute.For<IValidator<TestValidatableCommand>>();
        mockValidator.ValidateAsync(Arg.Any<ValidationContext<TestValidatableCommand>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new ValidationResult(new[] { new ValidationFailure("Prop", "WarnMsg") { Severity = Severity.Warning } })));
        services.AddSingleton(mockValidator);

        var serviceProvider = services.BuildServiceProvider();
        var behavior = new ValidationBehavior<TestValidatableCommand, bool>(new[] { mockValidator }, serviceProvider);

        var request = new TestValidatableCommand();
        RequestHandlerDelegate<bool> next = (ct) => Task.FromResult(true);

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_result = await behavior.Handle(request, next, CancellationToken.None);

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.True(asserted_result); // Execution continues since command is not IForcableValidatableCommand

        #endregion
    }

    /// <summary>
    /// description: Verifies warning behavior on unforced forcable command
    /// input: Validators return warnings, command is IForcableValidatableCommand with Force=false
    /// output: ValidationException thrown
    /// expected_result: ValidationException contains warning messages
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task test_ValidationResult_WHEN_ValidationWarningsAndForcableUnforced_THEN_ThrowsValidationException()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var services = new ServiceCollection();
        services.AddSingleton<ILogger<ValidationBehavior<TestForcableCommand, bool>>>(
            sp => Output.BuildLoggerFor<ValidationBehavior<TestForcableCommand, bool>>()
        );

        var mockValidator = Substitute.For<IValidator<TestForcableCommand>>();
        mockValidator.ValidateAsync(Arg.Any<ValidationContext<TestForcableCommand>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new ValidationResult(new[] { new ValidationFailure("Prop", "WarnMsgMsg") { Severity = Severity.Warning } })));
        services.AddSingleton(mockValidator);

        var serviceProvider = services.BuildServiceProvider();
        var behavior = new ValidationBehavior<TestForcableCommand, bool>(new[] { mockValidator }, serviceProvider);

        var request = new TestForcableCommand { Force = false };
        RequestHandlerDelegate<bool> next = (ct) => Task.FromResult(true);

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        // Act is combined with Assert below

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        var ex = await Assert.ThrowsAsync<ValidationException>(() => behavior.Handle(request, next, CancellationToken.None));
        Assert.Contains("WarnMsgMsg", ex.Errors.First().ErrorMessage);

        #endregion
    }

    /// <summary>
    /// description: Verifies that forcing bypasses warnings
    /// input: Validators return warnings, command is IForcableValidatableCommand with Force=true
    /// output: Execution continues
    /// expected_result: Next delegate is called successfully
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task test_ValidationResult_WHEN_ValidationWarningsAndForcableForced_THEN_DoesNotThrow()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var services = new ServiceCollection();
        services.AddSingleton<ILogger<ValidationBehavior<TestForcableCommand, bool>>>(
            sp => Output.BuildLoggerFor<ValidationBehavior<TestForcableCommand, bool>>()
        );

        var mockValidator = Substitute.For<IValidator<TestForcableCommand>>();
        mockValidator.ValidateAsync(Arg.Any<ValidationContext<TestForcableCommand>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new ValidationResult(new[] { new ValidationFailure("Prop", "WarnMsgForce") { Severity = Severity.Warning } })));
        services.AddSingleton(mockValidator);

        var serviceProvider = services.BuildServiceProvider();
        var behavior = new ValidationBehavior<TestForcableCommand, bool>(new[] { mockValidator }, serviceProvider);

        var request = new TestForcableCommand { Force = true };
        RequestHandlerDelegate<bool> next = (ct) => Task.FromResult(true);

        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_result = await behavior.Handle(request, next, CancellationToken.None);

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.True(asserted_result); // Execution continues since warning is ignored by force

        #endregion
    }
}
