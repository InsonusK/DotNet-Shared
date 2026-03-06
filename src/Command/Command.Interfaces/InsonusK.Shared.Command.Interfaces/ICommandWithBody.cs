namespace InsonusK.Shared.Command.Interfaces;

public interface ICommandWithBody
{
    Type BodyType { get; }
    object objBody { get; }
    bool BodyRequired { get; }
}
public interface ICommandWithBody<TBody>  : ICommandWithBody
{
    TBody Body { get; }
}