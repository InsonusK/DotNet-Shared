using FluentValidation;
using InsonusK.Shared.Models.Interfaces;

namespace InsonusK.Shared.Models.Validators;

public class PatchRequestValidator : AbstractValidator<IPatchRequest>
{
    public const string AllNullCode = "AllPatchFieldsIsNull";
    public PatchRequestValidator()
    {
        RuleFor(x => x).Must(x => x.HaveAtLeastOneNonNullField())
            .WithSeverity(Severity.Error).WithErrorCode(AllNullCode)
            .WithMessage("All fields is Null");
    }
}