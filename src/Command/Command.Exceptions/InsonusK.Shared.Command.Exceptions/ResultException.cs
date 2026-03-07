using Ardalis.Result;

namespace InsonusK.Shared.Command.Exceptions;


public class ResultException : Exception
{
    public ResultException(IResult result) : base(result.ToString())
    {
        InnerResult = result;
    }

    public IResult InnerResult { get; }
    public Result<T> ToResultOf<T>()
    {
        try
        {
            switch (InnerResult.Status)
            {
                case ResultStatus.Ok:
                case ResultStatus.Created:
                    throw new ArgumentOutOfRangeException("Result must be not Ok or Created");
                case ResultStatus.NoContent:
                    return Result<T>.NoContent();
                case ResultStatus.Error:
                    return Result<T>.Error(new ErrorList(InnerResult.Errors.ToArray()));
                case ResultStatus.Forbidden:
                    return Result<T>.Forbidden([.. InnerResult.Errors]);
                case ResultStatus.Unauthorized:
                    return Result<T>.Unauthorized([.. InnerResult.Errors]);
                case ResultStatus.Unavailable:
                    return Result<T>.Unavailable([.. InnerResult.Errors]);
                case ResultStatus.Invalid:
                    return Result<T>.Invalid(InnerResult.ValidationErrors);
                case ResultStatus.Conflict:
                    return Result<T>.Conflict([.. InnerResult.Errors]);
                case ResultStatus.NotFound:
                    return Result<T>.NotFound([.. InnerResult.Errors]);
                case ResultStatus.CriticalError:
                    return Result<T>.CriticalError([.. InnerResult.Errors]);
                default:
                    throw new NotSupportedException($"Unhandled status {InnerResult.Status}");
            }
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public Result ToResult()
    {
        switch (InnerResult.Status)
        {
            case ResultStatus.Ok:
            case ResultStatus.Created:
                throw new ArgumentOutOfRangeException("Result must be not Ok or Created");
            case ResultStatus.NoContent:
                return Result.NoContent();
            case ResultStatus.Error:
                return Result.Error(new ErrorList(InnerResult.Errors.ToArray()));
            case ResultStatus.Forbidden:
                return Result.Forbidden([.. InnerResult.Errors]);
            case ResultStatus.Unauthorized:
                return Result.Unauthorized([.. InnerResult.Errors]);
            case ResultStatus.Unavailable:
                return Result.Unavailable([.. InnerResult.Errors]);
            case ResultStatus.Invalid:
                return Result.Invalid(InnerResult.ValidationErrors);
            case ResultStatus.Conflict:
                return Result.Conflict([.. InnerResult.Errors]);
            case ResultStatus.NotFound:
                return Result.NotFound([.. InnerResult.Errors]);
            case ResultStatus.CriticalError:
                return Result.CriticalError([.. InnerResult.Errors]);
            default:
                throw new NotSupportedException($"Unhandled status {InnerResult.Status}");
        }
    }
}