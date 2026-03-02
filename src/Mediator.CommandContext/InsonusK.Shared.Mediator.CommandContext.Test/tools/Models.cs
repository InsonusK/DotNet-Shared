using InsonusK.Shared.DataBase.Models;
using InsonusK.Shared.Mediator.CommandContext.Command;
using InsonusK.Shared.Mediator.CommandContext.Interfaces;

namespace InsonusK.Shared.Mediator.CommandContext.Test;
public class TestEntity1 : EntityBase { }
public class TestEntity2 : EntityBase { }

public class TestEntityKey1 : IEntityKey
{
    public string EntityStringId { get; init; } = "1";
    public Type EntityType => typeof(TestEntity1);
}
public class TestEntityKey2 : IEntityKey
{
    public string EntityStringId { get; init; } = "2";
    public Type EntityType => typeof(TestEntity2);
}
public class TestCommand : CommandWithEntityKeys
{
    public override IEnumerable<IEntityKey> EntityKeys { get; init; } = new List<IEntityKey>();
}