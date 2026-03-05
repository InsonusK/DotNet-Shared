namespace InsonusK.Shared.Command.Interfaces;

public interface IForcableValidatableCommand : IValidatableCommand
{
    bool Force { get; }
}