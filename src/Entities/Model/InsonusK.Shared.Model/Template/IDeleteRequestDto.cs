namespace InsonusK.Shared.Models.Template;


public interface IDeleteRequestWithLastStateOptionDto
{
    bool ReturnLastState { get; }
}

public interface IDeleteRequestFilterDeleted
{
    bool OnlyActive { get; }
}