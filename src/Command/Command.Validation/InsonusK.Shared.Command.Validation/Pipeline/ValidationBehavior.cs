using Ardalis.Specification;
using FluentValidation;
using InsonusK.Shared.Command.Validation.Extensions;
using InsonusK.Shared.Command.Validation.Interfaces;
using InsonusK.Shared.Command.Validation.Tools;
using InsonusK.Shared.DataBase.Models;
using InsonusK.Shared.DataBase.Spec;
using InsonusK.Shared.Models.Common;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InsonusK.Shared.Command.Validation.Pipeline;

/// <summary>
/// Pipeline behavior for MediatR that handles validation for commands implementing <see cref="IValidatableCommand"/>.
/// It automatically resolves entities specified in the command's <see cref="IValidatableCommand.EntityKeys"/> 
/// and populates a <see cref="ValidationEntitiesContext"/> before running FluentValidation validators.
/// </summary>
/// <typeparam name="TRequest">The type of the request, must implement <see cref="IValidatableCommand"/>.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IValidatableCommand
{
    private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly IServiceProvider _serviceProvider;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators, IServiceProvider serviceProvider)
    {
        _logger = serviceProvider.GetService<ILogger<ValidationBehavior<TRequest, TResponse>>>()!;
        _validators = validators;
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (!_validators.Any())
            return await next();

        var validationContext = new ValidationContext<TRequest>(request);
        var entitiesContext = new ValidationEntitiesContext();
        var resolver = new EntityResolver(_serviceProvider);
        
        foreach (KeyValuePair<Type, string> kvp in request.EntityKeys.ToArray())
        {
            Type entityType = kvp.Key;
            string stringId = kvp.Value;

            var entity = await resolver.Resolve(entityType, stringId, ct);

            if (entity != null)
                entitiesContext.AddEntity((EntityBase)entity);
            else
                throw new ValidationException($"Entity of type {entityType.Name} with id {stringId} not found");
        }

        validationContext.SetEntitiesContext(entitiesContext);

        _logger.LogInformation("Validating command {CommandType} with {ValidatorCount} validators", typeof(TRequest).Name, _validators.Count());
        var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(validationContext, ct)));

        var validationErrors = validationResults.SelectMany(r => r.Errors);
        var serverities = validationErrors.Select(e => e.Severity);
        var errorCount = serverities.Count(s => s == Severity.Error);
        var otherCount = serverities.Count(s => s != Severity.Error);

        _logger.LogInformation("Validation completed for command {CommandType} with {ErrorCount} errors and {OtherCount} warnings", typeof(TRequest).Name, errorCount, otherCount);
        if (errorCount > 0)
            throw new ValidationException(validationErrors);
        else if (otherCount > 0)
            if (request is IForcableValidatableCommand forcableCommand)
                if (!forcableCommand.Force)
                    throw new ValidationException(validationErrors);

        return await next();
    }
}