using FluentValidation;
using FluentValidation.Validators;
using InsonusK.Shared.Command.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace InsonusK.Shared.Command.Validation.Validators;

public class CommandWithBodyValidator<TCommand> : AbstractValidator<TCommand>
    where TCommand : ICommandWithBody
{
    private readonly IServiceProvider _serviceProvider;

    public CommandWithBodyValidator(IServiceProvider serviceProvider)
    {
        this._serviceProvider = serviceProvider;

        RuleFor(cmd => cmd.objBody)
            .NotNull()
            .When(cmd => cmd.BodyRequired)
            .WithErrorCode("BodyRequired")
            .OverridePropertyName("Body")
            .WithMessage("Command body cannot be null.");

        When(cmd => cmd.objBody != null, () =>
        {
            RuleFor(cmd => cmd.objBody).CustomAsync(async (body, context,ct) =>
                {
                    var bodyType = context.InstanceToValidate.BodyType;
                    var validatorType = typeof(IValidator<>).MakeGenericType(bodyType);
                    var validators = serviceProvider.GetServices(validatorType);

                    var compositeType = typeof(CompositeValidator<>).MakeGenericType(bodyType);
                    var compositeValidator = (IValidator)Activator.CreateInstance(compositeType, validators)!;

                    var childContext = context.CloneForChildValidator(body);
                    var validationResult = await compositeValidator.ValidateAsync(childContext,ct);
                   
                });
        });
    }
}