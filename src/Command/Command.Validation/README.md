# InsonusK.Shared.Command.Validation

# Components
- [ValidationBehavior](#ValidationBehavior)
- [EntityResolver](#EntityResolver)
- [ValidationEntitiesContext](#ValidationEntitiesContext)
- [ValidationContextExtensions](#ValidationContextExtensions)

## ValidationBehavior
Method list
- Validate command and resolve entities - [Handle](#Handle) - Processes the MediatR pipeline for validation.

### Handle
Validates the incoming command. It resolves entities specified in `IValidatableCommand.EntityKeys`, stores them in a context, and executes validators.

#### Input

| field | type | default value | description |
| --- | --- | --- | --- |
| request | TRequest | - | The MediatR request to process, must implement `IValidatableCommand` |
| next | RequestHandlerDelegate<TResponse> | - | The next delegate in the pipeline |
| ct | CancellationToken | - | Cancellation token |

#### Output

| field | type | description |
| --- | --- | --- |
| Response | TResponse | The result from the pipeline. |

#### Exceptions / Errors
- `ValidationException` is thrown if any validator returns errors (Severity.Error) or if warning-level errors occur and the command isn't flagged to force bypass them (`IForcableValidatableCommand`).
- Throws `ValidationException` if an entity specified in `EntityKeys` cannot be found in the database.

#### Esecases
Injects into MediatR pipeline automatically if configured in dependency injection.

## EntityResolver
Mothed list
- Resolve entity from database - [Resolve](#Resolve) - Fetches the entity from the configured read repository.

### Resolve
Retrieves the corresponding database record based on whether the entity uses standard integer IDs or Guid IDs (`IGuidModel`).

#### Input

| field | type | default value | description |
| --- | --- | --- | --- |
| entityType | Type | - | The runtime Type of the entity to resolve |
| id | string | - | The string representation of the ID |
| ct | CancellationToken | - | Cancellation token |

#### Output

| field | type | description |
| --- | --- | --- |
| Entity | object? | The retrieved entity if found, or null otherwise. |

#### Exceptions / Errors
None specific. Throws standard repository exceptions on database failure.

#### Esecases
Resolving entities dynamically for the validation context.

## ValidationEntitiesContext
Mothed list
- Add entity to context - [AddEntity](#AddEntity) - Adds an entity reference to the current context.
- Add typed entity to context - [AddEntity<TEntity>](#AddEntity<TEntity>) - Adds a typed entity to the context.
- Retrieve typed entity - [GetEntity<TEntity>](#GetEntity) - Returns a typed entity from the context.
- Try retrieve typed entity - [TryGetEntity<TEntity>](#TryGetEntity) - Attempts to get a typed entity from the context.
- Check entity existence - [HasEntity<TEntity>](#HasEntity) - Checks if an entity type exists in the context.

### AddEntity
Adds a resolved `EntityBase` into its internal dictionary tracked by its precise runtime type.

#### Input

| field | type | default value | description |
| --- | --- | --- | --- |
| entity | EntityBase | - | The entity object to add |

#### Output
None.

#### Exceptions / Errors
- `ArgumentException` - Thrown if an entity of exactly that type has already been added.

#### Esecases
Used by Pipeline Behavior to populate context.

### AddEntity<TEntity>
Adds a resolved `TEntity` into its internal dictionary.

#### Input

| field | type | default value | description |
| --- | --- | --- | --- |
| entity | TEntity | - | The typed entity object to add |

#### Output
None.

#### Exceptions / Errors
- `ArgumentException` - Thrown if an entity of exactly that type has already been added.

#### Esecases
Used to easily populate the context statically.

### GetEntity
Retrieves an entity by its type from the context dictionary.

#### Input
None.

#### Output

| field | type | description |
| --- | --- | --- |
| Entity | TEntity | Typed entity |

#### Exceptions / Errors
- `ArgumentException` - Thrown if no entity matches the requested type.

#### Esecases
Used to retrieve entity from context.

### TryGetEntity
Attempts to retrieve an entity by its type.

#### Input

| field | type | default value | description |
| --- | --- | --- | --- |
| entity | out TEntity? | - | The returned entity reference |

#### Output

| field | type | description |
| --- | --- | --- |
| Success | bool | True if found, otherwise false |

#### Exceptions / Errors
None.

#### Esecases
Used to safely retrieve entity without exception.

### HasEntity
Checks if an entity of type exists.

#### Input
None.

#### Output

| field | type | description |
| --- | --- | --- |
| Exists | bool | True if exists, otherwise false |

#### Exceptions / Errors
None.

#### Esecases
Used to check entity existence in the context without retrieving.

## ValidationContextExtensions
Mothed list
- Set entities context - [SetEntitiesContext](#SetEntitiesContext) - Sets the context in RootContextData.
- Get entities context - [GetEntitiesContext](#GetEntitiesContext) - Gets the context from RootContextData.
- Read specific entity - [GetEntity<TEntity>](#GetEntity-Extension) - Shortcut to retrieve an entity from within a custom Validator rule.

### SetEntitiesContext
Sets the ValidationEntitiesContext in the FluentValidation root context.

#### Input

| field | type | default value | description |
| --- | --- | --- | --- |
| context | IValidationContext | - | Extension target context |
| ctx | ValidationEntitiesContext | - | The entities context |

#### Output
None.

#### Exceptions / Errors
None.

#### Esecases
Populating root data in behavior.

### GetEntitiesContext
Gets the ValidationEntitiesContext from the FluentValidation root context.

#### Input

| field | type | default value | description |
| --- | --- | --- | --- |
| context | IValidationContext | - | Extension target context |

#### Output

| field | type | description |
| --- | --- | --- |
| Context | IValidationEntitiesReadContext | The typed entities context |

#### Exceptions / Errors
- `InvalidOperationException` if the `ValidationEntitiesContext` is missing from RootContextData.

#### Esecases
Retrieving context inside downstream processors.

### GetEntity-Extension
Allows the FluentValidation `AbstractValidator` checks like `.Must()` to gain fast access to the pre-loaded entity.

#### Input

| field | type | default value | description |
| --- | --- | --- | --- |
| context | IValidationContext | - | Extension target context |

#### Output

| field | type | description |
| --- | --- | --- |
| Entity | TEntity? | Typed entity stored in context, if it exists |

#### Exceptions / Errors
- `InvalidOperationException` if the `ValidationEntitiesContext` is missing from RootContextData.

#### Esecases
Using `context.GetEntity<TEntity>()` directly in FluentValidation rules.
