using InsonusK.Shared.Command.EntityLoading.Interfaces;
using InsonusK.Shared.DataBase.Models;

namespace InsonusK.Shared.Command.EntityLoading.Tools;


/// <summary>
/// Контейнер данных контекста обработки Command
/// </summary>
public class CommandContext : ICommandContext
{
    private Dictionary<Type, object> _entities = new Dictionary<Type, object>();

    /// <summary>
    /// Добавляет сущность неопределенного типа в контекст.
    /// </summary>
    /// <param name="entityType">Тип добавляемой сущности.</param>
    /// <param name="entity">Сам объект сущности.</param>
    /// <exception cref="ArgumentException">Выбрасывается, если сущность такого типа уже добавлена.</exception>
    public void AddEntity(Type entityType, object entity)
    {
        if (_entities.ContainsKey(entityType))
            throw new ArgumentException($"Another Entity {entityType} has been already added");
        _entities[entityType] = entity!;
    }
    /// <summary>
    /// Добавляет сущность типа EntityBase в контекст.
    /// </summary>
    /// <typeparam name="TEntity">Тип добавляемой сущности (наследник EntityBase).</typeparam>
    /// <param name="entity">Объект сущности.</param>
    public void Add<TEntity>(TEntity entity) where TEntity : EntityBase
    {
        AddEntity(typeof(TEntity), entity);
    }

    /// <summary>
    /// Получает сущность указанного типа из контекста.
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности (наследник EntityBase).</typeparam>
    /// <returns>Найденная сущность.</returns>
    /// <exception cref="ArgumentException">Выбрасывается, если сущность указанного типа не найдена.</exception>
    public TEntity Get<TEntity>() where TEntity : EntityBase
    {
        if (_entities.TryGetValue(typeof(TEntity), out var entity))
            return (TEntity)entity;
        throw new ArgumentException($"Entity {typeof(TEntity).Name} not found");
    }

    /// <summary>
    /// Пытается получить сущность указанного типа из контекста.
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности (наследник EntityBase).</typeparam>
    /// <param name="entity">Возвращаемая сущность, если найдена; иначе null.</param>
    /// <returns>True, если сущность найдена, иначе False.</returns>
    public bool TryGet<TEntity>(out TEntity? entity) where TEntity : EntityBase
    {
        if (_entities.TryGetValue(typeof(TEntity), out var _entity))
        {
            entity = (TEntity)_entity;
            return true;
        }
        entity = null;
        return false;
    }

    /// <summary>
    /// Проверяет, содержится ли сущность указанного типа в контексте.
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности (наследник EntityBase).</typeparam>
    /// <returns>True, если сущность присутствует в контексте, иначе False.</returns>
    public bool Has<TEntity>() where TEntity : EntityBase
    {
        return _entities.ContainsKey(typeof(TEntity));
    }

   
}