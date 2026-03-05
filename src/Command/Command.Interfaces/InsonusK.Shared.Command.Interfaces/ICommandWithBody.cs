namespace InsonusK.Shared.Command.Interfaces;

public interface ICommandWithBody<TBody>
{
    TBody Body { get; }
}