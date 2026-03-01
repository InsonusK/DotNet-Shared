using FluentValidation;
using InsonusK.Shared.Models.Template;

namespace InsonusK.Shared.Models.Validators;
public class FetchRequestValidator : AbstractValidator<IFetchRequest>
{
    public FetchRequestValidator()
    {
        // Проверка, что Guid не является Guid.Empty
        RuleFor(x => x.PageSize).GreaterThanOrEqualTo(0).WithSeverity(Severity.Info);
        RuleFor(x => x.Page).GreaterThanOrEqualTo(0).WithSeverity(Severity.Info);
    }
}