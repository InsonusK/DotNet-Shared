using Divergic.Logging.Xunit;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using InsonusK.Shared.DataBase.Models;

namespace InsonusK.Shared.DataBase.Test.Models;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class ConstantGuidEntity_Test : LoggingTestsBase<ConstantGuidEntity_Test>
{
    public ConstantGuidEntity_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
    {
    }

    [Fact]
    public void test_IsEqualStringId_WHEN_string_matches_guid__THEN_returns_true()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var asserted_entity = new TestEntity();
        Guid expected_guid = Guid.NewGuid();
        asserted_entity.Guid = expected_guid;

        #endregion


        #region Act
        Logger.LogDebug("Test ACT");

        bool asserted_result = asserted_entity.IsEqualStringId(expected_guid.ToString());

        #endregion


        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.True(asserted_result);

        #endregion
    }

    [Fact]
    public void test_IsEqualStringId_WHEN_string_matches_int_id__THEN_returns_true()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var asserted_entity = new TestEntity { Id = 7 };

        #endregion


        #region Act
        Logger.LogDebug("Test ACT");

        bool asserted_result = asserted_entity.IsEqualStringId("7");

        #endregion


        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.True(asserted_result);

        #endregion
    }

    [Fact]
    public void test_IsEqualStringId_WHEN_string_matches_neither_guid_nor_id__THEN_returns_false()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");
        
        var asserted_entity = new TestEntity { Id = 5, Guid = Guid.NewGuid() };

        #endregion


        #region Act
        Logger.LogDebug("Test ACT");

        bool asserted_result = asserted_entity.IsEqualStringId("999");

        #endregion


        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.False(asserted_result);

        #endregion
    }

    [Fact]
    public void test_Guid_WHEN_setting_guid_on_existing_entity_with_nonzero_id__THEN_throws_ApplicationException()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var asserted_entity = new TestEntity { Id = 10, Guid = Guid.NewGuid() };
        

        #endregion


        #region Act & Assert
        Logger.LogDebug("Test ACT");

        Assert.Throws<ApplicationException>(() => asserted_entity.Guid = Guid.NewGuid());

        #endregion
    }
}
