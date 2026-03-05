using FluentValidation;
using InsonusK.Shared.Command.Interfaces;
using InsonusK.Shared.Command.Validation.Extensions;
using InsonusK.Shared.Models.Common;

namespace InsonusK.Shared.Command.Validation.Validators;

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
                    context.AddFailure($"Entity of type {key.EntityType.Name} with id '{key.StringId}' was not found.");
                    return;
                }

                // Если версия обязательна
                if (key is IEntityVersionedKey versionedKey && versionedKey.VersionRequired)
                {
                    if (existEntity is not IVersionatedModel existVersionedEntity)
                        throw new InvalidOperationException($"Entity {key.EntityType.Name} does not have a Version property.");

                    if (!uint.TryParse(versionedKey.Version, out uint keyUintVersion))
                        context.AddFailure($"Invalid version format for entity {key.EntityType.Name} with id '{key.StringId}'. Version should be a positive integer.");

                    if (keyUintVersion != existVersionedEntity.Version)
                        context.AddFailure($"Entity {key.EntityType.Name} with id '{key.StringId}' version mismatch. Expected: {existVersionedEntity.Version}, actual: {keyUintVersion}");
                }
            });
    }
}