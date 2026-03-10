using FluentValidation;
using InsonusK.Shared.Model.Common;
using InsonusK.Shared.Model.Validator.Properties;

namespace InsonusK.Shared.Model.Validator.Models;

public class GuidModelValidator : AbstractValidator<IGuidModel>
{
    public GuidModelValidator()
    {
        RuleFor(x => x.Guid).IsNotEmpty();
    }
}
