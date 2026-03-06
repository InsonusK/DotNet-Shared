using FluentValidation;
using InsonusK.Shared.Command.Interfaces;

namespace InsonusK.Shared.Command.Validation.Validators;

public class CommandWithBodyValidator<TCommand, TBody> : AbstractValidator<TCommand>
    where TCommand : ICommandWithBody<TBody>
{
    public CommandWithBodyValidator(IEnumerable<IValidator<TBody>> bodyValidators)
    {
        RuleFor(cmd => cmd.Body)
            .NotNull()
            .When(cmd => cmd.BodyRequired)
            .WithErrorCode("BodyRequired")
            .WithMessage("Command body cannot be null.");
        When(cmd => cmd.Body != null, () =>
        {
            RuleFor(cmd => cmd.Body)
                .SetValidator(new CompositeValidator<TBody>(bodyValidators));
        });
    }
}