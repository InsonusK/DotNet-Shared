using Ardalis.Result;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using InsonusK.Shared.Mediator.ExceptionHandler.Service;
using InsonusK.Shared.Command.Exceptions;

namespace InsonusK.Shared.Mediator.ExceptionHandler.Handler;

//TODO: вынести валидацию IUserActionTimeStamp в отдельный проект CommandValidation
// убрать зависимость проекта Shared.Mediator.ExceptionHandler от Shared.Models

/// <summary>
/// 
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResult"></typeparam>
/// <typeparam name="TEntity"></typeparam>
public class ExceptionHandler<TRequest, TResult>
    : IPipelineBehavior<TRequest, TResult>
    where TRequest : notnull
    where TResult : IResult
{
    private readonly ArdalisResultReflectionFactory<TResult> _resultReflectionFactory;
    private readonly ILogger _logger;

    public ExceptionHandler(
        ArdalisResultReflectionFactory<TResult> resultReflectionFactory,
        ILogger<ExceptionHandler<TRequest, TResult>> logger)
    {
        _resultReflectionFactory = resultReflectionFactory;
        _logger = logger;
    }

    public async Task<TResult> Handle(
        TRequest request,
        RequestHandlerDelegate<TResult> next,
        CancellationToken cancellationToken)
    {
        try
        {
            var _return = await next();
            return _return;
        }
        catch (ResultException ex)
        {
            _logger.LogInformation("Handle ResultException");
            //return ex.ToResultOf<TResult>();
            return _resultReflectionFactory.FromException(ex); 
        }
        catch (ValidationException validationEx)
        {
            _logger.LogInformation("Handle ValidationException");
            return _resultReflectionFactory.CreateInvalidResult(validationEx.Errors);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogCritical(ex, "Handler throwed error of unexpected behavior of code");
            return _resultReflectionFactory.CreateCriticalErrorResult(_errorMessage);
        }
        catch (Exception ex)
        {

            _logger.LogCritical(ex, "Handler throw unhandled exception");
            // Возвращаем объект типа Result, если TResponse является Result<T>
            // Используем рефлексию для создания экземпляра Result с ошибкой
            if (typeof(TResult).IsAssignableTo(typeof(IResult)))
            {
                return _resultReflectionFactory.CreateCriticalErrorResult(_errorMessage);
            }

            // Если TResponse не является Result<T>, просто перебрасываем исключение
            throw new ApplicationException("Get unexpected error please contact support");
        }
    }
    private const string _errorMessage = "Get unexpected error please contact support";


}