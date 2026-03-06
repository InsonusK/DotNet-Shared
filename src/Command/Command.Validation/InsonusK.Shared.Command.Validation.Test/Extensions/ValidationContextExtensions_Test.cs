using Divergic.Logging.Xunit;
using FluentValidation;
using InsonusK.Shared.Command.EntityLoading.Interfaces;
using InsonusK.Shared.Command.Validation.Extensions;
using InsonusK.Shared.DataBase.Models;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit.Abstractions;

namespace InsonusK.Shared.Command.Validation.Test.Extensions;

public class TestCommand { }

public class TestEntity : EntityBase
{
    
}
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class ValidationContextExtensions_Test : LoggingTestsBase<ValidationContextExtensions_Test>
{
    public ValidationContextExtensions_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
    {
    }

    #region SetEntitiesContext

    [Fact]
    public void SetEntitiesContext_WHEN_called_THEN_ICommandContext_is_added_to_RootContextData()
    {
        Logger.LogDebug("Test ARRAY");
        var context = new ValidationContext<TestCommand>(new TestCommand());
        var cmdCtxMock = Substitute.For<ICommandContext>();

        Logger.LogDebug("Test ACT");
        context.SetEntitiesContext(cmdCtxMock);

        Logger.LogDebug("Test ASSERT");
        Assert.True(context.RootContextData.ContainsKey("ValidationEntitiesContext"));
        Assert.Equal(cmdCtxMock, context.RootContextData["ValidationEntitiesContext"]);
    }

    #endregion

    #region GetEntitiesContext

    [Fact]
    public void GetEntitiesContext_WHEN_EntitiesKey_exists_in_RootContextData_THEN_returns_ICommandContext()
    {
        Logger.LogDebug("Test ARRAY");
        var context = new ValidationContext<TestCommand>(new TestCommand());
        var cmdCtxMock = Substitute.For<ICommandContext>();
        context.RootContextData["ValidationEntitiesContext"] = cmdCtxMock;

        Logger.LogDebug("Test ACT");
        var result = context.GetEntitiesContext();

        Logger.LogDebug("Test ASSERT");
        Assert.Equal(cmdCtxMock, result);
    }

    [Fact]
    public void GetEntitiesContext_WHEN_EntitiesKey_does_not_exist_in_RootContextData_THEN_throws_InvalidOperationException()
    {
        Logger.LogDebug("Test ARRAY");
        var context = new ValidationContext<TestCommand>(new TestCommand());

        Logger.LogDebug("Test ACT");
        var act = () => context.GetEntitiesContext();

        Logger.LogDebug("Test ASSERT");
        var exception = Assert.Throws<InvalidOperationException>(act);
        Assert.Equal("ValidationEntitiesContext not found in RootContextData", exception.Message);
    }

    [Fact]
    public void GetEntitiesContext_WHEN_EntitiesKey_has_wrong_type_THEN_throws_InvalidOperationException()
    {
        Logger.LogDebug("Test ARRAY");
        var context = new ValidationContext<TestCommand>(new TestCommand());
        context.RootContextData["ValidationEntitiesContext"] = "Not a ICommandContext";

        Logger.LogDebug("Test ACT");
        var act = () => context.GetEntitiesContext();

        Logger.LogDebug("Test ASSERT");
        var exception = Assert.Throws<InvalidOperationException>(act);
        Assert.Equal("ValidationEntitiesContext not found in RootContextData", exception.Message);
    }

    #endregion

    #region GetEntity

    [Fact]
    public void GetEntity_WHEN_called_THEN_calls_Get_on_EntitiesContext_and_returns_result()
    {
        Logger.LogDebug("Test ARRAY");
        var context = new ValidationContext<TestCommand>(new TestCommand());
        var cmdCtxMock = Substitute.For<ICommandContext>();

        var expectedEntity = new TestEntity { Id = 1 };
        cmdCtxMock.Get<TestEntity>().Returns(expectedEntity);

        context.RootContextData["ValidationEntitiesContext"] = cmdCtxMock;

        Logger.LogDebug("Test ACT");
        var result = context.GetEntity<TestEntity>();

        Logger.LogDebug("Test ASSERT");
        Assert.Equal(expectedEntity, result);
        cmdCtxMock.Received(1).Get<TestEntity>();
    }

    #endregion
}
