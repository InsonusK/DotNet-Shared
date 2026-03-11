using Divergic.Logging.Xunit;
using FluentValidation;
using InsonusK.Shared.Model.Validator.Properties;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace InsonusK.Shared.Model.Test.Validators;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class IsCollectionNotEmptyValidationExtensions_Test : LoggingTestsBase<IsCollectionNotEmptyValidationExtensions_Test>
{
    public IsCollectionNotEmptyValidationExtensions_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug)
        : base(output, logLevel) { }

    private sealed record CollectionStub(IEnumerable<string> Items);

    private class CollectionStubValidator : AbstractValidator<CollectionStub>
    {
        public CollectionStubValidator() => RuleFor(x => x.Items).IsCollectionNotEmpty();
    }

    private readonly CollectionStubValidator _validator = new();

    /// <summary>
    /// description: Collection is empty
    /// input: Items = []
    /// output: Validation fails
    /// expected_result: Contains error with ErrorCode "IsEmpty" and Severity.Error
    /// </summary>
    [Fact]
    public void test_Collection_WHEN_collection_is_empty__THEN_fails_with_IsEmpty_code()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var asserted_model = new CollectionStub(Enumerable.Empty<string>());
        string expected_errorCode = IsCollectionNotEmptyValidationExtensions.Code;

        #endregion


        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_result = _validator.Validate(asserted_model);

        #endregion


        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.False(asserted_result.IsValid);
        Assert.Contains(asserted_result.Errors, e => e.ErrorCode == expected_errorCode && e.Severity == Severity.Error);

        #endregion
    }

    /// <summary>
    /// description: Collection has at least one item
    /// input: Items = ["a", "b"]
    /// output: Validation passes
    /// expected_result: IsValid is true
    /// </summary>
    [Fact]
    public void test_Collection_WHEN_collection_has_items__THEN_passes()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var asserted_model = new CollectionStub(new[] { "a", "b" });

        #endregion


        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_result = _validator.Validate(asserted_model);

        #endregion


        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.True(asserted_result.IsValid);

        #endregion
    }
}
