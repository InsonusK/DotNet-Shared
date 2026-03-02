using InsonusK.Shared.Mediator.CommandContext.Interfaces;

namespace InsonusK.Shared.Mediator.CommandContext.Command;

/// <summary>
/// <para>Базовый Command, который содержит в себе ключи сущностей, которые должны быть извлечены и добавлены в CommandContext до выполнения Handler.</para>
/// <para>Сущности можно получить через <c>ICommandContext</c></para>
/// <para>Для использования необходимо:</para>
/// <list type="bullet">
///  <item><description>Наследовать конкретный Command от <c>CommandWithEntityKeys</c>.</description></item>
///  <item><description>Реализовать <c>IEntityCommandExtractor</c> для каждой сущности из IEntityKey</item>
/// </list>
/// </summary>
public abstract class CommandWithEntityKeys
{
    public abstract IEnumerable<IEntityKey> EntityKeys { get; init; }
}