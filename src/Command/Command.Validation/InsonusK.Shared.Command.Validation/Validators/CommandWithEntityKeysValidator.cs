using FluentValidation;
using InsonusK.Shared.Command.Interfaces;
using InsonusK.Shared.Command.Interfaces.Models;
using InsonusK.Shared.DataBase.Models;

namespace InsonusK.Shared.Command.Validation.Validators;

public class CommandWithEntityKeysValidator<TCommand, TResponse> : AbstractValidator<TCommand>
    where TCommand : ICommandWithEntityKeys
{
    public CommandWithEntityKeysValidator()
    {
        RuleFor(cmd => cmd.EntityKeys)
            .NotNull().WithMessage("Entity keys cannot be null.");

        RuleForEach(cmd => cmd.EntityKeys)
            .SetValidator(new EntityKeyValidator())
            .When(cmd => cmd.EntityKeys != null);
    }
}
