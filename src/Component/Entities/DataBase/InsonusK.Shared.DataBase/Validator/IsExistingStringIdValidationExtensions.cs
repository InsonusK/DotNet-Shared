using Ardalis.Specification;
using FluentValidation;
using InsonusK.Shared.DataBase.Models;
using InsonusK.Shared.DataBase.Validator.Properties;
using InsonusK.Shared.Model.Validator.Properties;

namespace InsonusK.Shared.DataBase.Validator;

public static class IsExistingStringIdValidationExtensions
{
    /// <summary>
    /// Cascades: IsValidStringId (format check) → StringIdExistValidator (DB lookup).
    /// </summary>
    public static IRuleBuilderOptions<TDto, string> IsExistingStringId<TDto, TEntity>(
        this IRuleBuilderInitial<TDto, string> ruleBuilder,
        IReadRepositoryBase<TEntity> repository)
        where TEntity : ConstantGuidEntity
    {
        return ruleBuilder
            .Cascade(CascadeMode.Stop)
            .IsValidStringId()
            .SetAsyncValidator(new StringIdExistValidator<TDto, TEntity>(repository))
            .WithErrorCode(StringIdExistValidator<TDto, TEntity>.Code)
            .WithSeverity(Severity.Error);
    }

    /// <summary>
    /// Collection overload: cascades IsValidStringId → StringIdExistValidator.
    /// </summary>
    public static IRuleBuilderOptions<TDto, string> IsExistingStringId<TDto, TEntity>(
        this IRuleBuilderInitialCollection<TDto, string> ruleBuilder,
        IReadRepositoryBase<TEntity> repository)
        where TEntity : ConstantGuidEntity
    {
        return ruleBuilder
            .Cascade(CascadeMode.Stop)
            .IsValidStringId()
            .SetAsyncValidator(new StringIdExistValidator<TDto, TEntity>(repository))
            .WithErrorCode(StringIdExistValidator<TDto, TEntity>.Code)
            .WithSeverity(Severity.Error);
    }
}
