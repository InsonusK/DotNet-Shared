using FluentValidation;

namespace InsonusK.Shared.Model.Validator.Properties;

public static class IsCollectionNotEmptyValidationExtensions
{
    public const string Code = "IsEmpty";

    public static IRuleBuilderOptions<TDto, IEnumerable<TProperty>> IsCollectionNotEmpty<TDto, TProperty>(
        this IRuleBuilder<TDto, IEnumerable<TProperty>> ruleBuilder)
    {
        return ruleBuilder
            .Must(x => x.Any())
            .WithErrorCode(Code)
            .WithMessage("{PropertyName} must not be empty.")
            .WithSeverity(Severity.Error);
    }
}
