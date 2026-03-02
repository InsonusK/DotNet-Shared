using InsonusK.Shared.Mediator.CommandContext.Interfaces;
using Ardalis.Specification;
using Microsoft.Extensions.DependencyInjection;
using Ardalis.Result;
using InsonusK.Shared.Mediator.ExceptionHandler;
using InsonusK.Shared.Models.Common;
using InsonusK.Shared.DataBase.Models;

namespace InsonusK.Shared.Mediator.CommandContext.Service;

/// <summary>
/// Контейнер данных контекста обработки Command
/// </summary>
public class CommandContextContainer : ICommandContext
{
    public CommandContextContainer(IServiceProvider provider)
    {
        _provider = provider;
    }


    private Dictionary<Type, object> _entities = new Dictionary<Type, object>();
    private readonly IServiceProvider _provider;
    public async Task AddEntityAsync<TEntity>(int id) where TEntity : EntityBase
    {
        var taskRepository = _provider.GetRequiredService<IReadRepositoryBase<TEntity>>();
        var entity = await taskRepository.GetByIdAsync(id);

        AddEntity(entity!);
    }

    public async Task AddEntityFromEntityKeyAsync(IEntityKey entityKey, CancellationToken cancellationToken = default)
    {
        var entityExtractorType = typeof(IEntityCommandExtractor<>).MakeGenericType(entityKey.EntityType);
        dynamic entityExtractor = _provider.GetRequiredService(entityExtractorType);
        var entity = await entityExtractor.GetAsync(entityKey, cancellationToken);
        
        if (entityKey is IEntityKeyWithVersion versionedKey && entity is IVersionatedModel versionedEntity)
        {
            if (versionedEntity.Version != versionedKey.Version)
                throw new ResultException(Result.Conflict($"Entity {entityKey.EntityType.Name} version mismatch"));
        }
        AddEntity(entity);
    }

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
                throw new ArgumentException($"Entity {typeof(TEntity).Name} already added");
            else
                return false;

        if (!typeof(IGuidModel).IsAssignableFrom(typeof(TEntity)))
            return false;

        if (entity is IGuidModel constantEntity && existEntity is IGuidModel existConstantEntity)
            if (existConstantEntity.Guid != constantEntity.Guid)
                throw new ArgumentException($"Entity {typeof(TEntity).Name} already added");

        return true;
    }

    public TEntity Entity<TEntity>() where TEntity : EntityBase
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
}