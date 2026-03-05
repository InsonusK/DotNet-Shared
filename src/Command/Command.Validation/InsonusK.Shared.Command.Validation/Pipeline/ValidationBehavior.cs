using Ardalis.Specification;
using FluentValidation;
using InsonusK.Shared.Command.Validation.Extensions;
using InsonusK.Shared.Command.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using InsonusK.Shared.Command.EntityLoading.Interfaces;
using InsonusK.Shared.Command.EntityLoading.Services;

namespace InsonusK.Shared.Command.Validation.Pipeline;

/// <summary>
/// Pipeline behavior for MediatR that handles validation for commands implementing <see cref="ICommandWithEntityKeys"/>.
/// It automatically resolves entities specified in the command's <see cref="ICommandWithEntityKeys.EntityKeys"/> 
/// and populates a <see cref="ValidationEntitiesContext"/> before running FluentValidation validators.
/// </summary>
/// <typeparam name="TRequest">The type of the request, must implement <see cref="ICommandWithEntityKeys"/>.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommandWithEntityKeys,IRequest
{
    private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ICommandContextSource _commandContextSrc;

    public ValidationBehavior(
        IEnumerable<IValidator<TRequest>> validators, 
        IServiceProvider serviceProvider)
    {
        _logger = serviceProvider.GetService<ILogger<ValidationBehavior<TRequest, TResponse>>>()!;
        _validators = validators;
        _commandContextSrc = serviceProvider.GetService<ICommandContextSource>();
        if (_commandContextSrc == null){
            _logger.LogDebug("No ICommandContextSource registered, entity context will not be available in validators");
            _commandContextSrc = serviceProvider.GetRequiredService<EntityProvider>();
        }
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (!_validators.Any())
            return await next();

        ICommandContext cmdCtx = await _commandContextSrc.GetForAsync(request,ct);
        var validationContext = new ValidationContext<TRequest>(request);
        validationContext.SetEntitiesContext(cmdCtx);

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