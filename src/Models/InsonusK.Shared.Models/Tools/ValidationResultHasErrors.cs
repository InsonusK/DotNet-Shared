using FluentValidation;
using FluentValidation.Results;

namespace InsonusK.Shared.Models.Tools;

public static class ValidationResultHasErrors
{
    public static bool HasErrors(this ValidationResult validationResult)
    {
        return !validationResult.IsValid && validationResult.Errors.Any(e => e.Severity == Severity.Error);
    }

    public static void AssertNoErrors(this ValidationResult validationResult)
    {
        if (validationResult.HasErrors())
            throw new ValidationException(validationResult.Errors);
    }

    public static ValidationResult ValidateAndThrowOnErrors<T>(
        this IValidator<T> validator,
        T instance)
    {
        var result = validator.Validate(instance);
        result.AssertNoErrors();
        return result;
    }

    public static async Task<ValidationResult> ValidateAndThrowOnErrorsAsync<T>(
        this IValidator<T> validator,
        T instance,
        CancellationToken cancellationToken = default)
    {
        var result = await validator.ValidateAsync(instance, cancellationToken);

        result.AssertNoErrors();
        
        return result;
    }
}