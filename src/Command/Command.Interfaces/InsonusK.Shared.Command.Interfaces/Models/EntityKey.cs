namespace InsonusK.Shared.Command.Interfaces.Models;

public class EntityKey<TEntity> : IEntityKey where TEntity : class
{
    public string StringId { get; }
    public string? Version { get; }
    public bool VersionRequired { get; }

    public Type EntityType => typeof(TEntity);

    public EntityKey(
        string stringId,
        string? version = null,
        bool versionRequired = false)
    {
        StringId = stringId;
        Version = version;
        VersionRequired = versionRequired;
    }

    public static EntityKey<TEntity> WithVersion(string stringId, string version)
    {
        return new EntityKey<TEntity>(stringId, version, true);
    }

    public static EntityKey<TEntity> WithoutVersion(string stringId)
    {
        return new EntityKey<TEntity>(stringId, null, false);
    }
}