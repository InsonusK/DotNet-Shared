using Ardalis.GuardClauses;
using InsonusK.Shared.Command.EntityLoading.Services;
using InsonusK.Shared.Command.EntityLoading.Tools;
using InsonusK.Shared.Command.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InsonusK.Shared.Command.EntityLoading.Pipeline;

internal class EntityLoadingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IValidatableCommand, IRequest
{
    private readonly ILogger _logger;
    private readonly CommandContextManager _commandContextManager;
    private readonly EntityProvider _entityResolver;

    public EntityLoadingBehavior(
        ILogger<EntityLoadingBehavior<TRequest, TResponse>> logger,
        CommandContextManager commandContextManager,
        EntityProvider entityResolver)
    {
        _logger = logger;
        _commandContextManager = commandContextManager;
        _entityResolver = entityResolver;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var newContext = _commandContextManager.StartFor(request);
        
        try
        {
            foreach (var entityKey in request.EntityKeys)
            {
                var entity = await _entityResolver.Resolve(entityKey, cancellationToken);

                if (entity != null)
                    newContext.AddEntity(entityKey.EntityType, entity);
                else
                    throw new NotFoundException(entityKey.StringId, $"Entity of type {entityKey.EntityType.Name} with id {entityKey.StringId} not found");
            }

            return await next();
        }
        finally
        {
            _commandContextManager.EndFor(request);
        }

    }

}