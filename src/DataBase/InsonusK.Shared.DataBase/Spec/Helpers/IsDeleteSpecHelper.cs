using Ardalis.Specification;
using InsonusK.Shared.DataBase.Models;

namespace InsonusK.Shared.DataBase.Spec.Helpers;

public static class IsDeleteSpecHelper
{
    public static ISpecificationBuilder<T> IsNotDeleted<T>(this ISpecificationBuilder<T> query) where T : IDeleteStatusEntity => query.Where(x => x.IsDeleted == false);
    public static ISpecificationBuilder<T> IsDeleted<T>(this ISpecificationBuilder<T> query) where T : IDeleteStatusEntity => query.Where(x => x.IsDeleted);
}