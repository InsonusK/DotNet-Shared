namespace InsonusK.Shared.Command.Interfaces;

/// <summary>
/// Represents a command that provides a body.
/// </summary>
public interface ICommandWithBody
{
    /// <summary>
    /// Gets the type of the body.
    /// </summary>
    Type BodyType { get; }
    
    /// <summary>
    /// Gets the body object.
    /// </summary>
    object objBody { get; }
    
    /// <summary>
    /// Gets a value indicating whether the body is required.
    /// </summary>
    bool BodyRequired { get; }
}

/// <summary>
/// Represents a generic command with a specifically typed body.
/// </summary>
/// <typeparam name="TBody">The type of the body.</typeparam>
public interface ICommandWithBody<TBody>  : ICommandWithBody
{
    /// <summary>
    /// Gets the typed body object.
    /// </summary>
    TBody Body { get; }
}