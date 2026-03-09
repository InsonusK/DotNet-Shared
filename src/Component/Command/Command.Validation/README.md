# InsonusK.Shared.Command.Validation

# Components
- [ServiceProviderExtensions](#ServiceProviderExtensions)
- [ValidationContextExtensions](#ValidationContextExtensions)
- [ValidationBehavior](#ValidationBehavior)

## ServiceProviderExtensions
Method list
- Dependency Injection Registration - [AddCommandValidation](#AddCommandValidation) - Registers validation behavior to MediatR pipeline.

### AddCommandValidation
Registers the `ValidationBehavior<,>` to the MediatR pipeline. This enables automatic validation of MediatR commands before their execution.

#### Input

| field | type | default value | description |
| --- | --- | --- | --- |
| services | IServiceCollection | | The dependency injection service collection. |

#### Output

| field | type | description |
| --- | --- | --- |
| | void | |

#### Exceptions / Errors

None

#### Usecases

```csharp
builder.Services.AddCommandValidation();
```

## ValidationContextExtensions
Method list
- Access Entities Context - [GetEntitiesContext](#GetEntitiesContext) - Retrieves the command context containing pre-loaded entities.
- Access Specific Entity - [GetEntity<TEntity>](#GetEntity) - Gets a specific type of pre-loaded entity from the validation context.

### GetEntitiesContext
Retrieves the command context containing pre-loaded entities from the validation context. Used internally but also accessible if raw context is needed.

#### Input

| field | type | default value | description |
| --- | --- | --- | --- |
| context | IValidationContext | | The FluentValidation validation context. |

#### Output

| field | type | description |
| --- | --- | --- |
| | ICommandContext | The command context with loaded entities. |

#### Exceptions / Errors

- `InvalidOperationException`: Thrown when the `ValidationEntitiesContext` is not found in `RootContextData`.

### GetEntity
Gets a specific type of pre-loaded entity from the validation context. Helper method to quickly access loaded entities inside FluentValidation rules.

#### Input

| field | type | default value | description |
| --- | --- | --- | --- |
| context | IValidationContext | | The FluentValidation validation context. |

#### Output

| field | type | description |
| --- | --- | --- |
| | TEntity? | The pre-loaded entity, or null if it was not loaded. |

#### Exceptions / Errors

- `InvalidOperationException`: Thrown when the entities context is not found in `RootContextData`.

#### Usecases

```csharp
RuleFor(x => x.Id)
    .Must((cmd, id, ctx) => 
    {
        var entity = ctx.GetEntity<MyEntity>();
        return entity != null; // Ensure entity was loaded and exists
    });
```

## ValidationBehavior
Method list
- MediatR Pipeline Handling - [Handle](#Handle) - Validates the command before passing it to the next handler.

### Handle
Handles the command validation before passing it to the next handler in the MediatR pipeline. Loads entities related to the command using `ICommandContextSource`, creates a validation context with these entities, and runs all registered FluentValidation validators. 

#### Input

| field | type | default value | description |
| --- | --- | --- | --- |
| request | TRequest | | The incoming MediatR command request (must implement `ICommandWithEntityKeys`). |
| next | RequestHandlerDelegate<TResponse> | | The delegate to the next handler in the MediatR pipeline. |
| ct | CancellationToken | | The cancellation token. |

#### Output

| field | type | description |
| --- | --- | --- |
| | Task<TResponse> | The response from the next handler if validation passes. |

#### Exceptions / Errors

- `FluentValidation.ValidationException`: Thrown when there are validation errors, or warnings if the command does not force execution (does not implement `IForcableValidatableCommand` with `Force` set to true).
