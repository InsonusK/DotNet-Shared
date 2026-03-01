using Microsoft.Extensions.DependencyInjection;

namespace InsonusK.Shared.AnnotationsForDI;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class ServiceAttribute : Attribute
{
    public ServiceLifetime Lifetime { get; }
    public Type[] Interfaces { get; }

    /// <summary>
    /// Атрибут регистрации сервисов по атрибуту ServiceAttribute
    /// </summary>
    /// <param name="lifetime">Время жизни сервиса</param>
    /// <param name="interfaces">Массив интерфейсов, которые будет реализованы сервисом</param>
    public ServiceAttribute(
        ServiceLifetime lifetime = ServiceLifetime.Scoped,
        params Type[] interfaces)
    {
        Lifetime = lifetime;
        Interfaces = interfaces ?? Array.Empty<Type>();
    }
}