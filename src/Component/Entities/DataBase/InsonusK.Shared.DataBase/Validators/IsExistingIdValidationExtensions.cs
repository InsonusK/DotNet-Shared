using Ardalis.Specification;
using FluentValidation;
using InsonusK.Shared.DataBase.Models;
using InsonusK.Shared.DataBase.Validators.Properties;
using InsonusK.Shared.Model.Validator.Properties;

namespace InsonusK.Shared.DataBase.Validators;

public static class IsExistingIdValidationExtensions
{
    /// <summary>
    /// Cascades: IsNotNegative → IsNotZero → IdExistValidator (DB lookup).
    /// </summary>
    public static IRuleBuilderOptions<TDto, int> IsExistingId<TDto, TEntity>(
        this IRuleBuilderInitial<TDto, int> ruleBuilder,
        IReadRepositoryBase<TEntity> repository)
        where TEntity : ConstantGuidEntity
    {
        return ruleBuilder
            .Cascade(CascadeMode.Stop)
            .IsNotNegative()
            .IsNotZero()
            .SetAsyncValidator(new IdExistValidator<TDto, TEntity>(repository))
            .WithErrorCode(IdExistValidator<TDto, TEntity>.Code)
            .WithSeverity(Severity.Error);
    }

    /// <summary>
    /// Collection overload: cascades IsNotNegative → IsNotZero → IdExistValidator.
    /// </summary>
    public static IRuleBuilderOptions<TDto, int> IsExistingId<TDto, TEntity>(
        this IRuleBuilderInitialCollection<TDto, int> ruleBuilder,
        IReadRepositoryBase<TEntity> repository)
        where TEntity : ConstantGuidEntity
    {
        return ruleBuilder
            .Cascade(CascadeMode.Stop)
            .IsNotNegative()
            .IsNotZero()
            .SetAsyncValidator(new IdExistValidator<TDto, TEntity>(repository))
            .WithErrorCode(IdExistValidator<TDto, TEntity>.Code)
            .WithSeverity(Severity.Error);
    }
}
