using FluentValidation;
using FluentValidation.Validators;
using InsonusK.Shared.Model.Tools;

namespace InsonusK.Shared.Model.Validator.Properties;

public class IsValidStringIdValidator<T> : PropertyValidator<T, string>
{
    public override string Name => "IsValidStringId";
    public const string Code = "IsNotValidStringId";

    protected override string GetDefaultMessageTemplate(string errorCode)
        => "{PropertyName} is not a valid string id (must be an integer or a guid).";

    public override bool IsValid(ValidationContext<T> context, string value)
    {
        return value.ToId(out _, out _);
    }
}

public static class IsValidStringIdValidationExtensions
{
    public static IRuleBuilderOptions<T, string> IsValidStringId<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .SetValidator(new IsValidStringIdValidator<T>())
            .WithErrorCode(IsValidStringIdValidator<T>.Code)
            .WithSeverity(Severity.Error);
    }
}
