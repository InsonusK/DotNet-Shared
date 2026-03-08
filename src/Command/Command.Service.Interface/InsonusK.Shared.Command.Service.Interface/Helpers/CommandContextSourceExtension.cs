using InsonusK.Shared.Command.Interface;
using InsonusK.Shared.DataBase.Models;
using MediatR;

namespace InsonusK.Shared.Command.Service.Interface.Helpers;

public static class CommandContextSourceExtensions
{
    public static async Task<TEntity> GetEntityAsync<TCommand, TEntity>(this ICommandContextSource source, TCommand command, CancellationToken cancellationToken)
        where TCommand : ICommandWithEntityKeys, IBaseRequest
        where TEntity : EntityBase
    {
        var cmdContext = await source.GetForAsync(command, cancellationToken);
        return cmdContext.Get<TEntity>();    
    }   

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
