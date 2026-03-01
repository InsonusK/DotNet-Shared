using InsonusK.Shared.Models.Common;

public interface IEditableEntityCommand : IImmutableEntityCommand, IVersionatedModel
{
}
public interface IImmutableEntityCommand
{
    public string EntityStringId { get; }
}