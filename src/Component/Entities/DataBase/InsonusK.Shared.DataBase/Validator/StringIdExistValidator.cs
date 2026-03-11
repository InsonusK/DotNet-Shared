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
    private readonly bool _validateOnlyIfNotEmpty;
    private readonly Guid? _newGuid;

    protected override string GetDefaultMessageTemplate(string errorCode)
        => "{PropertyName} does not exist in the database.";

    public StringIdExistValidator(
        IReadRepositoryBase<TEntity> repository,
        bool validateOnlyIfNotEmpty = false,
        Guid? newGuid = null)
    {
        _repository = repository;
        _validateOnlyIfNotEmpty = validateOnlyIfNotEmpty;
        _newGuid = newGuid;
    }

    public override async Task<bool> IsValidAsync(
        ValidationContext<TValidatedDto> context, string value, CancellationToken cancellationToken)
    {
        if (_validateOnlyIfNotEmpty && string.IsNullOrWhiteSpace(value))
            return true;

        if (_newGuid.HasValue &&
            Guid.TryParse(value, out var parsed) &&
            parsed == _newGuid.Value)
            return true;

        var spec = new ByStringIdSpec<TEntity>(value);
        return await _repository.CountAsync(spec, cancellationToken) > 0;
    }
}
