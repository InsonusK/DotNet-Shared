using FluentValidation;
using InsonusK.Shared.Models.Common;
using InsonusK.Shared.Models.Validators;

namespace InsonusK.Shared.Mediator.ExceptionHandler.Validators;

public class CommandValidator : AbstractValidator<IClientActionTimeStamp>
{
    public CommandValidator()
    {
        RuleFor(x => x).SetValidator(new ClientActionTimeStampValidator());
    }
}
