using Divergic.Logging.Xunit;
using FluentValidation;
using InsonusK.Shared.Model.Validator;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace InsonusK.Shared.DataBase.Test.Validators;

public class SandBox
{
    public string StringProp { get; set; }
}
public class SandboxValidator : AbstractValidator<SandBox>
{
    public SandboxValidator()
    {
        RuleFor(x => x.StringProp).NotEmpty();
    }
}
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class SandBox_Test : LoggingTestsBase<StringIdExistValidator_Test>
{
    public SandBox_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
    {
    }

    [Fact]
    public void test_DefaultRule_WHEN_use_default_rule_THEN_bad_code_and_message()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        var asserted_validator = new SandboxValidator();
        var input = new SandBox();
        input.StringProp = "";


        #endregion


        #region Act
        Logger.LogDebug("Test ACT");

        var asserted_result = asserted_validator.Validate(input);


        #endregion


        #region Assert
        Logger.LogDebug("Test ASSERT");
        var asserted_error = Assert.Single(asserted_result.Errors);
        Logger.LogInformation(asserted_error.ErrorCode  );
        Logger.LogInformation(asserted_error.ErrorMessage  );
        Logger.LogInformation(asserted_error.PropertyName  );
        Logger.LogInformation(asserted_error.Severity.ToString()  );
        
        #endregion
    }
}