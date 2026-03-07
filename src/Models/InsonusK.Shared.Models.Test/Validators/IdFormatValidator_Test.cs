using Divergic.Logging.Xunit;
using Microsoft.Extensions.Logging;
using InsonusK.Shared.Models.Validators;
using FluentValidation;
using Xunit.Abstractions;

namespace InsonusK.Shared.Models.Test.Validators;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class IdFormatValidator_Test : LoggingTestsBase<IdFormatValidator_Test>
{
    public IdFormatValidator_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
    {
    }

    private sealed record IntIdStub(int Id);

    private class IntIdStubValidator : AbstractValidator<IntIdStub>
    {
        public IntIdStubValidator()
        {
            RuleFor(x => x.Id).IdIdFormatIsValid();
        }
    }

    private readonly IntIdStubValidator _validator = new();

    [Fact]
    public void test_Id_WHEN_id_is_positive__THEN_validation_passes()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var asserted_model = new IntIdStub(Id: 5);

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

    [Fact]
    public void test_Id_WHEN_id_is_zero__THEN_validation_fails()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var asserted_model = new IntIdStub(Id: 0);

        #endregion


        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_result = _validator.Validate(asserted_model);

        #endregion


        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.False(asserted_result.IsValid);

        #endregion
    }

    [Fact]
    public void test_Id_WHEN_id_is_negative__THEN_validation_fails()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var asserted_model = new IntIdStub(Id: -1);

        #endregion


        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_result = _validator.Validate(asserted_model);

        #endregion


        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.False(asserted_result.IsValid);

        #endregion
    }
}
