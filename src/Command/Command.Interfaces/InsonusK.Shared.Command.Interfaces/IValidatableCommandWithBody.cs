namespace InsonusK.Shared.Command.Interfaces;

public interface IValidatableCommandWithBody<TBody>
{
    TBody Body { get; }
}