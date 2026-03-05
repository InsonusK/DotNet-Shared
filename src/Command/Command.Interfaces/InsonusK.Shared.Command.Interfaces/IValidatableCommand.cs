using InsonusK.Shared.Command.Interfaces.Models;

namespace InsonusK.Shared.Command.Interfaces;

public interface IValidatableCommand
{
    IReadOnlyCollection<IEntityKey> EntityKeys { get; }
}
