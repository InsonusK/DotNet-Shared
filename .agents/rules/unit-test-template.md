---
trigger: model_decision
description: When you create unit tests
---

# UnitTest.cs temlate

using Microsoft.Extensions.Logging;
using Divergic.Logging.Xunit;
using Xunit.Abstractions;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class {TestedClassName}_Test : LoggingTestsBase<{TestedClassName}_Test>
{

    public {TestedClassName}_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
    {

    }

    [Fact]
    public void test_{group}_WHEN_{condition}__THEN_{result}()
    {
        #region Array
        Logger.LogDebug("Test ARRAY");

        {define used variables}

        #endregion


        #region Act
        Logger.LogDebug("Test ACT");



        #endregion


        #region Assert
        Logger.LogDebug("Test ASSERT");

        #endregion
    }
}

# Method naming
- `{group}` - the method or property under test (NOT the class name)
- `{condition}` - the input state or scenario
- `{result}` - the expected outcome

# Fields naming
- expected_{field_name} - variable which contain expected values
- asserted_{field_name} - variable which must be checked and be equal expected_filed
