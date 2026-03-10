using FluentValidation;

namespace InsonusK.Shared.Model.Validator.Properties;

public static class IsNotEmptyValidationExtensions
{
    public const string Code = "IsEmpty";

    public static IRuleBuilderOptions<T, string> IsNotEmpty<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .WithErrorCode(Code)
            .WithMessage("{PropertyName} must not be empty.")
            .WithSeverity(Severity.Error);
    }

    public static IRuleBuilderOptions<T, Guid> IsNotEmpty<T>(
        this IRuleBuilder<T, Guid> ruleBuilder)
    {
        return ruleBuilder
            .NotEqual(Guid.Empty)
            .WithErrorCode(Code)
            .WithMessage("{PropertyName} must not be empty.")
            .WithSeverity(Severity.Error);
    }

    public static IRuleBuilderOptions<T, DateTime> IsNotEmpty<T>(
        this IRuleBuilder<T, DateTime> ruleBuilder)
    {
        return ruleBuilder
            .NotEqual(default(DateTime))
            .WithErrorCode(Code)
            .WithMessage("{PropertyName} must not be empty.")
            .WithSeverity(Severity.Error);
    }

    public static IRuleBuilderOptions<T, DateTimeOffset> IsNotEmpty<T>(
        this IRuleBuilder<T, DateTimeOffset> ruleBuilder)
    {
        return ruleBuilder
            .NotEqual(default(DateTimeOffset))
            .WithErrorCode(Code)
            .WithMessage("{PropertyName} must not be empty.")
            .WithSeverity(Severity.Error);
    }
}
