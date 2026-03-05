using InsonusK.Shared.Command.Interfaces.Models;

namespace InsonusK.Shared.Command.Interfaces;

public interface ICommandWithEntityKeys
{
    IReadOnlyCollection<IEntityKey> EntityKeys { get; }
}
