using InsonusK.Shared.Models.Common;

namespace InsonusK.Shared.Mediator.CommandContext.Interfaces;

/// <summary>
/// Интерфейс для контейнера который содержит информацию для контекста обработки
/// </summary>
/// <typeparam name="TEntity">Тип сущности</typeparam>
public interface IEntityKeyWithVersion : IEntityKey, IVersionatedModel
{
}