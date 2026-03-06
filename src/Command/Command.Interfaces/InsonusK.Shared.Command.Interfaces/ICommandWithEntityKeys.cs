namespace InsonusK.Shared.Command.Interfaces;

/// <summary>
/// Represents a command that targets one or more entity keys.
/// </summary>
public interface ICommandWithEntityKeys
{
    /// <summary>
    /// Gets the collection of entity keys associated with this command.
    /// </summary>
    IReadOnlyCollection<IEntityKey> EntityKeys { get; }
}
