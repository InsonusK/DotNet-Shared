# InsonusK.Shared.Command.Interfaces

A shared library providing core interfaces and models for commands within the CQRS structure, specifically tailored for entity identification, validation, and command bodies.

## Components

- [IEntityKey Interfaces](#ientitykey-interfaces)
- [ICommand Interfaces](#icommand-interfaces)
- [EntityKey Model](#entitykey-model)
- [Extensions](#extensions)

## IEntityKey Interfaces

Core interfaces to strongly type and identify entities in operations.

- `{Entity Identification}` - [IEntityType](#ientitytype) - Contains entity type information.
- `{Entity Identification}` - [IEntityKey](#ientitykey) - Unique key containing the `StringId`.
- `{Concurrency Control}` - [IEntityVersionedKey](#ientityversionedkey) - Unique key that supports optimistic concurrency.

### IEntityType
Contains the `Type` of the entity.

### IEntityKey
Extends `IEntityType` to include the `StringId` of the entity.

### IEntityVersionedKey
Extends `IEntityKey` for concurrency control, including a `Version` and a `VersionRequired` boolean flag.

## ICommand Interfaces

Core interfaces for properties of commands that can be utilized by middlewares, handlers, and validators.

- `{Command Request Payload}` - [ICommandWithBody](#icommandwithbody) - Interface for commands that include a request body payload.
- `{Command Identification}` - [ICommandWithEntityKeys](#icommandwithentitykeys) - Interface for commands that target entities by their keys.
- `{Command Validation}` - [IForcableValidatableCommand](#iforcablevalidatablecommand) - Allows commands to forcefully bypass certain validations.

### ICommandWithBody
Interface for commands that include a request body (e.g., POST/PUT/PATCH DTOs). Contains `BodyType`, `objBody`, and `BodyRequired`.
The generic variant `ICommandWithBody<TBody>` exposes a strongly typed `Body`.

### ICommandWithEntityKeys
Interface for commands that operate on one or more specific entities. Exposes an `IReadOnlyCollection<IEntityKey> EntityKeys`.

### IForcableValidatableCommand
Extends `ICommandWithEntityKeys` with a `Force` boolean property to allow bypassing certain rules or forcing execution.

## EntityKey Model

`EntityKey<TEntity>`

A concrete implementation of `IEntityVersionedKey` for a specific entity type `TEntity`.

### Method WithVersion

Creates an entity key that requires a specific version to validate concurrency.

#### Input

| field | type | default value | description |
| --- | --- | --- | --- |
| stringId | string | | The identifier of the entity |
| version | string | | The expected version of the entity |

#### Output

| field | type | description |
| --- | --- | --- |
| EntityKey | `EntityKey<TEntity>` | Entity key with version required |

### Method WithoutVersion

Creates an entity key that does not enforce a version constraint.

#### Input

| field | type | default value | description |
| --- | --- | --- | --- |
| stringId | string | | The identifier of the entity |

#### Output

| field | type | description |
| --- | --- | --- |
| EntityKey | `EntityKey<TEntity>` | Entity key without version requirement |

## Extensions

Helper extension methods for simpler extraction of entity keys.

- `{Key Extraction}` - [EntityKeyExtensions.Get<TEntity>](#entitykeyextensionsgettentity) - Extract key from collection.
- `{Key Extraction}` - [IValidatableCommandExtensions.GetKey<TEntity>](#ivalidatablecommandextensionsgetkeytentity) - Extract key from command.


### EntityKeyExtensions.Get<TEntity>

Extension method on `IEnumerable<IEntityKey>` to retrieve a specifically typed `EntityKey<TEntity>`.

#### Exceptions / Errors
- `InvalidOperationException`: Thrown if the requested entity type is not found in the collection.

### IValidatableCommandExtensions.GetKey<TEntity>

Extension method on `ICommandWithEntityKeys` to directly fetch a specifically typed `EntityKey<TEntity>` from the command's `EntityKeys` collection.

#### Exceptions / Errors
- `InvalidOperationException`: Thrown if the requested entity type is not found within the command.
