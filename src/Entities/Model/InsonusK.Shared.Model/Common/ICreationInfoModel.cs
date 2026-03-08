namespace InsonusK.Shared.Models.Common;

public interface ICreationInfoModelReadOnly
{
    DateTimeOffset ServerCreatedDateTime { get; }
    DateTimeOffset UserCreatedDateTime { get; }
}
public interface ICreationInfoModel : ICreationInfoModelReadOnly
{
    new DateTimeOffset ServerCreatedDateTime { get; set; }
    new DateTimeOffset UserCreatedDateTime { get; set; }
}