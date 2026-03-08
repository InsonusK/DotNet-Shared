# InsonusK.Shared.Command.Service.Interface

This library provides interfaces and dependency injection helpers for extracting entities from commands in the command processing pipeline.

# Components

- [IEntityCommandExtractor<TEntity>](#ientitycommandextractortentity)
- [ServiceCollectionExtensions](#servicecollectionextensions)

## IEntityCommandExtractor<TEntity>

Interface for extracting an entity based on an identification key, typically used to provide a loaded entity in the command processing context. 

### GetAsync

Extracts an entity based on the identification key.

#### Input

| field | type | default value | description |
| --- | --- | --- | --- |
| entityKey | `IEntityKey` | required | The entity key containing its identifier and type. |
| cancellationToken | `CancellationToken` | `default` | Cancellation token. |

#### Output

| field | type | description |
| --- | --- | --- |
| (return) | `Task<TEntity>` | The loaded entity. |

#### Exceptions / Errors

Depends on the specific implementation of the interface (e.g., throwing exceptions if the entity is not found in the database).

#### Usecases

To use this interface:
1. Implement `IEntityCommandExtractor<TEntity>` for your specific `TEntity`.
2. Register the implementation in the DI container. Use the provided extension method for automatic registration.
3. Access the entity in your command handler via the `ICommandContext`.

## ServiceCollectionExtensions

Provides extension methods for `IServiceCollection` to simplify the registration of `IEntityCommandExtractor` implementations.

### AddEntityCommandExtractor

Scans the provided assemblies for non-abstract implementations of `IEntityCommandExtractor<TEntity>` and registers them in the dependency injection container with a `Scoped` lifetime.

#### Input

| field | type | default value | description |
| --- | --- | --- | --- |
| sc | `IServiceCollection` | required | The service collection to add the extractors to. |
| config | `IConfiguration` | `null` | Optional configuration. Not currently utilized but kept for API surface compatibility. |
| assembliesToScan | `params Assembly[]` | `empty` | The assemblies to scan. If empty, it scans the calling assembly. |

#### Output

| field | type | description |
| --- | --- | --- |
| (return) | `IServiceCollection` | The modified service collection for chaining. |

#### Exceptions / Errors

Contains no specific exceptions. Throws standard ArgumentNullExceptions if `IServiceCollection` is null but handled usually by the container builder.

#### Usecases

```csharp
// Register all extractors from the current assembly
services.AddEntityCommandExtractor();

// Register extractors from specific assemblies
services.AddEntityCommandExtractor(null, typeof(MyExtractor).Assembly);
```