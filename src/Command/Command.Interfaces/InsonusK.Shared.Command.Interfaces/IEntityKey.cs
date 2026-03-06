namespace InsonusK.Shared.Command.Interfaces;

/// <summary>
/// Represents type information for an entity.
/// </summary>
public interface IEntityType
{
    /// <summary>
    /// Gets the type of the entity.
    /// </summary>
    Type EntityType { get; }
}

/// <summary>
/// Represents a unique key for an entity, including its type and string identifier.
/// </summary>
public interface IEntityKey : IEntityType
{
    /// <summary>
    /// Gets the string identifier of the entity.
    /// </summary>
    string StringId { get; }
}

/// <summary>
/// Represents a versioned unique key for an entity, used for optimistic concurrency.
/// </summary>
public interface IEntityVersionedKey : IEntityKey
{
    /// <summary>
    /// Gets the expected version of the entity.
    /// </summary>
    string? Version { get; }
    
    /// <summary>
    /// Gets a value indicating whether the version must be matched during operations.
    /// </summary>
    bool VersionRequired { get; }
}