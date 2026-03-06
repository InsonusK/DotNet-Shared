using FluentValidation;

namespace InsonusK.Shared.Command.Validation.Validators;

public class CompositeValidator<T> : AbstractValidator<T>
{
    public CompositeValidator(IEnumerable<IValidator<T>> validators)
    {
        foreach (var validator in validators)
        {
            Include(validator);
        }
    }
}