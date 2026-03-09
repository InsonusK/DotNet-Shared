using Ardalis.Result;
using InsonusK.Shared.Models.Common;

namespace InsonusK.Shared.Models.Template;

public interface IResponseWithValidationInfo
{
    public IEnumerable<ValidationError> ValidationMessages { get; }
}
public interface IResponseNoGuidEditableEntity : IDbModel, IVersionatedModel, ICreationInfoModelReadOnly, IUpdateInfoModelReadOnly { }
public interface IResponseGuidEditableEntity : IResponseNoGuidEditableEntity, IGuidModel { }
