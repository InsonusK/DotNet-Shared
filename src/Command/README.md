# Command.Validation

This project provides validation support for MediatR commands within the `InsonusK.Shared.Command` ecosystem, integrating closely with **FluentValidation** and the application's data repositories.

## Key Features

1. **Pipeline Behavior Validation**: Automatically validates MediatR requests that implement `IValidatableCommand` using defined FluentValidation rules.
2. **Automatic Entity Resolution**: Hooks into the command's `EntityKeys` to dynamically load referenced entities from the database before validation happens.
3. **Validation Context Enrichment**: Hydrates a `ValidationEntitiesContext` so that FluentValidation rules have direct access to fully-loaded entities for robust business-logic validation checks.
4. **Severity & Forcible Commands**: Differentiates between errors and warnings (`Severity.Warning`). Warning-level faults can be bypassed by implementing `IForcableValidatableCommand` and setting its `Force` property.

## Core Components

### `ValidationBehavior<TRequest, TResponse>`
A MediatR `IPipelineBehavior` that processes commands implementing `IValidatableCommand`.
- Finds all associated validators.
- Fetches all entities declared in `request.EntityKeys` via `EntityResolver`.
- Adds retrieved entities to `ValidationEntitiesContext`.
- Executes validators asynchronously.
- Throws a `ValidationException` if any `Severity.Error` validations fail (or warnings fail without the `Force` flag).

### `EntityResolver`
Dynamically resolves entities by parsing their IDs (either `Int` or `Guid`-based entities mapped to `stringId`).
- Relies on injected `IReadRepositoryBase<TEntity>`.
- Converts string identifiers into appropriate database queries (like `ByStringIdSpec` or direct `GetByIdAsync`).

### `ValidationEntitiesContext` & `IValidationEntitiesReadContext`
A scoped context for the duration of the validation pipeline. Let's rules access the fetched entities so that validation doesn't have to constantly query the database.
- Implements methods to `AddEntity`, `GetEntity<TEntity>`, and check `HasEntity<TEntity>`.

### `ValidationContextExtensions`
Provides extension methods for FluentValidation's `IValidationContext` such as `context.GetEntity<TEntity>()`, allowing easy extraction of pre-loaded entities directly inside custom validator classes.

## Usage

To utilize this library in a command:

1. Implement `IValidatableCommand` in your MediatR record/class.
2. Provide a dictionary of type/ID references in `EntityKeys`.
3. Create a custom FluentValidation `AbstractValidator` for your command.
4. Use `context.GetEntity<TEntity>()` in arbitrary `.Must()` extensions to enforce database-dependent rules without redundantly injecting repositories into validators.
