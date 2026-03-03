using Ardalis.GuardClauses;
using Ardalis.Specification;
using InsonusK.Shared.DataBase.Models;

namespace InsonusK.Shared.DataBase.Spec;

public class ByStringIdSpec<T> : SingleResultSpecification<T>, ISingleResultSpecification<T> where T : ConstantGuidEntity
{
    public readonly bool QueryIsEmpty = false;
    public static bool TryBuild(string stringId, out ByStringIdSpec<T> spec)
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        spec = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        if (Guid.TryParse(stringId, out var guid))
        {
            spec = new ByStringIdSpec<T>(guid);
        }
        else if (int.TryParse(stringId, out var intId))
        {
            spec = new ByStringIdSpec<T>(intId);
        }

        return spec != null;
    }
    public ByStringIdSpec(Guid guidId)
    {
        Query.Where(x => x.Guid == guidId);
    }
    public ByStringIdSpec(int Id)
    {
        Query.Where(x => x.Id == Id);
    }
    public ByStringIdSpec(string stringId, bool tryParse = true)
    {
        if (Guid.TryParse(stringId, out var guid))
        {
            Query.Where(x => x.Guid == guid);
        }
        else if (int.TryParse(stringId, out var intId))
        {
            Query.Where(x => x.Id == intId);
        }
        else if (tryParse)
        {
            Query.Where(x => false);
            this.QueryIsEmpty = true;
        }
        else
            throw new ArgumentException($"String id is not Guid or Int: {stringId}");
    }

}

public class ByStringIdsSpec<T> : Specification<T> where T : ConstantGuidEntity
{
    private List<string> _wrongFormatList = new List<string>();
    public string[] WrongFormatList => _wrongFormatList.ToArray();
    public bool WrongFormat => _wrongFormatList.Count > 0;
    public ByStringIdsSpec(IEnumerable<Guid> guids)
    {
        Guard.Against.NullOrEmpty(guids, nameof(guids));
        Query.Where(x => guids.Contains(x.Guid));
    }

    public ByStringIdsSpec(IEnumerable<int> Ids)
    {
        Guard.Against.NullOrEmpty(Ids, nameof(Ids));
        Query.Where(x => Ids.Contains(x.Id));
    }

    public ByStringIdsSpec(IEnumerable<string> stringIds)
    {
        Guard.Against.NullOrEmpty(stringIds, nameof(stringIds));
        List<Guid> guids = new();
        List<int> intIds = new();
        foreach (var stringId in stringIds)
        {
            if (Guid.TryParse(stringId, out var guid))
            {
                guids.Add(guid);
            }
            else if (int.TryParse(stringId, out var intId))
            {
                intIds.Add(intId);
            }
            else
            {
                _wrongFormatList.Add(stringId);
            }
        }

        if (guids.Count > 0 && intIds.Count > 0)
            Query.Where(x => guids.Contains(x.Guid) || intIds.Contains(x.Id));
        else if (guids.Count > 0)
            Query.Where(x => guids.Contains(x.Guid));
        else if (intIds.Count > 0)
            Query.Where(x => intIds.Contains(x.Id));
        else
            throw new ArgumentException($"String ids are not Guid or Int: {stringIds}");
    }

}