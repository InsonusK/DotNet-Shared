# InsonusK.Shared.Mediator.CommandContext

This library provides MediatR pipeline behaviors to automatically extract and populate necessary domain entities before your main command handler is executed.

## Nuget packages
- [GitHub Packages](https://github.com/InsonusK/DotNet-Shared/pkgs/nuget/InsonusK.Shared.Mediator.CommandContext)
- [Nuget](https://www.nuget.org/packages/InsonusK.Shared.Mediator.CommandContext/)

## Core Features

- **Pre-loading Entities**: By defining entity keys in your commands, the pipeline behaviors automatically load those entities, enforcing versions where necessary.
- **`ICommandContext`**: A centralized service `CommandContextContainer` where extracted entities are stored. Your command handlers can inject `ICommandContext` to safely retrieve entities by calling `Entity<TEntity>()`.
- **Command Base Classes**: Derive your requests from `CommandWithEntityKeys` (or `CommandWithStringIdEntity`) to specify which `IEntityKey` identifiers to extract.
- **Flexible Extraction**: Requires implementation of `IEntityCommandExtractor<TEntity>` to define how each particular entity type should be fetched from storage.

## Dependency Injection Registration

You can register this module into your dependency injection container using the provided extension method:

```csharp
services.Register(config); // From CommandContextModuleRegister
```
