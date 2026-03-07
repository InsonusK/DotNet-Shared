using Divergic.Logging.Xunit;
using FluentValidation;
using InsonusK.Shared.Command.Validation.Extensions;
using InsonusK.Shared.Command.Validation.Pipeline;
using InsonusK.Shared.Command.Validation.Test.Validators;
using InsonusK.Shared.Command.Validation.Validators;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace InsonusK.Shared.Command.Validation.Test.Extensions;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class ServiceProviderExtensions_Test : LoggingTestsBase<ServiceProviderExtensions_Test>
{
    public ServiceProviderExtensions_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
    {
    }

    #region AddCommandValidation

    [Fact]
    public void AddCommandValidation_WHEN_called_THEN_registers_ValidationBehavior_as_pipeline_behavior()
    {
        Logger.LogDebug("Test ARRAY");
        var services = new ServiceCollection();

        Logger.LogDebug("Test ACT");
        services.AddCommandValidation();

        Logger.LogDebug("Test ASSERT");
        var registeredBehavior = services.FirstOrDefault(sd => sd.ServiceType == typeof(IPipelineBehavior<,>));
        Assert.NotNull(registeredBehavior);
        Assert.Equal(typeof(ValidationBehavior<,>), registeredBehavior.ImplementationType);
        Assert.Equal(ServiceLifetime.Scoped, registeredBehavior.Lifetime);
    }

    #endregion

    /// <summary>
    /// description: AddCommandValidation correctly registers CommandWithEntityKeysValidator
    /// input: IServiceCollection with AddCommandValidation called, then BuildServiceProvider
    /// output: The resolved validator for MockCommandWithEntityKeys
    /// expected_result: It resolves to an instance of CommandWithEntityKeysValidator
    /// </summary>
    [Fact]
    public void test_DI_Registrations_WHEN_AddCommandValidation_THEN_CommandWithEntityKeysValidator_Resolved()
    {
        #region Arrange
        Logger.LogDebug("Test ARRANGE");
        var services = new ServiceCollection();
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        services.AddCommandValidation();
        var serviceProvider = services.BuildServiceProvider();

        // Get matching validator for MockCommandWithEntityKeys
        var validator = serviceProvider.GetServices<IValidator<Mocks.MockCommandWithEntityKeys>>();
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        Assert.NotNull(validator);
        Assert.IsType<IValidator<Mocks.MockCommandWithEntityKeys>[]>(validator);
        Assert.Contains(validator, v => v is CommandWithEntityKeysValidator<Mocks.MockCommandWithEntityKeys>);
        #endregion
    }

    /// <summary>
    /// description: AddCommandValidation correctly registers CommandWithBodyValidator
    /// input: IServiceCollection with AddCommandValidation called, then BuildServiceProvider
    /// output: The resolved validator for MockCommandWithBody
    /// expected_result: It resolves to an instance of CommandWithBodyValidator
    /// </summary>
    [Fact]
    public async Task test_DI_Registrations_WHEN_AddCommandValidation_THEN_CommandWithBodyValidator_ResolvedAsync()
    {
        #region Arrange
        Logger.LogDebug("Test ARRANGE");
        var services = new ServiceCollection();
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");
        services.AddCommandValidation();
        services.AddTransient(typeof(IValidator<string>), typeof(MockBodyValidator));
        services.AddTransient(typeof(IValidator<string>), typeof(MockBody2Validator));
        var serviceProvider = services.BuildServiceProvider();

        // Get matching validator for MockCommandWithBody
        var validator = serviceProvider.GetService<IValidator<Mocks.MockCommandWithBody>>();

        // Resolve the inner validators directly to verify DI registration
        var bodyValidators = serviceProvider.GetServices<IValidator<string>>().ToList();
        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");
        Assert.NotNull(validator);
        Assert.IsType<CommandWithBodyValidator<Mocks.MockCommandWithBody>>(validator);

        // Verify that DI provides both validators
        Assert.Equal(2, bodyValidators.Count);
        Assert.Contains(bodyValidators, v => v is MockBodyValidator);
        Assert.Contains(bodyValidators, v => v is MockBody2Validator);

        // Prove CommandWithBodyValidator utilizes both injected validators 
        // by executing validation with a body of length 3 (fails MockBody2Validator length 5-10)
        var command = new Mocks.MockCommandWithBody { Body = "123", BodyRequired = true };
        var validationResult = await validator.ValidateAsync(command);

        Assert.False(validationResult.IsValid);
        Assert.Equal(2, validationResult.Errors.Count);
        Assert.Contains(validationResult.Errors, e => e.ErrorCode == "Code1");
        Assert.Contains(validationResult.Errors, e => e.ErrorCode == "Code2");
        #endregion
    }
}
