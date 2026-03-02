using InsonusK.Shared.DataBase.Models;
using InsonusK.Shared.Mediator.CommandContext.Interfaces;

namespace InsonusK.Shared.Mediator.CommandContext.Models;
public class EntityKey<TEntity> : IEntityKey where TEntity : EntityBase
{
    public required string EntityStringId { get; init; }
    public Type EntityType => typeof(TEntity);
}