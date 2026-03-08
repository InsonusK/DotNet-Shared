using System.Reflection;
using Ardalis.Specification;
using InsonusK.Shared.Command.EntityLoading.Helper;
using InsonusK.Shared.Command.EntityLoading.Interfaces;
using InsonusK.Shared.Command.EntityLoading.Tools;
using InsonusK.Shared.Command.Interfaces;
using InsonusK.Shared.Command.Services.Interfaces;
using InsonusK.Shared.DataBase.Models;
using InsonusK.Shared.DataBase.Spec;
using InsonusK.Shared.Models.Common;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InsonusK.Shared.Command.EntityLoading.Services;

/// <summary>
/// Провайдер сущностей. Отвечает за извлечение и создание контекста сущностей (<see cref="CommandContext"/>) для команды.
/// Реализует паттерн получения сущностей через кастомные извлекатели или стандартные репозитории (по Guid или int).
/// </summary>
public class EntityProvider : ICommandContextSource
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger _logger;

    public EntityProvider(
        IServiceProvider serviceProvider,
        ILogger<EntityProvider> logger)
    {
        _serviceProvider = serviceProvider;
        this._logger = logger;
    }
    /// <summary>
    /// Динамически получает сущность неизвестного на этапе компиляции типа на основе ключа.
    /// </summary>
    /// <param name="entityKey">Ключ сущности.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Загруженная сущность в виде object или null.</returns>
    /// <exception cref="TargetInvocationException">Выбрасывается (внутри метода Invoke), если метод <see cref="Resolve{TEntity}"/> завершается с ошибкой.</exception>
    public async Task<object?> Resolve(IEntityKey entityKey, CancellationToken ct)
    {
        var method = typeof(EntityProvider)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name == nameof(EntityProvider.Resolve))
            .Single(m => m.IsGenericMethodDefinition);
        var genericMethod = method!.MakeGenericMethod(entityKey.EntityType);

        var entity = await this.InvokeGenericAsync(genericMethod, entityKey, ct);
        return entity;
    }
    
    /// <summary>
    /// Получает сущность известного типа на основе ключа, используя IEntityCommandExtractor, Guid или int репозитории.
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности.</typeparam>
    /// <param name="entityKey">Ключ сущности.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Загруженная сущность или null.</returns>
    /// <exception cref="TargetInvocationException">Выбрасывается при ошибках динамического вызова внутренних методов (например, <c>ResolveInt</c> или <c>ResolveGuid</c>).</exception>
    /// <exception cref="Exception">Может выбросить любое исключение, возникающее в реализации <see cref="IEntityCommandExtractor{TEntity}"/>.</exception>
    public async Task<TEntity?> Resolve<TEntity>(IEntityKey entityKey, CancellationToken ct) where TEntity : class
    {
        var entityExtractor = _serviceProvider.GetService<IEntityCommandExtractor<TEntity>>();
        if (entityExtractor != null)
        {
            _logger.LogInformation("Resolving entity of type {EntityType} with string id {StringId} using custom extractor", typeof(TEntity).Name, entityKey.StringId);
            return await entityExtractor.TryGetAsync(entityKey, ct);
        }
        else if (typeof(IGuidModel).IsAssignableFrom(typeof(TEntity)))
        {
            _logger.LogInformation("Resolving entity of type {EntityType} with string id {StringId} using Guid resolver", typeof(TEntity).Name, entityKey.StringId);
            var method = typeof(EntityProvider)
                .GetMethod(nameof(ResolveGuid), BindingFlags.NonPublic | BindingFlags.Instance)!
                .MakeGenericMethod(typeof(TEntity));
            return (TEntity?)await this.InvokeGenericAsync(method, entityKey, ct)!;
        }
        else if (typeof(EntityBase).IsAssignableFrom(typeof(TEntity)))
        {
            _logger.LogInformation("Resolving entity of type {EntityType} with string id {StringId} using int resolver", typeof(TEntity).Name, entityKey.StringId);
            var method = typeof(EntityProvider)
                    .GetMethod(nameof(ResolveInt), BindingFlags.NonPublic | BindingFlags.Instance)!
                    .MakeGenericMethod(typeof(TEntity));

            return (TEntity?)await this.InvokeGenericAsync(method, entityKey, ct)!;
        }
        else
        {
            _logger.LogWarning("Cannot resolve entity of type {EntityType} with string id {StringId} because it does not implement Custom Extractor, IGuidModel or EntityBase", typeof(TEntity).Name, entityKey.StringId);
            return null;
        }
    }

    private async Task<TEntity?> ResolveInt<TEntity>(IEntityKey entityKey, CancellationToken ct) where TEntity : EntityBase
    {
        var repo = _serviceProvider.GetRequiredService<IReadRepositoryBase<TEntity>>();
        if (!int.TryParse(entityKey.StringId, out var intId))
            return null;
        var entity = await repo.GetByIdAsync(intId, ct);
        return entity;
    }

    private async Task<TEntity?> ResolveGuid<TEntity>(IEntityKey entityKey, CancellationToken ct) where TEntity : EntityBase, IGuidModel
    {
        var repo = _serviceProvider.GetRequiredService<IReadRepositoryBase<TEntity>>();
        var spec = new ByStringIdSpec<TEntity>(entityKey.StringId);
        var entity = await repo.SingleOrDefaultAsync(spec, ct);
        return entity;
    }

    /// <summary>
    /// Создает новый контекст для команды, заполняя его необходимыми сущностями.
    /// </summary>
    /// <typeparam name="TRequest">Тип команды.</typeparam>
    /// <param name="command">Команда.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Готовый контекст с сущностями.</returns>
    /// <exception cref="NotFoundException">Выбрасывается (из <see cref="CommandContextExtensions.CreateNewFor"/>), если хотя бы одна сущность не найдена в хранилище.</exception>
    public async Task<ICommandContext> GetForAsync<TRequest>(TRequest command, CancellationToken cancellationToken) where TRequest : ICommandWithEntityKeys, IBaseRequest
    {
        return await this.CreateNewFor(command, cancellationToken);
    }
}