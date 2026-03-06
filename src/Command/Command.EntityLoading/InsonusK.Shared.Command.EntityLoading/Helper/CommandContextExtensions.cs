using Ardalis.GuardClauses;
using Ardalis.Result;
using InsonusK.Shared.Command.EntityLoading.Services;
using InsonusK.Shared.Command.EntityLoading.Tools;
using InsonusK.Shared.Command.Exceptions;
using InsonusK.Shared.Command.Interfaces;

namespace InsonusK.Shared.Command.EntityLoading.Helper;

public static class CommandContextExtensions
{
    /// <summary>
    /// Создает новый CommandContext для команды, загружая все сущности, указанные в EntityKeys команды.
    /// </summary>
    /// <param name="entityProvider">Провайдер сущностей, используемый для получения сущностей из базы.</param>
    /// <param name="command">Команда, содержащая ключи сущностей, которые требуется загрузить.</param>
    /// <param name="cancellationToken">Токен для отмены операции.</param>
    /// <returns>Заполненный CommandContext с загруженными сущностями.</returns>
    /// <exception cref="NotFoundException">Возникает, если хотя бы одна сущность не найдена в базе.</exception>
    public static async Task<CommandContext> CreateNewFor(this EntityProvider entityProvider, ICommandWithEntityKeys command, CancellationToken cancellationToken)
    {
        var context = new CommandContext();
        foreach (var entityKey in command.EntityKeys)
        {
            var entity = await entityProvider.Resolve(entityKey, cancellationToken);

            if (entity != null)
                context.AddEntity(entityKey.EntityType, entity);
            else
                throw new ResultException(Result.NotFound($"Entity of type {entityKey.EntityType.Name} with id {entityKey.StringId} not found"));
        }
        return context;
    }
}