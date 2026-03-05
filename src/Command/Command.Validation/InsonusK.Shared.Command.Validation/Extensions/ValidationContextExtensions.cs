using FluentValidation;
using InsonusK.Shared.Command.EntityLoading.Interfaces;
using InsonusK.Shared.DataBase.Models;
namespace InsonusK.Shared.Command.Validation.Extensions;

public static class ValidationContextExtensions
{
    private const string EntitiesKey = "ValidationEntitiesContext";

    internal static void SetEntitiesContext(this IValidationContext context, ICommandContext ctx)
        => context.RootContextData[EntitiesKey] = ctx;

    public static ICommandContext GetEntitiesContext(this IValidationContext context)
    {
        if (context.RootContextData.TryGetValue(EntitiesKey, out var obj)
            && obj is ICommandContext ctx)
            return ctx;
        throw new InvalidOperationException("ValidationEntitiesContext not found in RootContextData");
    }

    public static TEntity? GetEntity<TEntity>(this IValidationContext context)
        where TEntity : EntityBase
        => context.GetEntitiesContext().Get<TEntity>();
}