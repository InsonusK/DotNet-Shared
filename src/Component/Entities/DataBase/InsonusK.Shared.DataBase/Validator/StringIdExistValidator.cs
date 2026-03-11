using Ardalis.Specification;
using FluentValidation;
using FluentValidation.Validators;
using InsonusK.Shared.DataBase.Models;
using InsonusK.Shared.DataBase.Spec;

namespace InsonusK.Shared.DataBase.Validator.Properties;

public class StringIdExistValidator<TValidatedDto, TEntity> : AsyncPropertyValidator<TValidatedDto, string>
    where TEntity : ConstantGuidEntity
{
    public override string Name => "IsExistingStringId";
    public const string Code = "IsNotExistingStringId";

    private readonly IReadRepositoryBase<TEntity> _repository;

    protected override string GetDefaultMessageTemplate(string errorCode)
        => "{PropertyName} does not exist in the database.";

    public StringIdExistValidator(
        IReadRepositoryBase<TEntity> repository)
    {
        _repository = repository;
    }

    public override async Task<bool> IsValidAsync(
        ValidationContext<TValidatedDto> context, string value, CancellationToken cancellationToken)
    {
        var spec = new ByStringIdSpec<TEntity>(value);
        var existEntity = await _repository.SingleOrDefaultAsync(spec, cancellationToken);
        return existEntity != null;
    }
}
