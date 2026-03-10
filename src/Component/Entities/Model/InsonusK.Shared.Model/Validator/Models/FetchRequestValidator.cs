using FluentValidation;
using InsonusK.Shared.Model.Template;
using InsonusK.Shared.Model.Validator.Properties;

namespace InsonusK.Shared.Model.Validator.Models;

public class FetchRequestValidator : AbstractValidator<IFetchRequest>
{
    public FetchRequestValidator()
    {
        RuleFor(x => x.PageSize).IsNotNegative();
        RuleFor(x => x.Page).IsNotNegative();
    }
}