using FluentValidation;

namespace InsonusK.Shared.Model.Validator.Properties;

public static class IsNotNegativeValidationExtensions
{
    public const string Code = "IsNegative";

    public static IRuleBuilderOptions<T, int> IsNotNegative<T>(
        this IRuleBuilder<T, int> ruleBuilder)
    {
        return ruleBuilder
            .GreaterThanOrEqualTo(0)
            .WithErrorCode(Code)
            .WithMessage("{PropertyName} must not be negative.")
            .WithSeverity(Severity.Error);
    }

    public static IRuleBuilderOptions<T, long> IsNotNegative<T>(
        this IRuleBuilder<T, long> ruleBuilder)
    {
        return ruleBuilder
            .GreaterThanOrEqualTo(0L)
            .WithErrorCode(Code)
            .WithMessage("{PropertyName} must not be negative.")
            .WithSeverity(Severity.Error);
    }

    public static IRuleBuilderOptions<T, decimal> IsNotNegative<T>(
        this IRuleBuilder<T, decimal> ruleBuilder)
    {
        return ruleBuilder
            .GreaterThanOrEqualTo(0m)
            .WithErrorCode(Code)
            .WithMessage("{PropertyName} must not be negative.")
            .WithSeverity(Severity.Error);
    }

    public static IRuleBuilderOptions<T, double> IsNotNegative<T>(
        this IRuleBuilder<T, double> ruleBuilder)
    {
        return ruleBuilder
            .GreaterThanOrEqualTo(0d)
            .WithErrorCode(Code)
            .WithMessage("{PropertyName} must not be negative.")
            .WithSeverity(Severity.Error);
    }
}
