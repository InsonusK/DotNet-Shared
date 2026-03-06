using System.Reflection;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation.Results;
using InsonusK.Shared.Command.Exceptions;
using Microsoft.Extensions.Logging;

namespace InsonusK.Shared.Mediator.ExceptionHandler.Service;

public class ArdalisResultReflectionFactory<TResult> where TResult : IResult
{
    private ILogger _logger;
    private Type _responseType = typeof(TResult);
    public ArdalisResultReflectionFactory(ILogger<ArdalisResultReflectionFactory<TResult>> logger)
    {
        _logger = logger;

    }
    public TResult FromException(ResultException exception)
    {
        if (exception == null)
            throw new ArgumentNullException(nameof(exception));

        return Map(exception.InnerResult);
    }
    public TResult Map(IResult result)
    {
        switch (result.Status)
        {
            case ResultStatus.Ok:
            case ResultStatus.Created:
                throw new ArgumentOutOfRangeException("Result must be not Ok or Created");
            case ResultStatus.NoContent:
                return CreateResultNoContent(nameof(Result.NoContent));
            case ResultStatus.Error:
                return CreateResultWithContent(nameof(Result.Error), new ErrorList(result.Errors.ToArray()));
            case ResultStatus.Forbidden:
                return CreateResultWithContent(nameof(Result.Forbidden), result.Errors.ToArray());
            case ResultStatus.Unauthorized:
                return CreateResultWithContent(nameof(Result.Unauthorized), result.Errors.ToArray());
            case ResultStatus.Unavailable:
                return CreateResultWithContent(nameof(Result.Unavailable), result.Errors.ToArray());
            case ResultStatus.Invalid:
                return CreateResultWithContent(nameof(Result.Invalid), result.ValidationErrors.ToArray());
            case ResultStatus.Conflict:
                return CreateResultWithContent(nameof(Result.Conflict), result.Errors.ToArray());
            case ResultStatus.NotFound:
                return CreateResultWithContent(nameof(Result.NotFound), result.Errors.ToArray());
            case ResultStatus.CriticalError:
                return CreateResultWithContent(nameof(Result.CriticalError), result.Errors.ToArray());
            default:
                throw new NotSupportedException($"Unhandled status {result.Status}");
        }
    }


    public TResult CreateInvalidResult(IEnumerable<ValidationFailure> errors)
    {
        IEnumerable<ValidationError> _errors = new ValidationResult(errors).AsErrors();
        return CreateResultWithContent(nameof(Result.Invalid), _errors);
    }

    public TResult CreateCriticalErrorResult(params string[] errorMessages)
    {
        return CreateResultWithContent(nameof(Result.CriticalError), errorMessages);
    }

    private Type GetResultType()
    {
        if (_responseType == typeof(Result))
            return _responseType;

        if (_responseType.IsGenericType &&
            _responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            try
            {
                var bodyType = _responseType.GetGenericArguments()[0];
                var genericResultType = typeof(Result<>).MakeGenericType(bodyType);
                if (genericResultType == null)
                {
                    _logger.LogCritical("Cannot create generic type for {responseType}", _responseType.Name);
                    throw new ApplicationException($"Cannot create generic type for {_responseType.Name}");
                }
                return genericResultType;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Cannot create generic type for {responseType}", _responseType.Name);
                throw new ApplicationException($"Cannot create generic type for {_responseType.Name}", ex);
            }
        }

        _logger.LogCritical("Unsupported response type: {responseType}", _responseType);
        throw new InvalidOperationException(
            $"Unsupported response type: {_responseType}");
    }
    private MethodInfo GetCreateMethod(Type resultType, string methodName, Type[] argType)
    {
        MethodInfo? _return = null;
        try
        {
            _return = resultType
                        .GetMethod(methodName,
                            BindingFlags.Public | BindingFlags.Static, null,
                            argType, null);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Cannot find {methodName} for {responseType}", methodName, resultType.FullName);
            throw new ApplicationException($"Cannot find {methodName} for {resultType.FullName}", ex);
        }

        if (_return == null)
        {
            _logger.LogCritical("Cannot find {methodName} for {responseType}", methodName, resultType.FullName);
            throw new ApplicationException($"Cannot find {methodName} for {resultType.FullName}");
        }
        return _return;

    }
    private TResult CreateResultNoContent(string methodName)
    {
        try
        {
            var args = new object[] { };
            Type resultType = GetResultType();
            MethodInfo method = GetCreateMethod(resultType, methodName, new Type[] { });
            return (TResult)method!.Invoke(null, args)!;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Cannot create {methodName} result", methodName);
            throw new ApplicationException($"Cannot create {methodName} result", ex);
        }
    }
    private TResult CreateResultWithContent(string methodName, Type[] argType, object[] args)
    {
        Type resultType = GetResultType();
        MethodInfo method = GetCreateMethod(resultType, methodName, argType);
        try
        {
            return (TResult)method!.Invoke(null, args)!;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Cannot create {methodName} for {responseType}", methodName, resultType.FullName);
            throw new ApplicationException($"Cannot create {methodName} for {resultType.FullName}", ex);
        }
    }
    private TResult CreateResultWithContent<TContent>(string methodName, TContent content) where TContent : notnull
    {
        var args = new object[] { content };
        Type resultType = GetResultType();
        MethodInfo method = GetCreateMethod(resultType, methodName, new Type[] { typeof(TContent) });
        try
        {
            return (TResult)method!.Invoke(null, args)!;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Cannot create {methodName} for {responseType}", methodName, resultType.FullName);
            throw new ApplicationException($"Cannot create {methodName} for {resultType.FullName}", ex);
        }
    }
}
