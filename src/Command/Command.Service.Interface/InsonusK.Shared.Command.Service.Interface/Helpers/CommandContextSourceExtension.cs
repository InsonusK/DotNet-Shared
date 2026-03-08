using InsonusK.Shared.Command.Interface;
using InsonusK.Shared.DataBase.Models;
using MediatR;

namespace InsonusK.Shared.Command.Service.Interface.Helpers;

/// <summary>
/// Provides extension methods for <see cref="ICommandContextSource"/> to simplify entity retrieval.
/// </summary>
public static class CommandContextSourceExtensions
{
    /// <summary>
    /// Gets an entity from the command context source using the provided command.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command, must implement <see cref="ICommandWithEntityKeys"/> and <see cref="IBaseRequest"/>.</typeparam>
    /// <typeparam name="TEntity">The type of the entity, must inherit from <see cref="EntityBase"/>.</typeparam>
    /// <param name="source">The command context source.</param>
    /// <param name="command">The command containing entity keys.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The loaded entity.</returns>
    public static async Task<TEntity> GetEntityAsync<TCommand, TEntity>(this ICommandContextSource source, TCommand command, CancellationToken cancellationToken)
        where TCommand : ICommandWithEntityKeys, IBaseRequest
        where TEntity : EntityBase
    {
        var cmdContext = await source.GetForAsync(command, cancellationToken);
        return cmdContext.Get<TEntity>();    
    }   

    /// <summary>
    /// Attempts to get an entity from the command context source using the provided command.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command, must implement <see cref="ICommandWithEntityKeys"/> and <see cref="IBaseRequest"/>.</typeparam>
    /// <typeparam name="TEntity">The type of the entity, must inherit from <see cref="EntityBase"/>.</typeparam>
    /// <param name="source">The command context source.</param>
    /// <param name="command">The command containing entity keys.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The loaded entity or null if not found.</returns>
    public static async Task<TEntity?> TryGetEntityAsync<TCommand, TEntity>(this ICommandContextSource source, TCommand command, CancellationToken cancellationToken)
        where TCommand : ICommandWithEntityKeys, IBaseRequest
        where TEntity : EntityBase
    {
        var cmdContext = await source.GetForAsync(command, cancellationToken);
        if (cmdContext.TryGet<TEntity>(out var entity))
            return entity;
        return null;
    }   
}
