using InsonusK.Shared.Command.Interfaces;
using InsonusK.Shared.DataBase.Models;
using InsonusK.Shared.Models.Common;
using System.Collections.Generic;

namespace InsonusK.Shared.Command.Validation.Test.Mocks;

public class MockCommandWithBody : ICommandWithBody<string>
{
    public string Body { get; set; }

    public bool BodyRequired { get; set; }

    public Type BodyType => typeof(string);

    public object objBody => Body;
}

public class MockCommandWithEntityKeys : ICommandWithEntityKeys
{
    public IReadOnlyCollection<IEntityKey> EntityKeys { get; set; }
}



public class MockEntityModel : EntityBase
{
    public int Id { get; set; } = 2;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class MockVersionedEntityModel : EntityBase, IVersionatedModel
{
    public int Id { get; set; } = 1;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    public uint Version { get; set; } = 2;
}
