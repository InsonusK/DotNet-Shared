namespace InsonusK.Shared.Models.Tools;
public static class StringIdExtension
{
    public static bool ToId(this string value, out int id, out Guid guid)
    {
        id=-1;
        guid = Guid.Empty;
        if (string.IsNullOrWhiteSpace(value))
            return false;
        
        if (int.TryParse(value, out id))
            return true;
        else id=-1;

        if (Guid.TryParse(value, out guid))
            return true;
        else guid = Guid.Empty;

        return false;
    }
}