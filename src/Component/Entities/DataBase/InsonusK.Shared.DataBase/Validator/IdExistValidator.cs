using Ardalis.Specification;
using FluentValidation;
using FluentValidation.Validators;
using InsonusK.Shared.DataBase.Models;

namespace InsonusK.Shared.DataBase.Validator.Properties;

public class IdExistValidator<TEntity, TDb> : AsyncPropertyValidator<TEntity, int>
    where TDb : EntityBase
{
    private readonly IReadRepositoryBase<TDb> _repository;

    public override string Name => "IsExistingId";
    public const string Code = "IsNotExistingId";

    protected override string GetDefaultMessageTemplate(string errorCode)
        => "{PropertyName} does not exist in the database.";

    public IdExistValidator(IReadRepositoryBase<TDb> repository)
    {
        _repository = repository;
    }

    public override async Task<bool> IsValidAsync(
        ValidationContext<TEntity> context, int value, CancellationToken cancellation)
    {
        var entity = await _repository.GetByIdAsync(value, cancellation);
        return entity != null;
    }
}