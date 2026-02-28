namespace InsonusK.Shared.Models.Common;

public interface IUpdateInfoModelReadOnly : IVersionatedModel
{
    DateTimeOffset ServerUpdatedDateTime { get; }
    DateTimeOffset UserUpdatedDateTime { get; }

}
public interface IUpdateInfoModel : IUpdateInfoModelReadOnly
{
    new DateTimeOffset ServerUpdatedDateTime { get; set; }
    new DateTimeOffset UserUpdatedDateTime { get; set; }

    public static void ValidateUpdateValue(ICreationInfoModel model, DateTimeOffset serverUpdatedDateTime)
    {
        if (model.ServerCreatedDateTime > serverUpdatedDateTime)
            throw new ApplicationException("Cann't set up ServerUpdateDatetime earlier than ServerCreateDatetime");
    }
}

