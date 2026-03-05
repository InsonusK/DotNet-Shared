namespace InsonusK.Shared.Command.Interfaces;

public interface IForcableValidatableCommand : ICommandWithEntityKeys
{
    bool Force { get; }
}