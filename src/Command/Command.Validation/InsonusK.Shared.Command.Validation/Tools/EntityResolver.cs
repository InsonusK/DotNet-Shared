using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Ardalis.Specification;
using InsonusK.Shared.DataBase.Models;
using InsonusK.Shared.DataBase.Spec;
using InsonusK.Shared.Models.Common;
using Microsoft.Extensions.DependencyInjection;

namespace InsonusK.Shared.Command.Validation.Tools;

public class EntityResolver
{
    private readonly IServiceProvider _sp;

    private static readonly ConcurrentDictionary<Type, Func<IServiceProvider, string, CancellationToken, Task<object?>>>
        _cache = new();

    public EntityResolver(IServiceProvider sp)
    {
        _sp = sp;
    }

    public Task<object?> Resolve(Type entityType, string id, CancellationToken ct)
    {
        var resolver = _cache.GetOrAdd(entityType, BuildResolver);
        return resolver(_sp, id, ct);
    }

    private static Func<IServiceProvider, string, CancellationToken, Task<object?>> BuildResolver(Type entityType)
    {
        var method = typeof(EntityResolver)
            .GetMethod(nameof(BuildResolverGeneric), BindingFlags.NonPublic | BindingFlags.Static)!
            .MakeGenericMethod(entityType);

        return (Func<IServiceProvider, string, CancellationToken, Task<object?>>)
            method.Invoke(null, null)!;
    }

    private static Func<IServiceProvider, string, CancellationToken, Task<object?>> BuildResolverGeneric<TEntity>()
     where TEntity : EntityBase
    {
        if (typeof(IGuidModel).IsAssignableFrom(typeof(TEntity)))
            return BuildGuidResolver<TEntity>();

        return BuildIntResolver<TEntity>();
    }

    private static Func<IServiceProvider, string, CancellationToken, Task<object?>> BuildGuidResolver<TEntity>()
        where TEntity : EntityBase
    {
        return async (sp, stringId, ct) =>
        {
            var repo = sp.GetRequiredService<IReadRepositoryBase<TEntity>>();

            var specType = typeof(ByStringIdSpec<>).MakeGenericType(typeof(TEntity));
            dynamic spec = Activator.CreateInstance(specType, stringId, true)!;

            return await repo.SingleOrDefaultAsync(spec, ct);
        };
    }
    private static Func<IServiceProvider, string, CancellationToken, Task<object?>> BuildIntResolver<TEntity>()
        where TEntity : EntityBase
    {
        return async (sp, stringId, ct) =>
        {
            var repo = sp.GetRequiredService<IReadRepositoryBase<TEntity>>();

            if (!int.TryParse(stringId, out var intId))
                return null;

            return await repo.GetByIdAsync(intId, ct);
        };
    }
}