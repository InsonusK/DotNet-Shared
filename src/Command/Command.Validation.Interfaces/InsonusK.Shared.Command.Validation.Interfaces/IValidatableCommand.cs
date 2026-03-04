namespace InsonusK.Shared.Command.Validation.Interfaces;

public interface IValidatableCommand
{
    IReadOnlyDictionary<Type, string> EntityKeys { get; }
}
