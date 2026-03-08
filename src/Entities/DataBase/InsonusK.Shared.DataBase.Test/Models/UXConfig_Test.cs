using InsonusK.Shared.DataBase.Models;

namespace InsonusK.Shared.DataBase.Test.Models;


[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class IndexConfig_Test
{
    [Fact]
    public void test_IndexName_WHEN_field_ordered_THEN_return_correct_indexName()
    {
        // Arrange
        var config = new IndexConfig<TestEntity>
        {
            Fields = new[] { "Field1", "Field2" }
        };
        var expectedName = "UX_TestEntity_Field1_Field2";

        // Act
        var actualName = config.IndexName;

        // Assert
        Assert.Equal(expectedName, actualName);
    }

    [Fact]
    public void test_IndexName_WHEN_field_worng_ordered_THEN_return_correct_indexName()
    {
        // Arrange
        var config = new IndexConfig<TestEntity>
        {
            Fields = new[] { "Field2", "Field1" }
        };
        var expectedName = "UX_TestEntity_Field1_Field2";

        // Act
        var actualName = config.IndexName;

        // Assert
        Assert.Equal(expectedName, actualName);
    }

    [Fact]
    public void test_IndexName_WHEN_not_unique_THEN_return_ix_indexName()
    {
        // Arrange
        var config = new IndexConfig<TestEntity>
        {
            Fields = new[] { "Field2", "Field1" },
            IsUnique = false
        };
        var expectedName = "IX_TestEntity_Field1_Field2";

        // Act
        var actualName = config.IndexName;

        // Assert
        Assert.Equal(expectedName, actualName);
    }

    [Fact]
    public void UXName_WithNoFields_ReturnsCorrectlyFormattedString()
    {
        // Arrange
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new IndexConfig<TestEntity>
        {
            Fields = System.Array.Empty<string>()
        });
    }
}
