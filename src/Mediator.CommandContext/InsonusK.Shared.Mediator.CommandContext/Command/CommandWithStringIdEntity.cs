using InsonusK.Shared.DataBase.Models;
using InsonusK.Shared.Mediator.CommandContext.Interfaces;

namespace InsonusK.Shared.Mediator.CommandContext.Command;

/// <summary>
/// <para>Интерфейс для команды которая содержит информацию об id сущности для контекста обработки</para>
/// <para>Сущности можно получить через <c>ICommandContext</c></para>
/// <para>Для использования необходимо:</para>
/// <list type="bullet">
///  <item><description>Реализовать <c>IEntityCommandExtractor</c> для TEntity</item>
/// </list>
/// </summary>
/// <typeparam name="TEntity">Сушность, id которой содержится в команде</typeparam>
public abstract class CommandWithStringIdEntity<TEntity> : IEntityKey where TEntity : EntityBase
{
    public abstract string EntityStringId { get; init; }
    public Type EntityType => typeof(TEntity);
}
