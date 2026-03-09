using FluentValidation;
using FluentValidation.Validators;

namespace InsonusK.Shared.Models.Validators;

public class IdFormatValidator<T> : PropertyValidator<T, int>
{
    public override string Name => "InvalidId";
    public const string Code = "InvalidId";
    protected override string GetDefaultMessageTemplate(string errorCode) => "Id is not int";
    
    public override bool IsValid(ValidationContext<T> context, int value)
    {
        return value > 0;
    }
}

public static class IdFormatValidatorExtensions
{
    public static IRuleBuilderOptions<T, int> IdIdFormatIsValid<T>(
        this IRuleBuilder<T, int> ruleBuilder)
    {
        return ruleBuilder
            .SetValidator(new IdFormatValidator<T>())
            .WithErrorCode(StirngIdFormatValidator<T>.Code)
            .WithSeverity(Severity.Error);
    }
}