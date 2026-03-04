namespace InsonusK.Shared.Command.Validation.Interfaces;

public interface IForcableValidatableCommand : IValidatableCommand
{
    bool Force { get; }
}