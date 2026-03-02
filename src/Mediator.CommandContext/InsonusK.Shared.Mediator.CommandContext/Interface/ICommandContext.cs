using InsonusK.Shared.DataBase.Models;

namespace InsonusK.Shared.Mediator.CommandContext.Interfaces;

/// <summary>
/// Интерфейс для контекста обработки команды, который позволяет извлекать сущности, которые были добавлены в контекст до выполнения Handler.
/// </summary>
public interface ICommandContext
{
    public TEntity Entity<TEntity>() where TEntity : EntityBase;
    public bool TryGetEntity<TEntity>(out TEntity? entity) where TEntity : EntityBase;
}
