# InsonusK.Shared.Models

Shared domain contracts, DTO templates, FluentValidation validators, and utility helpers used across services. Does **not** depend on any infrastructure or database library.

- **Target framework:** .NET 9.0
- **Key dependencies:** `FluentValidation 12`, `Ardalis.Result 10`

## Nuget packages
- [GitHub Packages](https://github.com/InsonusK/DotNet-Shared/pkgs/nuget/InsonusK.Shared.Models)
- [Nuget](https://www.nuget.org/packages/InsonusK.Shared.Models/)


## Project layout

```
InsonusK.Shared.Models/
├── Common/         # Core interfaces describing entity state
├── Interfaces/     # Generic command / request contracts
├── Template/       # Reusable DTO shapes (request/response)
├── Tools/          # Utility helpers
└── Validators/     # FluentValidation validators
```

---

## Common — entity interfaces

| Interface | Description |
|---|---|
| `IBaseModel` | Minimal marker for all models |
| `IGuidModel` | Adds a `Guid` identifier |
| `IVersionatedModel` | Adds a `RowVersion` / concurrency token |
| `IValidationResult` | Carries a validation result alongside a model |
| `IClientActionTimeStamp` | Carries a client-supplied `ActionTimeStamp` |
| `ICreationInfoModel` / `ICreationInfoModelReadOnly` | `ServerCreatedDateTime` + `UserCreatedDateTime` |
| `IUpdateInfoModel` / `IUpdateInfoModelReadOnly` | `ServerUpdatedDateTime` + `UserUpdatedDateTime` (validates that update is not earlier than creation) |

---

## Interfaces — command contracts

| Interface | Description |
|---|---|
| `IEntityCommand` | Base command with an entity identifier |
| `IPatchRequest` | Marker for PATCH-style partial-update requests |

---

## Template — reusable DTO shapes

| Type | Description |
|---|---|
| `IFetchRequest` | Pagination contract: `Page` (default 1) and `PageSize` (default 10) |
| `IPostRequestEditableEntity` | Contract for POST (create) requests |
| `IPutRequestEditableEntity` | Contract for PUT (full replace) requests |
| `IDeleteRequestDto` | Contract for DELETE requests |
| `IResponseEditableEntity` | Contract for responses that include editable-entity fields |
| `BulkResponseDto` | Response wrapper for bulk operations |

---

## Tools — utility helpers

### `StringIdExtension`

```csharp
bool ok = "42".ToId(out int id, out Guid guid);       // id = 42
bool ok = "3fa8...".ToId(out int id, out Guid guid);  // guid = parsed Guid
```

Parses a string that can represent either an `int` or a `Guid`. Returns `false` for blank or unrecognised values.

### `ValidationResultHasErrors`

Extension methods that treat only `Severity.Error` entries as real failures (warnings are ignored):

```csharp
// Returns true only when errors with Severity.Error exist
bool failed = validationResult.HasErrors();

// Throws ValidationException when errors exist
validationResult.AssertNoErrors();

// Validate and throw on errors in one call (sync + async)
var result = validator.ValidateAndThrowOnErrors(dto);
var result = await validator.ValidateAndThrowOnErrorsAsync(dto, ct);
```

### `ResultAdapter`

Helpers for converting between `FluentValidation.Results.ValidationResult` and `Ardalis.Result`.

### `ClientActionTimeStampParser`

Parses a client-provided `ActionTimeStamp` string into a `DateTimeOffset`.

---

## Validators

| Validator | Validates |
|---|---|
| `ClientActionTimeStampValidator` | `IClientActionTimeStamp.ActionTimeStamp` must not be `default(DateTimeOffset)`. Error code: `IClientActionTimeStamp.Empty` |
| `GuidModelValidator` | `IGuidModel.Guid` must not be `Guid.Empty` |
| `IdFormatValidator` | `int` id must be positive |
| `StirngIdFormetValidator` | String-encoded id must parse as either a positive `int` or a non-empty `Guid` |
| `FetchRequestValidator` | `Page ≥ 1` and `PageSize ≥ 1` |
| `PostRequestEditableEntityValidator` | Validates a POST request DTO |
| `PutRequestEditableEntityValidator` | Validates a PUT request DTO |
| `PatchRequestValidator` | Validates a PATCH request DTO |
| `IsNotEmpty` | Generic not-empty rule helper |
