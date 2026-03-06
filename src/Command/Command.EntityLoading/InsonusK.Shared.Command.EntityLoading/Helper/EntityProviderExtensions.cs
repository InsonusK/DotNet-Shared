using System.Reflection;

namespace InsonusK.Shared.Command.EntityLoading.Helper;

public static class EntityProviderExtensions
{
    public static async Task<object?> InvokeGenericAsync(this object obj, MethodInfo method, params object[] parameters)
    {
        dynamic dynTask = method.Invoke(obj, parameters);
        return await dynTask;
    }
}