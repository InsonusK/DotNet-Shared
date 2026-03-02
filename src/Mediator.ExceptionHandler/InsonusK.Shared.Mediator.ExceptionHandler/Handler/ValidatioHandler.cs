using Ardalis.Result;
using MediatR;
using InsonusK.Shared.Mediator.ExceptionHandler.Service;
using InsonusK.Shared.Mediator.ExceptionHandler.Validators;
using InsonusK.Shared.Models.Common;

namespace InsonusK.Shared.Mediator.ExceptionHandler.Handler;

//TODO: вынести валидацию IUserActionTimeStamp в отдельный проект CommandValidation
// убрать зависимость проекта Shared.Mediator.ExceptionHandler от Shared.Models

/// <summary>
/// 
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResult"></typeparam>
/// <typeparam name="TEntity"></typeparam>
public class CommandValidationHandler<TRequest, TResult>
    : IPipelineBehavior<TRequest, TResult>
    where TRequest : IClientActionTimeStamp
    where TResult : IResult
{
    private readonly ArdalisResultReflectionFactory<TResult> _resultReflectionFactory;
    private readonly CommandValidator _commandValidator;

    public CommandValidationHandler(
        ArdalisResultReflectionFactory<TResult> resultReflectionFactory,
        CommandValidator commandValidator)
    {
        _resultReflectionFactory = resultReflectionFactory;
        _commandValidator = commandValidator;
    }

    public async Task<TResult> Handle(
        TRequest request,
        RequestHandlerDelegate<TResult> next,
        CancellationToken cancellationToken)
    {
        var validation_result = _commandValidator.Validate(request);
        if (!validation_result.IsValid)
            return _resultReflectionFactory.CreateInvalidResult(validation_result.Errors);

        return await next();
    }
}