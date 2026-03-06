using InsonusK.Shared.DataBase.Models;

namespace InsonusK.Shared.Command.EntityLoading.Interfaces;

/// <summary>
/// Интерфейс для чтения данных контекста обработки Command.
/// Предоставляет доступ к сущностям, которые были загружены перед выполнением команды.
/// </summary>
public interface ICommandContext
{
    /// <summary>
    /// Получает сущность указанного типа из контекста.
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности (наследник EntityBase).</typeparam>
    /// <returns>Загруженная сущность.</returns>
    /// <exception cref="ArgumentException">Выбрасывается, если сущность указанного типа не найдена в контексте.</exception>
    TEntity Get<TEntity>() where TEntity : EntityBase;

    /// <summary>
    /// Пытается получить сущность указанного типа из контекста.
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности (наследник EntityBase).</typeparam>
    /// <param name="entity">Возвращаемая сущность, если найдена; иначе null.</param>
    /// <returns>True, если сущность найдена, иначе False.</returns>
    bool TryGet<TEntity>(out TEntity? entity) where TEntity : EntityBase;

    /// <summary>
    /// Проверяет, содержится ли сущность указанного типа в контексте.
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности (наследник EntityBase).</typeparam>
    /// <returns>True, если сущность присутствует в контексте, иначе False.</returns>
    bool Has<TEntity>() where TEntity : EntityBase;
}
