using InsonusK.Shared.Command.Interfaces.Models;

namespace InsonusK.Shared.Command.Interfaces.Helpers;

public static class IValidatableCommandExtensions
{
    public static EntityKey<TEntity> GetKey<TEntity>(this IValidatableCommand command) where TEntity : class
    {
        return command.EntityKeys.Get<TEntity>();
    }
}
