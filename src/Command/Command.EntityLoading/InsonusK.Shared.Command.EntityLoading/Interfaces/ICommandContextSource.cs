using InsonusK.Shared.Command.Interfaces;
using MediatR;

namespace InsonusK.Shared.Command.EntityLoading.Interfaces;

/// <summary>
/// Интерфейс источника контекста обработки команды.
/// Предназначен для получения контекста сущностей, загруженных для выполнения команды.
/// </summary>
public interface ICommandContextSource
{
    /// <summary>
    /// Получает контекст команды с предварительно загруженными сущностями для указанной команды.
    /// </summary>
    /// <typeparam name="TRequest">Тип команды, реализующий ICommandWithEntityKeys и IRequest.</typeparam>
    /// <param name="command">Команда, для которой необходимо получить контекст.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Контекст команды, содержащий загруженные сущности.</returns>
    /// <exception cref="InvalidOperationException">Возможна ошибка, если контекст не был создан или не найден.</exception>
    Task<ICommandContext> GetForAsync<TRequest>(TRequest command, CancellationToken cancellationToken = default) where TRequest : ICommandWithEntityKeys, IRequest;
}