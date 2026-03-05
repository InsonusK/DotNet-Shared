using System.Collections.Concurrent;
using System.Windows.Input;
using InsonusK.Shared.Command.EntityLoading.Interfaces;
using InsonusK.Shared.Command.EntityLoading.Tools;
using InsonusK.Shared.Command.Interfaces;
using MediatR;

namespace InsonusK.Shared.Command.EntityLoading.Services;

/// <summary>
/// Менеджер для управления жизненным циклом <see cref="CommandContext"/> для обрабатываемых команд.
/// Сохраняет контекст для конкретной команды (IBaseRequest), чтобы к нему можно было обращаться во время выполнения конвейера (pipeline).
/// </summary>
internal class CommandContextManager : ICommandContextSource
{
    private readonly ConcurrentDictionary<IBaseRequest, CommandContext> _commandContext = new();

    /// <summary>
    /// Запускает отслеживание указанного контекста для заданной команды.
    /// </summary>
    /// <typeparam name="TRequest">Тип команды.</typeparam>
    /// <param name="command">Команда, для которой создается контекст.</param>
    /// <param name="context">Готовый контекст с загруженными сущностями.</param>
    /// <exception cref="InvalidOperationException">Выбрасывается, если контекст для этой команды уже существует.</exception>
    public void StartFor<TRequest>(TRequest command, CommandContext context) where TRequest : ICommandWithEntityKeys, IBaseRequest
    {
        if (!_commandContext.TryAdd(command, context))
            throw new InvalidOperationException("A context for this command already exists");
    }
    /// <summary>
    /// Создает и запускает отслеживание нового пустого контекста для заданной команды.
    /// </summary>
    /// <typeparam name="TRequest">Тип команды.</typeparam>
    /// <param name="command">Команда, для которой создается контекст.</param>
    /// <returns>Новый пустой контекст.</returns>
    public CommandContext StartFor<TRequest>(TRequest command) where TRequest : ICommandWithEntityKeys, IBaseRequest
    {
        var context = new CommandContext();
        StartFor(command, context);
        return context;
    }
    
    /// <summary>
    /// Завершает отслеживание контекста для заданной команды и удаляет его.
    /// </summary>
    /// <typeparam name="TRequest">Тип команды.</typeparam>
    /// <param name="command">Команда, для которой удаляется контекст.</param>
    /// <exception cref="InvalidOperationException">Выбрасывается, если контекст для этой команды не найден.</exception>
    public void EndFor<TRequest>(TRequest command) where TRequest : ICommandWithEntityKeys, IBaseRequest
    {
        if (!_commandContext.TryRemove(command, out _))
            throw new InvalidOperationException("No context found for this command");
    }

    public async Task<ICommandContext> GetForAsync<TRequest>(TRequest command, CancellationToken cancellationToken = default) where TRequest : ICommandWithEntityKeys, IBaseRequest
    {
        return _commandContext[command];
    }
}