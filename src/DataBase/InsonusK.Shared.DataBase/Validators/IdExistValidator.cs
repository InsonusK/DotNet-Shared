using Ardalis.Specification;
using FluentValidation;
using FluentValidation.Validators;
using InsonusK.Shared.DataBase.Models;
using InsonusK.Shared.Models.Validators;

namespace InsonusK.Shared.DataBase.Validators;

public class IdExistValidator<Tentity, Tdb> : AsyncPropertyValidator<Tentity, int> where Tdb : EntityBase
{
    private readonly IReadRepositoryBase<Tdb> _repository;

    public override string Name => "IdDoesNotExist";
    protected override string GetDefaultMessageTemplate(string errorCode) => "Id doesn't exist in database";
    public IdExistValidator(IReadRepositoryBase<Tdb> repository)
    {
        _repository = repository;
    }

    public override async Task<bool> IsValidAsync(ValidationContext<Tentity> context, int value, CancellationToken cancellation)
    {
        var existEnity = await _repository.GetByIdAsync(value, cancellation);
        var exists = existEnity != null;
        return exists;
    }
}

public static class IdValidatorExtensions
{
    public static IRuleBuilderOptions<TValidatedDto, int> IdExist<TValidatedDto, TEntity>(
        this IRuleBuilderInitial<TValidatedDto, int> ruleBuilder, IReadRepositoryBase<TEntity> repository)
         where TEntity : ConstantGuidEntity
    {
        return ruleBuilder
            .Cascade(CascadeMode.Stop)
            .SetValidator(new IdFormatValidator<TValidatedDto>()).WithSeverity(Severity.Error)
            .SetAsyncValidator(new IdExistValidator<TValidatedDto, TEntity>(repository)).WithSeverity(Severity.Error);
    }
    public static IRuleBuilderOptions<TValidatedDto, int> StringIdExist<TValidatedDto, TEntity>(
        this IRuleBuilderInitialCollection<TValidatedDto, int> ruleBuilder, IReadRepositoryBase<TEntity> repository)
         where TEntity : ConstantGuidEntity
    {
        return ruleBuilder
            .Cascade(CascadeMode.Stop)
            .SetValidator(new IdFormatValidator<TValidatedDto>()).WithSeverity(Severity.Error)
            .SetAsyncValidator(new IdExistValidator<TValidatedDto, TEntity>(repository)).WithSeverity(Severity.Error);
    }
}