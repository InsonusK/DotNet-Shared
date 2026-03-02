using Ardalis.Result;
using MediatR;
using Microsoft.Extensions.Logging;
using InsonusK.Shared.Mediator.CommandContext.Interfaces;
using InsonusK.Shared.Mediator.CommandContext.Service;

namespace InsonusK.Shared.Mediator.CommandContext.Handler;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
/// <typeparam name="TEntity"></typeparam>
public class CommandWithStringIdHandler<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IEntityKey
    where TResponse : IResult
{
    private readonly ILogger _logger;
    private readonly CommandContextContainer _commandContext;
    private readonly IServiceProvider _provider;

    public CommandWithStringIdHandler(
        ILogger<CommandWithStringIdHandler<TRequest, TResponse>> logger,
        CommandContextContainer commandContext, 
        IServiceProvider provider)
    {
        _logger = logger;
        _commandContext = commandContext;
        _provider = provider;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Extract StringId Entity from {typeof(TRequest).Name}: {request}");

        await FillContext(request, cancellationToken);

        var _return = await next();
        return _return;
    }

    private async Task FillContext(TRequest request, CancellationToken cancellationToken)
    {
        await _commandContext.AddEntityFromEntityKeyAsync(request, cancellationToken);
    }
}