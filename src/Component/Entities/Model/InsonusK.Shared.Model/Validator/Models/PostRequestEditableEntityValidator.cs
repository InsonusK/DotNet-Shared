using InsonusK.Shared.Model.Template;
using FluentValidation;

namespace InsonusK.Shared.Model.Validator.Models;

public class PostRequestEditableEntityValidator : AbstractValidator<IPostRequestEditableEntity>
{
    public PostRequestEditableEntityValidator()
    {
        RuleFor(x => x).SetValidator(new GuidModelValidator());
    }
}
