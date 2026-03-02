namespace InsonusK.Shared.Mediator.CommandContext.Interfaces;

/// <summary>
/// Интерфейс для команды которая содержит информацию для контекста обработки
/// </summary>
public interface IEntityKey
{
    public string EntityStringId { get; }
    public Type EntityType { get; }
}
