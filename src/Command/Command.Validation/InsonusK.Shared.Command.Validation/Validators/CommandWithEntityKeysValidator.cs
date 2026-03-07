using FluentValidation;
using InsonusK.Shared.Command.Interfaces;
using InsonusK.Shared.Command.Interfaces.Models;
using InsonusK.Shared.DataBase.Models;
using InsonusK.Shared.Command.Validation.Validators.Properties;

namespace InsonusK.Shared.Command.Validation.Validators;

public class CommandWithEntityKeysValidator<TCommand> : AbstractValidator<TCommand>
    where TCommand : ICommandWithEntityKeys
{
    public CommandWithEntityKeysValidator()
    {
        RuleFor(cmd => cmd.EntityKeys)
            .NotNull()
            .WithErrorCode("EntityKeysRequired")
            .WithMessage("Entity keys cannot be null.");
        When(cmd => cmd.EntityKeys != null, () =>
        {
            RuleForEach(cmd => cmd.EntityKeys)
                .SetValidator(new EntityKeyValidator());
        });
    }
}
