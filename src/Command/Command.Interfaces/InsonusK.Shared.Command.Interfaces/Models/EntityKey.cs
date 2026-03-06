namespace InsonusK.Shared.Command.Interfaces.Models;

/// <summary>
/// Represents a concrete implementation of <see cref="IEntityVersionedKey"/> for a specific entity type.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public class EntityKey<TEntity> : IEntityVersionedKey where TEntity : class
{
    /// <summary>
    /// Gets the string identifier of the entity.
    /// </summary>
    public string StringId { get; }
    
    /// <summary>
    /// Gets the expected version of the entity.
    /// </summary>
    public string? Version { get; }    
    
    /// <summary>
    /// Gets a value indicating whether the version must be matched during operations.
    /// </summary>
    public bool VersionRequired { get; }

    /// <summary>
    /// Gets the type of the entity.
    /// </summary>
    public Type EntityType => typeof(TEntity);

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityKey{TEntity}"/> class.
    /// </summary>
    /// <param name="stringId">The string identifier of the entity.</param>
    /// <param name="version">The optional version of the entity.</param>
    /// <param name="versionRequired">Whether the version is required for operations.</param>
    public EntityKey(
        string stringId,
        string? version = null,
        bool versionRequired = false)
    {
        StringId = stringId;
        Version = version;
        VersionRequired = versionRequired;
    }

    /// <summary>
    /// Creates a new <see cref="EntityKey{TEntity}"/> with a required version.
    /// </summary>
    /// <param name="stringId">The string identifier of the entity.</param>
    /// <param name="version">The required version of the entity.</param>
    /// <returns>A new <see cref="EntityKey{TEntity}"/> instance.</returns>
    public static EntityKey<TEntity> WithVersion(string stringId, string version)
    {
        return new EntityKey<TEntity>(stringId, version, true);
    }

    /// <summary>
    /// Creates a new <see cref="EntityKey{TEntity}"/> without a version requirement.
    /// </summary>
    /// <param name="stringId">The string identifier of the entity.</param>
    /// <returns>A new <see cref="EntityKey{TEntity}"/> instance.</returns>
    public static EntityKey<TEntity> WithoutVersion(string stringId)
    {
        return new EntityKey<TEntity>(stringId, null, false);
    }
}