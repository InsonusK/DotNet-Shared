using InsonusK.Shared.Command.Interfaces;

namespace InsonusK.Shared.Command.EntityLoading.Interfaces;

/// <summary>
/// <para>Интерфейс для извлечения сущности из команды для контекста обработки</para>
/// <para>Для использования необходимо:</para>
/// <para>- Реализовать <c>IEntityCommandExtractor</c> для TEntity</para>
/// <para>- Зарегистрировать реализацию в DI контейнере. Можно через <c>[Service(interfaces: typeof(IEntityCommandExtractor&lt;TEntity&gt;))]</c></para>
/// <para>В классах получать <c>ICommandContext</c> который будет содержать извлечённую сущность</para>
/// </summary>
/// <typeparam name="TEntity">Тип сущности</typeparam>
public interface IEntityCommandExtractor<TEntity> where TEntity : class
{
    public Task<TEntity> GetAsync(IEntityKey entityKey, CancellationToken cancellationToken = default);
}
