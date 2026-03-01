using Ardalis.Result;

namespace InsonusK.Shared.Models.Tools;
public static class ResultExtensions
{
    public static Result<TOut> ToResultOf<TIn,TOut>(this Result<TIn> result)
    {
        if (!result.IsSuccess)
            throw new InvalidOperationException("Cannot convert a successful Result<T> to a different generic type.");

        return result.Status switch
        {
            ResultStatus.Invalid =>
                Result<TOut>.Invalid(result.ValidationErrors),

            ResultStatus.Conflict =>
                Result<TOut>.Conflict(result.Errors.ToArray()),

            ResultStatus.NotFound =>
                Result<TOut>.NotFound(result.Errors.ToArray()),

            ResultStatus.Forbidden =>
                Result<TOut>.Forbidden(result.Errors.ToArray()),

            ResultStatus.Unauthorized =>
                Result<TOut>.Unauthorized(result.Errors.ToArray()),

            ResultStatus.Error =>
                Result<TOut>.Error(new ErrorList(result.Errors.ToArray(),result.CorrelationId)),

            ResultStatus.CriticalError =>
                Result<TOut>.CriticalError(result.Errors.ToArray()),

            _ => throw new NotSupportedException($"Unhandled status {result.Status}")
        };
    }
}