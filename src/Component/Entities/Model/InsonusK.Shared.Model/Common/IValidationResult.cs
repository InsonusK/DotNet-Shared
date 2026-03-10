using Ardalis.Result;

namespace InsonusK.Shared.Model.Common;

public interface IValidationResult
{
    public IEnumerable<ValidationError> ValidationMessages { get; }
}