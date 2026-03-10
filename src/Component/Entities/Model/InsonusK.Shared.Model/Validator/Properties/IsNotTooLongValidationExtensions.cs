using FluentValidation;

namespace InsonusK.Shared.Model.Validator.Properties;

public static class IsNotTooLongValidationExtensions
{
    public const string Code = "IsTooLong";

    public static IRuleBuilderOptions<T, string> IsNotTooLong<T>(
        this IRuleBuilder<T, string> ruleBuilder, int maxLength)
    {
        return ruleBuilder
            .MaximumLength(maxLength)
            .WithErrorCode(Code)
            .WithMessage($"{{PropertyName}} must not exceed {maxLength} characters.")
            .WithSeverity(Severity.Error);
    }
}
