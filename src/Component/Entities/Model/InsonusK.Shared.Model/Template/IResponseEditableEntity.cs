using Ardalis.Result;
using InsonusK.Shared.Model.Common;

namespace InsonusK.Shared.Model.Template;

public interface IResponseWithValidationInfo
{
    public IEnumerable<ValidationError> ValidationMessages { get; }
}
public interface IResponseNoGuidEditableEntity : IDbModel, IVersionatedModel, ICreationInfoModelReadOnly, IUpdateInfoModelReadOnly { }
public interface IResponseGuidEditableEntity : IResponseNoGuidEditableEntity, IGuidModel { }
