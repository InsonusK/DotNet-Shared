using Ardalis.Result;

namespace InsonusK.Shared.Mediator.ExceptionHandler;


public class ResultException : Exception
{
    public ResultException(IResult result) : base(result.ToString())
    {
        innerResult = result;
    }

    public IResult innerResult { get; }
    public Result<T> ToResultOf<T>()
    {
        try
        {
            switch (innerResult.Status)
            {
                case ResultStatus.Ok:
                case ResultStatus.Created:
                    throw new ArgumentOutOfRangeException("Result must be not Ok or Created");
                case ResultStatus.NoContent:
                    return Result<T>.NoContent();
                case ResultStatus.Error:
                    return Result<T>.Error(new ErrorList(innerResult.Errors.ToArray()));
                case ResultStatus.Forbidden:
                    return Result<T>.Forbidden([.. innerResult.Errors]);
                case ResultStatus.Unauthorized:
                    return Result<T>.Unauthorized([.. innerResult.Errors]);
                case ResultStatus.Unavailable:
                    return Result<T>.Unavailable([.. innerResult.Errors]);
                case ResultStatus.Invalid:
                    return Result<T>.Invalid(innerResult.ValidationErrors);
                case ResultStatus.Conflict:
                    return Result<T>.Conflict([.. innerResult.Errors]);
                case ResultStatus.NotFound:
                    return Result<T>.NotFound([.. innerResult.Errors]);
                case ResultStatus.CriticalError:
                    return Result<T>.CriticalError([.. innerResult.Errors]);
                default:
                    throw new NotSupportedException($"Unhandled status {innerResult.Status}");
            }
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public Result ToResult()
    {
        switch (innerResult.Status)
        {
            case ResultStatus.Ok:
            case ResultStatus.Created:
                throw new ArgumentOutOfRangeException("Result must be not Ok or Created");
            case ResultStatus.NoContent:
                return Result.NoContent();
            case ResultStatus.Error:
                return Result.Error(new ErrorList(innerResult.Errors.ToArray()));
            case ResultStatus.Forbidden:
                return Result.Forbidden([.. innerResult.Errors]);
            case ResultStatus.Unauthorized:
                return Result.Unauthorized([.. innerResult.Errors]);
            case ResultStatus.Unavailable:
                return Result.Unavailable([.. innerResult.Errors]);
            case ResultStatus.Invalid:
                return Result.Invalid(innerResult.ValidationErrors);
            case ResultStatus.Conflict:
                return Result.Conflict([.. innerResult.Errors]);
            case ResultStatus.NotFound:
                return Result.NotFound([.. innerResult.Errors]);
            case ResultStatus.CriticalError:
                return Result.CriticalError([.. innerResult.Errors]);
            default:
                throw new NotSupportedException($"Unhandled status {innerResult.Status}");
        }
    }
}