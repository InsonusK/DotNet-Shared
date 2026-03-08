using Ardalis.Specification;
using FluentValidation;
using FluentValidation.Validators;
using InsonusK.Shared.DataBase.Models;
using InsonusK.Shared.DataBase.Spec;
using InsonusK.Shared.Models.Validators;

namespace InsonusK.Shared.DataBase.Validators;

public class StringIdExistValidator<TValidatedDto, TEntity> : AsyncPropertyValidator<TValidatedDto, string> where TEntity : ConstantGuidEntity
{

    public override string Name => "StringIdDoesNotExist";
    public const string Code = "StringIdDoesNotExist";
    private readonly IReadRepositoryBase<TEntity> _repository;
    private readonly bool _validateOnlyIfNotEmpty;
    private readonly Guid? _newGuid;

    protected override string GetDefaultMessageTemplate(string errorCode) => "String Id doesn't exist in database";

    public StringIdExistValidator(
        IReadRepositoryBase<TEntity> repository,
        bool validateOnlyIfNotEmpty = false,
        Guid? newGuid = null)
    {
        _repository = repository;
        _validateOnlyIfNotEmpty = validateOnlyIfNotEmpty;
        _newGuid = newGuid;
    }

    public override async Task<bool> IsValidAsync(ValidationContext<TValidatedDto> context, string value, CancellationToken cancellationToken)
    {
        if (_validateOnlyIfNotEmpty && string.IsNullOrWhiteSpace(value))
            return true;
        if (_newGuid.HasValue && 
            Guid.TryParse(value, out var parsed) &&
            parsed == _newGuid.Value)
            return true;

        var spec = new ByStringIdSpec<TEntity>(value);
        var exists = await _repository.CountAsync(spec, cancellationToken) > 0;
        return exists;
    }
}

public static class StirngIdValidatorExtensions
{
    public static IRuleBuilderOptions<TValidatedDto, string> StringIdExist<TValidatedDto, TEntity>(
        this IRuleBuilderInitial<TValidatedDto, string> ruleBuilder, IReadRepositoryBase<TEntity> repository,
        Guid? newGuid = null) where TEntity : ConstantGuidEntity
    {
        return ruleBuilder
            .Cascade(CascadeMode.Stop)
            .SetValidator(new StirngIdFormatValidator<TValidatedDto>()).WithSeverity(Severity.Error)
            .SetAsyncValidator(new StringIdExistValidator<TValidatedDto, TEntity>(repository, false, newGuid)).WithSeverity(Severity.Error);
    }
    public static IRuleBuilderOptions<TValidatedDto, string> StringIdExist<TValidatedDto, TEntity>(
        this IRuleBuilderInitialCollection<TValidatedDto, string> ruleBuilder, IReadRepositoryBase<TEntity> repository,
        Guid? newGuid = null) where TEntity : ConstantGuidEntity
    {
        return ruleBuilder
            .Cascade(CascadeMode.Stop)
            .SetValidator(new StirngIdFormatValidator<TValidatedDto>()).WithSeverity(Severity.Error)
            .SetAsyncValidator(new StringIdExistValidator<TValidatedDto, TEntity>(repository, false, newGuid)).WithSeverity(Severity.Error);
    }
}