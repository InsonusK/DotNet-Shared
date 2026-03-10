using FluentValidation;
using InsonusK.Shared.Model.Common;
using InsonusK.Shared.Model.Validator;

namespace InsonusK.Shared.Mediator.ExceptionHandler.Validators;

public class CommandValidator : AbstractValidator<IClientActionTimeStamp>
{
    public CommandValidator()
    {
        RuleFor(x => x).SetValidator(new ClientActionTimeStampValidator());
    }
}
