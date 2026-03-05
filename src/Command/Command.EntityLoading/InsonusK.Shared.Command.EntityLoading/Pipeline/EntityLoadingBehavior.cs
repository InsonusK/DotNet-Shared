using Ardalis.GuardClauses;
using InsonusK.Shared.Command.EntityLoading.Helper;
using InsonusK.Shared.Command.EntityLoading.Services;
using InsonusK.Shared.Command.EntityLoading.Tools;
using InsonusK.Shared.Command.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InsonusK.Shared.Command.EntityLoading.Pipeline;

/// <summary>
/// Конвейерное поведение для обработки команд (pipeline behavior).
/// Перехватывает выполнение команды, загружает необходимые сущности с помощью <see cref="EntityProvider"/>
/// и инициализирует <see cref="CommandContext"/> через <see cref="CommandContextManager"/> перед передачей управления дальше по конвейеру.
/// После завершения обработки команды контекст удаляется.
/// </summary>
/// <typeparam name="TRequest">Тип обрабатываемой команды.</typeparam>
/// <typeparam name="TResponse">Тип возвращаемого ответа.</typeparam>
internal class EntityLoadingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommandWithEntityKeys, IRequest
{
    private readonly ILogger _logger;
    private readonly CommandContextManager _commandContextManager;
    private readonly EntityProvider _entityProvider;

    public EntityLoadingBehavior(
        ILogger<EntityLoadingBehavior<TRequest, TResponse>> logger,
        CommandContextManager commandContextManager,
        EntityProvider entityProvider)
    {
        _logger = logger;
        _commandContextManager = commandContextManager;
        _entityProvider = entityProvider;
    }

    /// <summary>
    /// Обрабатывает команду в конвейере.
    /// </summary>
    /// <param name="request">Запрос (команда).</param>
    /// <param name="next">Делегат следующего обработчика.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Ответ от следующего обработчика.</returns>
    /// <exception cref="NotFoundException">Выбрасывается (из <see cref="EntityProvider"/>), если запрашиваемая сущность не найдена в базе данных.</exception>
    /// <exception cref="InvalidOperationException">Выбрасывается (из <see cref="CommandContextManager"/>), если возникает конфликт состояний контекста.</exception>
    /// <exception cref="Exception">Может выбрасывать любые исключения, возникающие в последующих обработчиках конвейера (<paramref name="next"/>), включая исключения бизнес-логики и базы данных.</exception>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            var newContext = await _entityProvider.CreateNewFor(request, cancellationToken);
            _commandContextManager.StartFor(request, newContext);
            
            return await next();
        }
        finally
        {
            _commandContextManager.EndFor(request);
        }

    }

}