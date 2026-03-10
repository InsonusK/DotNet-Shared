using FluentValidation;
using InsonusK.Shared.Model.Common;
using InsonusK.Shared.Model.Validator.Properties;

namespace InsonusK.Shared.Model.Validator.Models;

public class ClientActionTimeStampValidator : AbstractValidator<IClientActionTimeStamp>
{
    public ClientActionTimeStampValidator()
    {
        RuleFor(x => x.ActionTimeStamp).IsNotEmpty();
    }
}
