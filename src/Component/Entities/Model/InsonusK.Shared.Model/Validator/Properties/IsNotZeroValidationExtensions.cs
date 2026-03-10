using FluentValidation;

namespace InsonusK.Shared.Model.Validator.Properties;

public static class IsNotZeroValidationExtensions
{
    public const string Code = "IsZero";

    public static IRuleBuilderOptions<T, int> IsNotZero<T>(
        this IRuleBuilder<T, int> ruleBuilder)
    {
        return ruleBuilder
            .NotEqual(0)
            .WithErrorCode(Code)
            .WithMessage("{PropertyName} must not be zero.")
            .WithSeverity(Severity.Error);
    }

    public static IRuleBuilderOptions<T, long> IsNotZero<T>(
        this IRuleBuilder<T, long> ruleBuilder)
    {
        return ruleBuilder
            .NotEqual(0L)
            .WithErrorCode(Code)
            .WithMessage("{PropertyName} must not be zero.")
            .WithSeverity(Severity.Error);
    }

    public static IRuleBuilderOptions<T, decimal> IsNotZero<T>(
        this IRuleBuilder<T, decimal> ruleBuilder)
    {
        return ruleBuilder
            .NotEqual(0m)
            .WithErrorCode(Code)
            .WithMessage("{PropertyName} must not be zero.")
            .WithSeverity(Severity.Error);
    }

    public static IRuleBuilderOptions<T, double> IsNotZero<T>(
        this IRuleBuilder<T, double> ruleBuilder)
    {
        return ruleBuilder
            .NotEqual(0d)
            .WithErrorCode(Code)
            .WithMessage("{PropertyName} must not be zero.")
            .WithSeverity(Severity.Error);
    }
}
