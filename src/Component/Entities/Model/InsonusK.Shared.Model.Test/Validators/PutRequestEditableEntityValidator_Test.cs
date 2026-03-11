using Divergic.Logging.Xunit;
using InsonusK.Shared.Model.Template;
using InsonusK.Shared.Model.Validator.Models;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace InsonusK.Shared.Model.Test.Validators;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class PutRequestEditableEntityValidator_Test : LoggingTestsBase<PutRequestEditableEntityValidator_Test>
{
    private readonly PutRequestEditableEntityValidator _validator = new();

    public PutRequestEditableEntityValidator_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
    {
    }

    private sealed record PutRequestEditableEntityStub : IPutRequestEditableEntity;

    /// <summary>
    /// description: PutRequestEditableEntityValidator has no rules; any input is valid
    /// input: any IPutRequestEditableEntity stub
    /// output: Validation passes
    /// expected_result: IsValid is true
    /// </summary>
    [Fact]
    public void test_Validate_WHEN_any_input__THEN_passes()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var asserted_model = new PutRequestEditableEntityStub();

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
