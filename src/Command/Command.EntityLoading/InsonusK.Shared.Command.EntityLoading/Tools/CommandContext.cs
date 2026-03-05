using InsonusK.Shared.Command.EntityLoading.Interfaces;
using InsonusK.Shared.Command.Interfaces;
using InsonusK.Shared.Command.Interfaces.Models;
using InsonusK.Shared.DataBase.Models;

namespace InsonusK.Shared.Command.EntityLoading.Tools;


/// <summary>
/// Контейнер данных контекста обработки Command
/// </summary>
public class CommandContext : ICommandContext
{
    private Dictionary<Type, object> _entities = new Dictionary<Type, object>();

    public void AddEntity(Type entityType, object entity)
    {
        if (_entities.ContainsKey(entityType))
            throw new ArgumentException($"Another Entity {entityType} has been already added");
        _entities[entityType] = entity!;
    }
    public void Add<TEntity>(TEntity entity) where TEntity : EntityBase
    {
        AddEntity(typeof(TEntity), entity);
    }

    public TEntity Get<TEntity>() where TEntity : EntityBase
    {
        if (_entities.TryGetValue(typeof(TEntity), out var entity))
            return (TEntity)entity;
        throw new ArgumentException($"Entity {typeof(TEntity).Name} not found");
    }

    public bool TryGet<TEntity>(out TEntity? entity) where TEntity : EntityBase
    {
        if (_entities.TryGetValue(typeof(TEntity), out var _entity))
        {
            entity = (TEntity)_entity;
            return true;
        }
        entity = null;
        return false;
    }

    public bool Has<TEntity>() where TEntity : EntityBase
    {
        return _entities.ContainsKey(typeof(TEntity));
    }
}