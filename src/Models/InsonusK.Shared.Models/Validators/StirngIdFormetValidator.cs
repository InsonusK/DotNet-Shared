using FluentValidation;
using FluentValidation.Validators;
using InsonusK.Shared.Models.Tools;

namespace InsonusK.Shared.Models.Validators;

public class StirngIdFormatValidator<T> : PropertyValidator<T, string>
{
    public override string Name => "InvalidStringId";
    public const string Code = "InvalidStringId";
    protected override string GetDefaultMessageTemplate(string errorCode) => "String Id is not int or guid";
    
    public override bool IsValid(ValidationContext<T> context, string value)
    {
        return value.ToId(out _, out _) ;
    }
}

public static class StirngIdFormatValidatorExtensions
{
    public static IRuleBuilderOptions<T, string> StringIdIdFormatIsValid<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .SetValidator(new StirngIdFormatValidator<T>())
            .WithErrorCode(StirngIdFormatValidator<T>.Code)
            .WithSeverity(Severity.Error);
    }
}
