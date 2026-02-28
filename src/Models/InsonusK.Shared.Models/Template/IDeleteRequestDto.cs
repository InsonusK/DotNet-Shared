namespace InsonusK.Shared.Models.Template;


public interface IDeleteRequestWithLastStateOptionDto
{
    bool ReturnLastState { get; }
}