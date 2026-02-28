using Ardalis.Result;

namespace InsonusK.Shared.Models.Common;

public interface IValidationResult
{
    public IEnumerable<ValidationError> ValidationMessages { get; }
}