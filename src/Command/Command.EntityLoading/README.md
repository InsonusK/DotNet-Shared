# InsonusK.Shared.Command.EntityLoading

Package for pre-loading entities before command execution in a MediatR pipeline. 
It integrates seamlessly with commands that implement `ICommandWithEntityKeys`.

## Components
- [ICommandContext](#ICommandContext)
- [IEntityCommandExtractor](#IEntityCommandExtractor)
- [AddCommandEntityLoading](#AddCommandEntityLoading)

## ICommandContext
Interface for reading entity context data during command execution.

### Methods
- **Get&lt;TEntity&gt;** - Gets an entity of the specified type from the context.
- **TryGet&lt;TEntity&gt;** - Attempts to get an entity from the context, returning true if successful.
- **Has&lt;TEntity&gt;** - Checks if an entity of the specified type exists in the context.

#### Output
| method | type | description |
| --- | --- | --- |
| Get | TEntity | Entity of the specified type from context |
| TryGet | bool | True if found, out entity returns the entity |
| Has | bool | True if found |

#### Exceptions / Errors
- **ArgumentException** - Thrown when attempting to `Get<TEntity>()` a type that hasn't been added to the `CommandContext`.

## IEntityCommandExtractor
Interface for custom entity extraction based on the `IEntityKey`.

### Setup
Implement `IEntityCommandExtractor<TEntity>` and register it in the dependency injection container. For example: `[Service(interfaces: typeof(IEntityCommandExtractor<MyEntity>))]`.
When a command specifies an entity key matching the generic type, this extractor will be invoked.

### Methods
- **GetAsync** - Extracts and returns an entity using its key. Dispatched automatically by the `EntityProvider`.

#### Input
| field | type | default value | description |
| --- | --- | --- | --- |
| entityKey | IEntityKey | | Entity key containing type and string ID. |
| cancellationToken | CancellationToken | default | Cancellation token. |

#### Exceptions / Errors
- **NotFoundException** - Can be thrown if an entity referenced by `IEntityKey` cannot be resolved. Expected behavior so that command execution aborts.

## AddCommandEntityLoading
Extension method to register necessary services in the DI container.

### Setup
In your `Program.cs` or `Startup.cs`:
```csharp
builder.Services.AddCommandEntityLoading();
```
This registers the pipeline behavior (`EntityLoadingBehavior`), `CommandContextManager`, `ICommandContextSource`, and `EntityProvider`.

## Use cases

1. Define a command implementing `ICommandWithEntityKeys`.
2. Send the command using `MediatR`'s `ISender`.
3. Receive `ICommandContextSource` injected into your Command Handler to access perfectly pre-loaded `ICommandContext` entities.

```csharp
public class MyCommandHandler : IRequestHandler<MyCommand>
{
    private readonly ICommandContextSource _contextSource;

    public MyCommandHandler(ICommandContextSource contextSource)
    {
        _contextSource = contextSource;
    }

    public async Task Handle(MyCommand request, CancellationToken ct)
    {
        // Obtain loaded context
        var context = await _contextSource.GetForAsync(request, ct);

        // Fetch pre-loaded entity
        var entity = context.Get<MyEntity>();
        
        // Process entity...
    }
}
```
