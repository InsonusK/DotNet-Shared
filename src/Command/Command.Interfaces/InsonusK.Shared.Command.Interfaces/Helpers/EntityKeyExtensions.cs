using InsonusK.Shared.Command.Interfaces.Models;

namespace InsonusK.Shared.Command.Interfaces.Helpers;

/// <summary>
/// Extension methods for <see cref="IEntityKey"/> collections.
/// </summary>
public static class EntityKeyExtensions
{
    /// <summary>
    /// Gets the <see cref="EntityKey{TEntity}"/> for the specified entity type from a collection of keys.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="keys">The collection of entity keys.</param>
    /// <returns>The <see cref="EntityKey{TEntity}"/> corresponding to the requested type.</returns>
    /// <exception cref="InvalidOperationException">Thrown if no key of the specified type exists in the collection.</exception>
    public static EntityKey<TEntity> Get<TEntity>(
        this IEnumerable<IEntityKey> keys) where TEntity : class
    {
        return (EntityKey<TEntity>)
            keys.First(x => x.EntityType == typeof(TEntity));
    }
}
