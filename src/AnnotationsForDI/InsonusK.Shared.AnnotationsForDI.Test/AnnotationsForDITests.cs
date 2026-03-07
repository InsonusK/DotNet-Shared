using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace InsonusK.Shared.AnnotationsForDI.Test;
public interface ITestService { }
[Service(ServiceLifetime.Scoped, new[] { typeof(ITestService) })]
public class TestService : ITestService { }
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class AnnotationsForDITests
{
    [Fact]
    public void ServiceAttribute_CanBeConstructed_WithDefaultValues()
    {
        var attr = new ServiceAttribute();
        Assert.NotNull(attr);
    }

    [Fact]
    public void ServiceAttribute_CanBeAppliedToClass()
    {
        var type = typeof(TestService);
        var attr = type.GetCustomAttribute<ServiceAttribute>();
        Assert.NotNull(attr);
    }

    [Fact]
    public void ServiceCollectionExtensions_AddAnnotatedServices_RegistersService()
    {
        var services = new ServiceCollection();
        services.AddAnnotatedServices(typeof(TestService).Assembly);

        var provider = services.BuildServiceProvider().CreateScope().ServiceProvider;
        var service = provider.GetService<ITestService>();
        Assert.NotNull(service);
        Assert.IsType<TestService>(service);
    }
}
