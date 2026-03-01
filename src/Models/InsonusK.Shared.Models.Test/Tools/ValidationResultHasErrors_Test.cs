using Divergic.Logging.Xunit;
using Microsoft.Extensions.Logging;
using FluentValidation;
using FluentValidation.Results;
using InsonusK.Shared.Models.Tools;
using Xunit.Abstractions;

namespace InsonusK.Shared.Models.Test.Tools;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class ValidationResultHasErrors_Test : LoggingTestsBase<ValidationResultHasErrors_Test>
{
    public ValidationResultHasErrors_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
    {
    }

    [Fact]
    public void test_HasErrors_WHEN_result_has_error_severity_failure__THEN_returns_true()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var failure = new ValidationFailure("Field", "Error message")
        {
            Severity = Severity.Error
        };
        var asserted_validationResult = new ValidationResult(new[] { failure });

        #endregion


        #region Act
        Logger.LogDebug("Test ACT");

        bool asserted_result = asserted_validationResult.HasErrors();

        #endregion


        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.True(asserted_result);

        #endregion
    }

    [Fact]
    public void test_HasErrors_WHEN_result_only_has_warning_severity_failure__THEN_returns_false()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var failure = new ValidationFailure("Field", "Warning message")
        {
            Severity = Severity.Warning
        };
        var asserted_validationResult = new ValidationResult(new[] { failure });

        #endregion


        #region Act
        Logger.LogDebug("Test ACT");

        bool asserted_result = asserted_validationResult.HasErrors();

        #endregion


        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.False(asserted_result);

        #endregion
    }

    [Fact]
    public void test_HasErrors_WHEN_result_is_valid__THEN_returns_false()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var asserted_validationResult = new ValidationResult();

        #endregion


        #region Act
        Logger.LogDebug("Test ACT");

        bool asserted_result = asserted_validationResult.HasErrors();

        #endregion


        #region Assert
        Logger.LogDebug("Test ASSERT");

        Assert.False(asserted_result);

        #endregion
    }

    [Fact]
    public void test_AssertNoErrors_WHEN_result_has_error__THEN_throws_ValidationException()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var failure = new ValidationFailure("Field", "Error message")
        {
            Severity = Severity.Error
        };
        var asserted_validationResult = new ValidationResult(new[] { failure });

        #endregion


        #region Act & Assert
        Logger.LogDebug("Test ACT");

        Assert.Throws<ValidationException>(() => asserted_validationResult.AssertNoErrors());

        #endregion
    }

    [Fact]
    public void test_AssertNoErrors_WHEN_result_is_valid__THEN_does_not_throw()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var asserted_validationResult = new ValidationResult();

        #endregion


        #region Act & Assert
        Logger.LogDebug("Test ACT");

        var exception = Record.Exception(() => asserted_validationResult.AssertNoErrors());
        Assert.Null(exception);

        #endregion
    }
}
