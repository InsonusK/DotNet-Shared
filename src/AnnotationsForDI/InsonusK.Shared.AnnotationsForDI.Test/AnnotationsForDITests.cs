using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace InsonusK.Shared.AnnotationsForDI.Test;

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

        var provider = services.BuildServiceProvider();
        var service = provider.GetService<ITestService>();
        Assert.NotNull(service);
        Assert.IsType<TestService>(service);
    }

    // Helper types for testing
    [Service]
    private class TestService : ITestService { }

    private interface ITestService { }
}
