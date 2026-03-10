using Ardalis.Specification;
using Divergic.Logging.Xunit;
using FluentValidation.Internal;
using FluentValidation;
using InsonusK.Shared.DataBase.Validators.Properties;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit.Abstractions;

namespace InsonusK.Shared.DataBase.Test.Validators;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class IdExistValidator_Test : LoggingTestsBase<IdExistValidator_Test>
{
    public IdExistValidator_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
    {
    }

    private static ValidationContext<object> MakeContext()
        => new ValidationContext<object>(new object(), new PropertyChain(), ValidatorOptions.Global.ValidatorSelectors.DefaultValidatorSelectorFactory());

    [Fact]
    public async Task test_IsValidAsync_WHEN_entity_exists_in_repository__THEN_returns_true()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        int input = 42;
        var asserted_repository = Substitute.For<IReadRepositoryBase<TestEntity>>();
        asserted_repository.GetByIdAsync(input, Arg.Any<CancellationToken>())
            .Returns(new TestEntity());

        var asserted_validator = new IdExistValidator<object, TestEntity>(asserted_repository);

        #endregion


        #region Act
        Logger.LogDebug("Test ACT");

        bool asserted_result = await asserted_validator.IsValidAsync(MakeContext(), input, CancellationToken.None);

        #endregion


        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.True(asserted_result);

        #endregion
    }

    [Fact]
    public async Task test_IsValidAsync_WHEN_entity_does_not_exist_in_repository__THEN_returns_false()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        int input = 99;
        var asserted_repository = Substitute.For<IReadRepositoryBase<TestEntity>>();
        asserted_repository.GetByIdAsync(input, Arg.Any<CancellationToken>())
            .Returns((TestEntity?)null);

        var asserted_validator = new IdExistValidator<object, TestEntity>(asserted_repository);

        #endregion


        #region Act
        Logger.LogDebug("Test ACT");

        bool asserted_result = await asserted_validator.IsValidAsync(MakeContext(), input, CancellationToken.None);

        #endregion


        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.False(asserted_result);

        #endregion
    }

    [Fact]
    public void test_Code_WHEN_checked__THEN_is_IsNotExistingId()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");
        string expected_code = "IsNotExistingId";

        #endregion


        #region Act
        Logger.LogDebug("Test ACT");
        string asserted_code = IdExistValidator<object, TestEntity>.Code;

        #endregion


        #region Assert
        Logger.LogDebug("Test ASSERT");
        Assert.Equal(expected_code, asserted_code);

        #endregion
    }
}
