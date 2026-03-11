using Divergic.Logging.Xunit;
using FluentValidation;
using InsonusK.Shared.Model.Common;
using InsonusK.Shared.Model.Template;
using InsonusK.Shared.Model.Validator.Models;
using InsonusK.Shared.Model.Validator.Properties;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace InsonusK.Shared.Model.Test.Validators;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class PostRequestEditableEntityValidator_Test : LoggingTestsBase<PostRequestEditableEntityValidator_Test>
{
    private readonly PostRequestEditableEntityValidator _validator = new();

    public PostRequestEditableEntityValidator_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
    {
    }

    private sealed record PostRequestEditableEntityStub(Guid Guid) : IPostRequestEditableEntity;

    /// <summary>
    /// description: PostRequestEditableEntity with empty Guid
    /// input: Guid = Guid.Empty
    /// output: Validation fails
    /// expected_result: Contains error with ErrorCode "IsEmpty" and Severity.Error
    /// </summary>
    [Fact]
    public void test_Guid_WHEN_guid_is_empty__THEN_fails_with_IsEmpty_code()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var asserted_model = new PostRequestEditableEntityStub(Guid.Empty);
        string expected_errorCode = IsNotEmptyValidationExtensions.Code;

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
    /// description: PostRequestEditableEntity with valid Guid
    /// input: Guid = Guid.NewGuid()
    /// output: Validation passes
    /// expected_result: IsValid is true
    /// </summary>
    [Fact]
    public void test_Guid_WHEN_guid_is_valid__THEN_passes()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var asserted_model = new PostRequestEditableEntityStub(Guid.NewGuid());

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
