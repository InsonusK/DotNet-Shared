using Ardalis.GuardClauses;
using Ardalis.Specification;
using InsonusK.Shared.DataBase.Models;
using InsonusK.Shared.Models.Template;

namespace InsonusK.Shared.DataBase.Spec;
public class TodoTaskFetchSpec<TEntity> : Specification<TEntity> where TEntity:EntityBase
{
    public TodoTaskFetchSpec(IFetchRequest request)
    {
        if (request.Page >= 0 && request.PageSize > 0)
        {
            Query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize);
        }
    }
}