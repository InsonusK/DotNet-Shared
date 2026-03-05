using InsonusK.Shared.Command.Interfaces.Models;

namespace InsonusK.Shared.Command.Interfaces.Helpers;

public static class EntityKeyExtensions
{
    public static EntityKey<TEntity> Get<TEntity>(
        this IEnumerable<IEntityKey> keys) where TEntity : class
    {
        return (EntityKey<TEntity>)
            keys.First(x => x.EntityType == typeof(TEntity));
    }
}
