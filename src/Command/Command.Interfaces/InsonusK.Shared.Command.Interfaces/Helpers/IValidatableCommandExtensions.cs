using InsonusK.Shared.Command.Interfaces.Models;

namespace InsonusK.Shared.Command.Interfaces.Helpers;

public static class IValidatableCommandExtensions
{
    public static EntityKey<TEntity> GetKey<TEntity>(this ICommandWithEntityKeys command) where TEntity : class
    {
        return command.EntityKeys.Get<TEntity>();
    }
}
