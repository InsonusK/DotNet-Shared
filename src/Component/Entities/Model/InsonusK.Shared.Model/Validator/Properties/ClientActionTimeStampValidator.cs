using FluentValidation;
using InsonusK.Shared.Model.Common;

namespace InsonusK.Shared.Model.Validator;

public class ClientActionTimeStampValidator : AbstractValidator<IClientActionTimeStamp>
{
    public const string ModelName = nameof(IClientActionTimeStamp);
    public const string ActionTimeStampEmptyCode = ModelName + ".Empty";
    
    public ClientActionTimeStampValidator()
    {
        // Проверка, что Guid не является Guid.Empty
        RuleFor(x => x.ActionTimeStamp)
            .NotEqual(default(DateTimeOffset))
            .WithSeverity(Severity.Error).WithErrorCode(ActionTimeStampEmptyCode)
            .WithMessage("The ActionTimeStamp cannot be empty.");
    }
}
