using FluentValidation;
using InsonusK.Shared.Command.EntityLoading.Interfaces;
using InsonusK.Shared.Command.Interfaces;
using InsonusK.Shared.DataBase.Models;
namespace InsonusK.Shared.Command.Validation.Extensions;

/// <summary>
/// Extensions for <see cref="IValidationContext"/> to allow access to pre-loaded entities during validation.
/// </summary>
public static class ValidationContextExtensions
{
    private const string EntitiesKey = "ValidationEntitiesContext";

    /// <summary>
    /// Sets the command context containing pre-loaded entities into the validation context.
    /// Used internally by the validation pipeline behavior.
    /// </summary>
    /// <param name="context">The validation context.</param>
    /// <param name="ctx">The command context with loaded entities.</param>
    internal static void SetEntitiesContext(this IValidationContext context, ICommandContext ctx)
        => context.RootContextData[EntitiesKey] = ctx;

    /// <summary>
    /// Retrieves the command context containing pre-loaded entities from the validation context.
    /// </summary>
    /// <param name="context">The validation context.</param>
    /// <returns>The <see cref="ICommandContext"/> with loaded entities.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the context is not found in RootContextData.</exception>
    public static ICommandContext GetEntitiesContext(this IValidationContext context)
    {
        if (context.RootContextData.TryGetValue(EntitiesKey, out var obj)
            && obj is ICommandContext ctx)
            return ctx;
        throw new InvalidOperationException("ValidationEntitiesContext not found in RootContextData");
    }

    /// <summary>
    /// Gets a specific type of pre-loaded entity from the validation context.
    /// Helper method to quickly access loaded entities inside FluentValidation rules.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to retrieve. Must inherit from <see cref="EntityBase"/>.</typeparam>
    /// <param name="context">The validation context.</param>
    /// <returns>The pre-loaded entity, or null if it was not loaded.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the entities context is not found.</exception>
    public static TEntity? GetEntity<TEntity>(this IValidationContext context)
        where TEntity : EntityBase
        => context.GetEntitiesContext().Get<TEntity>();

    public static object? GetEntityFromContext(this IValidationContext context, IEntityKey key)
    {
        var method = typeof(ValidationContextExtensions)
            .GetMethod(nameof(ValidationContextExtensions.GetEntity))
            ?.MakeGenericMethod(key.EntityType);

        if (method == null)
            throw new InvalidOperationException("Cannot find GetEntity generic method.");

        return method.Invoke(null, new object[] { context });
    }
}