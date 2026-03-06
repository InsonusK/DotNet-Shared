using FluentValidation;
using FluentValidation.Validators;
using InsonusK.Shared.Command.Interfaces;
using InsonusK.Shared.Command.Validation.Extensions;
using InsonusK.Shared.Models.Common;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace InsonusK.Shared.Command.Validation.Validators.Properties;
public class EntityKeyValidator : AbstractValidator<IEntityKey>
{
    public EntityKeyValidator()
    {
        // валидируем через кастомный метод
        RuleFor(key => key)
            .Custom((key, context) =>
            {
                // Получаем Entity через ValidationContext
                var existEntity = context.GetEntityFromContext(key);

                if (existEntity == null)
                {
                    context.AddFailure(new FluentValidation.Results.ValidationFailure(context.PropertyPath, $"Entity of type {key.EntityType.Name} with id '{key.StringId}' was not found.")
                    {
                        ErrorCode = "EntityNotFound"
                    });
                    return;
                }

                // Если версия обязательна
                if (key is IEntityVersionedKey versionedKey && versionedKey.VersionRequired)
                {
                    if (existEntity is not IVersionatedModel existVersionedEntity)
                    {
                        context.AddFailure(new FluentValidation.Results.ValidationFailure(context.PropertyPath, $"Entity {key.EntityType.Name} does not have a Version property.")
                        {
                            ErrorCode = "VersionPropertyMissing"
                        });
                        return;
                    }

                    if (!uint.TryParse(versionedKey.Version, out uint keyUintVersion))
                    {
                        context.AddFailure(new FluentValidation.Results.ValidationFailure(context.PropertyPath, $"Invalid version format for entity {key.EntityType.Name} with id '{key.StringId}'. Version should be a positive integer.")
                        {
                            ErrorCode = "InvalidVersionFormat"
                        });
                        return;
                    }

                    if (keyUintVersion != existVersionedEntity.Version)
                    {
                        context.AddFailure(new FluentValidation.Results.ValidationFailure(context.PropertyPath, $"Entity {key.EntityType.Name} with id '{key.StringId}' version mismatch. Expected: {existVersionedEntity.Version}, actual: {keyUintVersion}")
                        {
                            ErrorCode = "VersionMismatch"
                        });
                    }
                }
            });
    }
}