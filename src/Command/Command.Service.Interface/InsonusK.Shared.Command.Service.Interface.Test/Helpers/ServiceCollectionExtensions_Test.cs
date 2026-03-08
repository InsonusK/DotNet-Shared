using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Divergic.Logging.Xunit;
using Xunit.Abstractions;
using InsonusK.Shared.Command.Service.Interface.Helpers;
using InsonusK.Shared.Command.Services.Interfaces;
using InsonusK.Shared.Command.Interfaces;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace InsonusK.Shared.Command.Service.Interface.Test.Helpers;

// Mocks for testing
public class TestEntity { }

public class ValidTestExtractor : IEntityCommandExtractor<TestEntity>
{
    public Task<TestEntity> GetAsync(IEntityKey entityKey, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new TestEntity());
    }
    
    public Task<TestEntity?> TryGetAsync(IEntityKey entityKey, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<TestEntity?>(new TestEntity());
    }
}

public abstract class AbstractTestExtractor : IEntityCommandExtractor<TestEntity>
{
    public abstract Task<TestEntity> GetAsync(IEntityKey entityKey, CancellationToken cancellationToken = default);
    public abstract Task<TestEntity?> TryGetAsync(IEntityKey entityKey, CancellationToken cancellationToken = default);
}

public interface IAnotherTestExtractor : IEntityCommandExtractor<TestEntity>
{
}

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class ServiceCollectionExtensions_Test : LoggingTestsBase<ServiceCollectionExtensions_Test>
{
    public ServiceCollectionExtensions_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
    {
    }

    /// <summary>
    /// description: Verifies that valid implementations from the provided assemblies are registered as scoped services.
    /// input: IServiceCollection, Assembly containing ValidTestExtractor
    /// output: IServiceCollection with registered service
    /// expected_result: The service collection contains the registration for ValidTestExtractor.
    /// </summary>
    [Fact]
    public void test_Add_WHEN_AssembliesProvided_AND_ContainsImplementations_THEN_RegistersImplementations()
    {
        #region Arrange
        Logger.LogDebug("Test ARRANGE");

        var services = new ServiceCollection();
        var assembly = Assembly.GetExecutingAssembly();
        
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        services.AddEntityCommandExtractor(null, assembly);

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        var serviceProvider = services.BuildServiceProvider();
        var extractor = serviceProvider.GetService<IEntityCommandExtractor<TestEntity>>();
        
        Assert.NotNull(extractor);
        Assert.IsType<ValidTestExtractor>(extractor);
        
        // Assert it's registered as scoped
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IEntityCommandExtractor<TestEntity>));
        Assert.NotNull(descriptor);
        Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);

        #endregion
    }

    /// <summary>
    /// description: Verifies that valid implementations from the calling assembly are registered when no assembly is explicitly provided.
    /// input: IServiceCollection, No explicit assemblies
    /// output: IServiceCollection with registered service
    /// expected_result: The service collection contains the registration from the calling assembly.
    /// </summary>
    [Fact]
    public void test_Add_WHEN_AssembliesNotProvided_THEN_UsesCallingAssembly()
    {
        #region Arrange
        Logger.LogDebug("Test ARRANGE");

        var services = new ServiceCollection();
        
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        services.AddEntityCommandExtractor(null); // No assemblies provided

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        var serviceProvider = services.BuildServiceProvider();
        var extractor = serviceProvider.GetService<IEntityCommandExtractor<TestEntity>>();
        
        Assert.NotNull(extractor);
        Assert.IsType<ValidTestExtractor>(extractor);

        #endregion
    }

    /// <summary>
    /// description: Verifies that abstract implementations or interfaces are skipped during registration.
    /// input: IServiceCollection, Assembly containing AbstractTestExtractor and IAnotherTestExtractor
    /// output: IServiceCollection without abstract registrations
    /// expected_result: The service collection does not contain a registration for AbstractTestExtractor or IAnotherTestExtractor.
    /// </summary>
    [Fact]
    public void test_Add_WHEN_AssemblyHasAbstractImplementation_THEN_SkipsRegistration()
    {
        #region Arrange
        Logger.LogDebug("Test ARRANGE");

        var services = new ServiceCollection();
        var assembly = Assembly.GetExecutingAssembly();
        
        #endregion

        #region Act
        Logger.LogDebug("Test ACT");

        services.AddEntityCommandExtractor(null, assembly);

        #endregion

        #region Assert
        Logger.LogDebug("Test ASSERT");

        // The count of descriptors for IEntityCommandExtractor<TestEntity> should be exactly 1 (from ValidTestExtractor)
        // because AbstractTestExtractor and IAnotherTestExtractor should be skipped.
        var descriptors = services.Where(d => d.ServiceType == typeof(IEntityCommandExtractor<TestEntity>)).ToList();
        
        Assert.Single(descriptors);
        Assert.Equal(typeof(ValidTestExtractor), descriptors[0].ImplementationType);

        #endregion
    }
}
