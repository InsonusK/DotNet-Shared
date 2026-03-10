using FluentValidation;

namespace InsonusK.Shared.Model.Validator.Properties;

public static class IsUtcValidationExtensions
{
    public const string Code = "IsNotUtc";

    public static IRuleBuilderOptions<T, DateTime> IsUtc<T>(
        this IRuleBuilder<T, DateTime> ruleBuilder)
    {
        return ruleBuilder
            .Must(d => d.Kind != DateTimeKind.Unspecified)
            .WithErrorCode(Code)
            .WithMessage("{PropertyName} must have timezone information set (Kind must not be Unspecified).")
            .WithSeverity(Severity.Error);
    }

}
