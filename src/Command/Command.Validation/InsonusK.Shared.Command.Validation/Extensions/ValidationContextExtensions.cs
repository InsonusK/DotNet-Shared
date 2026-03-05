using FluentValidation;
using InsonusK.Shared.Command.Validation.Interfaces;
using InsonusK.Shared.Command.Validation.Tools;
using InsonusK.Shared.DataBase.Models;
namespace InsonusK.Shared.Command.Validation.Extensions;

public static class ValidationContextExtensions
{
    private const string EntitiesKey = "ValidationEntitiesContext";

    public static void SetEntitiesContext(this IValidationContext context, ValidationEntitiesContext ctx)
        => context.RootContextData[EntitiesKey] = ctx;

    public static IValidationEntitiesReadContext GetEntitiesContext(this IValidationContext context)
    {
        if (context.RootContextData.TryGetValue(EntitiesKey, out var obj)
            && obj is ValidationEntitiesContext ctx)
            return ctx;
        throw new InvalidOperationException("ValidationEntitiesContext not found in RootContextData");
    }

    public static TEntity? GetEntity<TEntity>(this IValidationContext context)
        where TEntity : EntityBase
        => context.GetEntitiesContext().GetEntity<TEntity>();
}