namespace InsonusK.Shared.Models.Tools;

public static class ClientActionTimeStampParser
{
    public static DateTimeOffset ParseUserActionTimeStamp(this string clientActionTimeStampStr)
    {
        if (string.IsNullOrWhiteSpace(clientActionTimeStampStr))
            return default;

        if (DateTimeOffset.TryParse(clientActionTimeStampStr, out var clientActionTimestamp))
            return clientActionTimestamp;

        return default;

    }
}
