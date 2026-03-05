using FluentValidation;
using InsonusK.Shared.Command.Interfaces;

namespace InsonusK.Shared.Command.Validation.Validators;

public class CommandWithBodyValidator<TCommand, TBody> : AbstractValidator<TCommand>
    where TCommand : ICommandWithBody<TBody>
{
    public CommandWithBodyValidator(IValidator<TBody> bodyValidator)
    {
        RuleFor(cmd => cmd.Body)
            .NotNull()
            .When(cmd => cmd.BodyRequired)
            .WithMessage("Command body cannot be null.");
        When(cmd => cmd.Body != null, () =>
        {
            RuleFor(cmd => cmd.Body)
                .SetValidator(bodyValidator);
        });
    }
}