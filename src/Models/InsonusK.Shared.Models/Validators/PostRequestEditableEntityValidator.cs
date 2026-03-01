using InsonusK.Shared.Models.Template;
using FluentValidation;

namespace InsonusK.Shared.Models.Validators;

public class PostRequestEditableEntityValidator : AbstractValidator<IPostRequestEditableEntity>
{
    public PostRequestEditableEntityValidator()
    {
        RuleFor(x => x).SetValidator(new GuidModelValidator());
    }
}
