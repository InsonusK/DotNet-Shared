namespace InsonusK.Shared.Command.Interfaces;

/// <summary>
/// Represents a validatable command that can bypass certain validation rules or force an action.
/// </summary>
public interface IForcableValidatableCommand : ICommandWithEntityKeys
{
    /// <summary>
    /// Gets a value indicating whether the command should force the operation, ignoring some constraints like id existence.
    /// </summary>
    bool Force { get; }
}