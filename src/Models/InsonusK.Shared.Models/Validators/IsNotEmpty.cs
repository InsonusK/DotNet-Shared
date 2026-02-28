using FluentValidation;

namespace InsonusK.Shared.Models.Validators;
public static class IsNotEmptyValidatorExtension
{
    public const string ErrorCode = "IsNotEmpty";
    public const string Message = "Must not be empty";
    public const Severity DefaultSeverity = Severity.Error;

    public static void IsNotEmpty<TDto, TProperty>(this IRuleBuilder<TDto, IEnumerable<TProperty>> ruleBuilder)
    {
        ruleBuilder.Must(x=> x.Any()).WithSeverity(DefaultSeverity).WithMessage(Message).WithErrorCode(ErrorCode);
    }
}