using InsonusK.Shared.Command.Interfaces.Models;

namespace InsonusK.Shared.Command.Interfaces.Helpers;

/// <summary>
/// Extension methods for commands that contain entity keys.
/// </summary>
public static class IValidatableCommandExtensions
{
    /// <summary>
    /// Gets the <see cref="EntityKey{TEntity}"/> for the specified entity type from the command's entity keys.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="command">The command containing the entity keys.</param>
    /// <returns>The <see cref="EntityKey{TEntity}"/> corresponding to the requested type.</returns>
    /// <exception cref="InvalidOperationException">Thrown if no key of the specified type exists in the command's keys.</exception>
    public static EntityKey<TEntity> GetKey<TEntity>(this ICommandWithEntityKeys command) where TEntity : class
    {
        return command.EntityKeys.Get<TEntity>();
    }
}
