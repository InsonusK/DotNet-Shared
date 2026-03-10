using FluentValidation;
using InsonusK.Shared.Model.Common;

namespace InsonusK.Shared.Model.Validator;

public class GuidModelValidator : AbstractValidator<IGuidModel>
{
    public const string ModelName = nameof(IGuidModel);
    public const string GuidEmptyCode = ModelName + ".EmptyGuid";
    public GuidModelValidator()
    {
        // Проверка, что Guid не является Guid.Empty
        RuleFor(x => x.Guid)
            .NotEqual(Guid.Empty)
            .WithSeverity(Severity.Error).WithErrorCode(GuidEmptyCode)
            .WithMessage("The GUID cannot be empty.");
    }
}
