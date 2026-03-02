using InsonusK.Shared.DataBase.Models;
using InsonusK.Shared.Mediator.CommandContext.Interfaces;
using InsonusK.Shared.Models.Common;

namespace InsonusK.Shared.Mediator.CommandContext.Models;

public class EntityKeyWithVersion<TEntity> : EntityKey<TEntity>, IEntityKeyWithVersion where TEntity : EntityBase, IVersionatedModel
{
    public required uint Version { get; init; }
}
