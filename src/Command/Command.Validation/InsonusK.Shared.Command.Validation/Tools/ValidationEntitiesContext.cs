using System;
using InsonusK.Shared.Command.Validation.Interfaces;
using InsonusK.Shared.DataBase.Models;
using InsonusK.Shared.Models.Common;

namespace InsonusK.Shared.Command.Validation.Tools;


/// <summary>
/// Контейнер данных контекста обработки Command
/// </summary>
public class ValidationEntitiesContext : IValidationEntitiesReadContext
{
    private Dictionary<Type, object> _entities = new Dictionary<Type, object>();

    public void AddEntity(EntityBase entity)
    {
        var type = entity.GetType();
        if (_entities.ContainsKey(type))
            throw new ArgumentException($"Another Entity {type.Name} has been already added");
        _entities[entity.GetType()] = entity;
    }

    public void AddEntity<TEntity>(TEntity entity) where TEntity : EntityBase
    {
        this.AddEntity((EntityBase)entity);
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