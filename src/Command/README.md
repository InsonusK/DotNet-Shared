# InsonusK.Shared.Command.Validation

# Components
- [ValidationBehavior](#ValidationBehavior)
- [EntityResolver](#EntityResolver)
- [ValidationEntitiesContext](#ValidationEntitiesContext)
- [ValidationContextExtensions](#ValidationContextExtensions)

---

## ValidationBehavior
Pipeline behavior for MediatR that automatically validates commands and resolves database entities before running FluentValidation rules.

- Intercept Command - [Handle](#Handle) - Processes the MediatR pipeline for validation.

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

#### Usecases
Injects into MediatR pipeline automatically if configured in dependency injection.

---

## EntityResolver
Responsible for dynamically loading entities from the database given their Type and a string identifier.

- Resolve Entity - [Resolve](#Resolve) - Fetches the entity from the configured read repository.

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

---

## ValidationEntitiesContext
A container holding the specific subset of database entities loaded for a specific validation cycle.

- Add to context - [AddEntity](#AddEntity) - Adds an entity reference to the current context.
- Retrieve from context - [GetEntity<TEntity>](#GetEntity) - Returns a typed entity from the context.

### AddEntity
Adds a resolved `EntityBase` into its internal dictionary tracked by its precise runtime type.

#### Input
| field | type | default value | description |
| --- | --- | --- | --- |
| entity | EntityBase | - | The entity object |

#### Exceptions / Errors
- `ArgumentException` - Thrown if an entity of exactly that type has already been added.

### GetEntity
Retrieves an entity by its type from the context dictionary.

#### Output
| field | type | description |
| --- | --- | --- |
| Entity | TEntity | Typed entity |

#### Exceptions / Errors
- `ArgumentException` - Thrown if no entity matches the requested type.

---

## ValidationContextExtensions
Extension methods bridging the FluentValidation `IValidationContext` and the `ValidationEntitiesContext`.

- Read specific entity - [GetEntity<TEntity>](#GetEntity-Extension) - Shortcut to retrieve an entity from within a custom Validator rule.

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
