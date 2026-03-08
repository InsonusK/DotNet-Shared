using Ardalis.Specification;

namespace InsonusK.Shared.DataBase.Spec.Helpers;

public static class EmptySpecHelper
{
    public static ISpecificationBuilder<T> EmptyResult<T>(this ISpecificationBuilder<T> query) => query.Where(_ => false);
}
