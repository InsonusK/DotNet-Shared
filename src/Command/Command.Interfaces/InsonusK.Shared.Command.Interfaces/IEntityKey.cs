namespace InsonusK.Shared.Command.Interfaces;

public interface IEntityType
{
    Type EntityType { get; }
}

public interface IEntityKey : IEntityType
{
    string StringId { get; }
}

public interface IEntityVersionedKey : IEntityKey
{
    string? Version { get; }
    bool VersionRequired { get; }
}