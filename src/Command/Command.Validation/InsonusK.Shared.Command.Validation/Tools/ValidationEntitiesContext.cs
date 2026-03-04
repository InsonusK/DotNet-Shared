using System;
using InsonusK.Shared.Command.Validation.Interfaces;
using InsonusK.Shared.DataBase.Models;
using InsonusK.Shared.Models.Common;

namespace InsonusK.Shared.Command.Validation.Tools;


/// <summary>
/// Контейнер данных контекста обработки Command
/// </summary>
public class ValidationEntitiesContext: IValidationEntitiesReadContext
{
    private Dictionary<Type, object> _entities = new Dictionary<Type, object>();

    public void AddEntity<TEntity>(TEntity entity) where TEntity : EntityBase
    {
        if (!AssertIsExist(entity))
            _entities.Add(typeof(TEntity), entity);
    }


    private bool AssertIsExist<TEntity>(TEntity entity) where TEntity : EntityBase
    {
        if (!_entities.TryGetValue(typeof(TEntity), out var _object))
            return false;

        TEntity existEntity = (TEntity)_object;

        if (existEntity.Id != 0 && entity.Id != 0)
            if (existEntity.Id != entity.Id)
                throw new ArgumentException($"Another Entity {typeof(TEntity).Name} has been already added");
            else
                return false;

        if (!typeof(IGuidModel).IsAssignableFrom(typeof(TEntity)))
            return false;

        if (entity is IGuidModel constantEntity && existEntity is IGuidModel existConstantEntity)
            if (existConstantEntity.Guid != constantEntity.Guid)
                throw new ArgumentException($"Another Entity {typeof(TEntity).Name} has been already added");

        return true;
    }

    public TEntity GetEntity<TEntity>() where TEntity : EntityBase
    {
        if (_entities.TryGetValue(typeof(TEntity), out var entity))
            return (TEntity)entity;
        throw new ArgumentException($"Entity {typeof(TEntity).Name} not found");
    }

    public bool TryGetEntity<TEntity>(out TEntity? entity) where TEntity : EntityBase
    {
        if (_entities.TryGetValue(typeof(TEntity), out var _entity))
        {
            entity = (TEntity)_entity;
            return true;
        }
        entity = null;
        return false;
    }

    public bool HasEntity<TEntity>() where TEntity : EntityBase
    {
        return _entities.ContainsKey(typeof(TEntity));
    }
}