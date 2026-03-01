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

        if (Guid.TryParse(value, out guid))
            return true;
            
        return false;
    }
}