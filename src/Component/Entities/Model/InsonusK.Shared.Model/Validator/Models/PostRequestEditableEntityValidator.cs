using InsonusK.Shared.Model.Template;
using FluentValidation;

namespace InsonusK.Shared.Model.Validator;

public class PostRequestEditableEntityValidator : AbstractValidator<IPostRequestEditableEntity>
{
    public PostRequestEditableEntityValidator()
    {
        RuleFor(x => x).SetValidator(new GuidModelValidator());
    }
}
