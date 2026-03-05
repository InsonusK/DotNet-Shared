using InsonusK.Shared.DataBase.Models;

namespace InsonusK.Shared.Command.EntityLoading.Interfaces;

/// <summary>
/// Интерфейс для чтения данных контекста обработки Command
/// </summary>
public interface ICommandContext
{
    TEntity Get<TEntity>() where TEntity : EntityBase;
    bool TryGet<TEntity>(out TEntity? entity) where TEntity : EntityBase;
    bool Has<TEntity>() where TEntity : EntityBase;
}
