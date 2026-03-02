using Ardalis.Result;
using MediatR;
using Microsoft.Extensions.Logging;
using InsonusK.Shared.Mediator.CommandContext.Service;
using InsonusK.Shared.Mediator.CommandContext.Command;

namespace InsonusK.Shared.Mediator.CommandContext.Handler;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TRequestCmd"></typeparam>
/// <typeparam name="TResponse"></typeparam>
/// <typeparam name="TEntity"></typeparam>
public class CommandWithKeysHandler<TRequestCmd, TResponse>
    : IPipelineBehavior<TRequestCmd, TResponse>
    where TRequestCmd : CommandWithEntityKeys
    where TResponse : IResult
{
    private readonly ILogger _logger;
    private readonly CommandContextContainer _commandContext;
    private readonly IServiceProvider _provider;

    public CommandWithKeysHandler(
        ILogger<CommandWithKeysHandler<TRequestCmd, TResponse>> logger,
        CommandContextContainer commandContext, 
        IServiceProvider provider)
    {
        _logger = logger;
        _commandContext = commandContext;
        _provider = provider;
    }

    public async Task<TResponse> Handle(
        TRequestCmd request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Extract StringId Entity from {typeof(TRequestCmd).Name}: {request}");

        await FillContext(request, cancellationToken);

        var _return = await next();
        return _return;
    }

    private async Task FillContext(TRequestCmd request, CancellationToken cancellationToken)
    {
        foreach (var entityKey in request.EntityKeys)
        {
            await _commandContext.AddEntityFromEntityKeyAsync(entityKey, cancellationToken);
        }
    }
}