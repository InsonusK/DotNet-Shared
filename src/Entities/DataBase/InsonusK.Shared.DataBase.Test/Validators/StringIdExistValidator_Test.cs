using Divergic.Logging.Xunit;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using NSubstitute;
using Ardalis.Specification;
using FluentValidation;
using FluentValidation.Internal;
using InsonusK.Shared.DataBase.Validators;

namespace InsonusK.Shared.DataBase.Test.Validators;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class StringIdExistValidator_Test : LoggingTestsBase<StringIdExistValidator_Test>
{
    public StringIdExistValidator_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
    {
    }

    private static ValidationContext<object> MakeContext()
        => new ValidationContext<object>(new object(), new PropertyChain(), ValidatorOptions.Global.ValidatorSelectors.DefaultValidatorSelectorFactory());

    [Fact]
    public async Task test_IsValidAsync_WHEN_entity_exists_in_repository__THEN_returns_true()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        string input = Guid.NewGuid().ToString();
        var asserted_repository = Substitute.For<IReadRepositoryBase<TestEntity>>();
        asserted_repository.CountAsync(Arg.Any<ISpecification<TestEntity>>(), Arg.Any<CancellationToken>())
            .Returns(1);

        var asserted_validator = new StringIdExistValidator<object, TestEntity>(asserted_repository);

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

        string input = Guid.NewGuid().ToString();
        var asserted_repository = Substitute.For<IReadRepositoryBase<TestEntity>>();
        asserted_repository.CountAsync(Arg.Any<ISpecification<TestEntity>>(), Arg.Any<CancellationToken>())
            .Returns(0);

        var asserted_validator = new StringIdExistValidator<object, TestEntity>(asserted_repository);

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
    public async Task test_IsValidAsync_WHEN_value_is_empty_and_validateOnlyIfNotEmpty_is_true__THEN_returns_true()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        string input = "";
        var asserted_repository = Substitute.For<IReadRepositoryBase<TestEntity>>();
        var asserted_validator = new StringIdExistValidator<object, TestEntity>(
            asserted_repository, validateOnlyIfNotEmpty: true);

        #endregion


        #region Act
        Logger.LogDebug("Test ACT");

        bool asserted_result = await asserted_validator.IsValidAsync(MakeContext(), input, CancellationToken.None);

        #endregion


        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.True(asserted_result);
        await asserted_repository.DidNotReceive()
            .CountAsync(Arg.Any<ISpecification<TestEntity>>(), Arg.Any<CancellationToken>());

        #endregion
    }

    [Fact]
    public async Task test_IsValidAsync_WHEN_value_matches_newGuid__THEN_returns_true_without_querying_repository()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        Guid expected_newGuid = Guid.NewGuid();
        string input = expected_newGuid.ToString();
        var asserted_repository = Substitute.For<IReadRepositoryBase<TestEntity>>();
        var asserted_validator = new StringIdExistValidator<object, TestEntity>(
            asserted_repository, newGuid: expected_newGuid);

        #endregion


        #region Act
        Logger.LogDebug("Test ACT");

        bool asserted_result = await asserted_validator.IsValidAsync(MakeContext(), input, CancellationToken.None);

        #endregion


        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.True(asserted_result);
        await asserted_repository.DidNotReceive()
            .CountAsync(Arg.Any<ISpecification<TestEntity>>(), Arg.Any<CancellationToken>());

        #endregion
    }
}
