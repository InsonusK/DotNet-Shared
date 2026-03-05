using System.Reflection;
using Ardalis.Specification;
using InsonusK.Shared.Command.EntityLoading.Interfaces;
using InsonusK.Shared.Command.Interfaces;
using InsonusK.Shared.Command.Interfaces.Models;
using InsonusK.Shared.DataBase.Models;
using InsonusK.Shared.DataBase.Spec;
using InsonusK.Shared.Models.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InsonusK.Shared.Command.EntityLoading.Services;

public class EntityProvider
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger _logger;

    public EntityProvider(
        IServiceProvider serviceProvider,
        ILogger<EntityProvider> logger)
    {
        _serviceProvider = serviceProvider;
        this._logger = logger;
    }
    public async Task<object?> Resolve(IEntityKey entityKey, CancellationToken ct)
    {
        var method = typeof(EntityProvider)
            .GetMethod(nameof(Resolve), BindingFlags.Public | BindingFlags.Instance)!
            .MakeGenericMethod(entityKey.EntityType);

        return await (Task<object?>)method.Invoke(this, new object[] { entityKey, ct })!;
    }
    
    public async Task<TEntity?> Resolve<TEntity>(IEntityKey entityKey, CancellationToken ct) where TEntity : class
    {
        var entityExtractor = _serviceProvider.GetService<IEntityCommandExtractor<TEntity>>();
        if (entityExtractor != null)
        {
            _logger.LogInformation("Resolving entity of type {EntityType} with string id {StringId} using custom extractor", typeof(TEntity).Name, entityKey.StringId);
            return await entityExtractor.GetAsync(entityKey, ct);
        }
        else if (typeof(IGuidModel).IsAssignableFrom(typeof(TEntity)))
        {
            _logger.LogInformation("Resolving entity of type {EntityType} with string id {StringId} using Guid resolver", typeof(TEntity).Name, entityKey.StringId);
            var method = typeof(EntityProvider)
                .GetMethod(nameof(ResolveGuid), BindingFlags.NonPublic | BindingFlags.Instance)!
                .MakeGenericMethod(typeof(TEntity));

            return (TEntity?)await (Task<object?>)method.Invoke(this, new object[] { entityKey, ct })!;
        }
        else if (typeof(EntityBase).IsAssignableFrom(typeof(TEntity)))
        {
            _logger.LogInformation("Resolving entity of type {EntityType} with string id {StringId} using int resolver", typeof(TEntity).Name, entityKey.StringId);
            var method = typeof(EntityProvider)
                    .GetMethod(nameof(ResolveInt), BindingFlags.NonPublic | BindingFlags.Instance)!
                    .MakeGenericMethod(typeof(TEntity));

            return (TEntity?)await (Task<object?>)method.Invoke(this, new object[] { entityKey, ct })!;
        }
        else
        {
            _logger.LogWarning("Cannot resolve entity of type {EntityType} with string id {StringId} because it does not implement Custom Extractor, IGuidModel or EntityBase", typeof(TEntity).Name, entityKey.StringId);
            return null;
        }
    }

    private async Task<TEntity?> ResolveInt<TEntity>(IEntityKey entityKey, CancellationToken ct) where TEntity : EntityBase
    {
        var repo = _serviceProvider.GetRequiredService<IReadRepositoryBase<TEntity>>();
        if (!int.TryParse(entityKey.StringId, out var intId))
            return null;
        var entity = await repo.GetByIdAsync(intId, ct);
        return entity;
    }

    private async Task<TEntity?> ResolveGuid<TEntity>(IEntityKey entityKey, CancellationToken ct) where TEntity : EntityBase, IGuidModel
    {
        var repo = _serviceProvider.GetRequiredService<IReadRepositoryBase<TEntity>>();
        var spec = new ByStringIdSpec<TEntity>(entityKey.StringId);
        var entity = await repo.SingleOrDefaultAsync(spec, ct);
        return entity;
    }
}