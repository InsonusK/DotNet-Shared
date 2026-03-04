using System;
using InsonusK.Shared.DataBase.Models;

namespace InsonusK.Shared.Command.Validation.Interfaces;

/// <summary>
/// Интерфейс для чтения данных контекста обработки Command
/// </summary>
public interface IValidationEntitiesReadContext
{
    TEntity GetEntity<TEntity>() where TEntity : EntityBase;
    bool TryGetEntity<TEntity>(out TEntity? entity) where TEntity : EntityBase;
    bool HasEntity<TEntity>() where TEntity : EntityBase;
}
