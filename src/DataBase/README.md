# InsonusK.Shared.DataBase

Reusable EF Core / Ardalis.Specification building blocks for repositories that use a dual-key entity model (auto-increment `int Id` + immutable `Guid`).

- **Target framework:** .NET 9.0
- **Key dependencies:** `Ardalis.Specification 9`, `Ardalis.GuardClauses 5`, `EF Core 9`, `Npgsql.EntityFrameworkCore.PostgreSQL 9`
- **Depends on:** `InsonusK.Shared.Models`

## Nuget packages
- [GitHub Packages](https://github.com/InsonusK/DotNet-Shared/pkgs/nuget/InsonusK.Shared.DataBase)
- [Nuget](https://www.nuget.org/packages/InsonusK.Shared.DataBase/)

## Project layout

```
InsonusK.Shared.DataBase/
├── Models/       # EF entity base classes
├── Spec/         # Ardalis.Specification specifications
├── Validators/   # FluentValidation async property validators
└── Handlers/     # Exception handlers
```

---

## Models — entity base classes

All entities inherit from `EntityBase → ConstantNoGuidEntity → ConstantGuidEntity`.

| Class | Key fields | Notes |
|---|---|---|
| `EntityBase` | — | Minimal marker |
| `ConstantNoGuidEntity` | `int Id` | Auto-increment PK |
| `ConstantGuidEntity` | `int Id`, `Guid Guid` | Guid is **immutable** once `Id != 0`. Provides `IsEqualStringId(string)` |
| `EditableNoGuidEntity` | `int Id` + audit fields | Adds `ICreationInfoModel` + `IUpdateInfoModel` |
| `EditableGuidEntity` | `int Id`, `Guid Guid` + audit | Editable variant with both keys |
| `IDeleteStatusEntity` | `bool IsDeleted` | Soft-delete flag |
| `IndexConfig` | `TableName`, `UFields`, `IsUnique`, `IndexName`,`AddToEntityTypeBuilder(builder)` | Maps a unique-constraint name to its field list for user-friendly error messages |

---

## Spec — Ardalis specifications

### `ByStringIdSpec<T>` where `T : ConstantGuidEntity`

Queries a single entity by a string identifier that can be either an `int` or a `Guid`.

```csharp
// Parse-or-throw — throws ArgumentException for unrecognised formats
var spec = new ByStringIdSpec<MyEntity>("42");
var spec = new ByStringIdSpec<MyEntity>("3fa85f64-...");

// Safe-parse mode — sets QueryIsEmpty = true instead of throwing
var spec = new ByStringIdSpec<MyEntity>("bad-value", tryParse: true);
if (!spec.QueryIsEmpty) { /* run query */ }

// Explicit typed constructors
var spec = new ByStringIdSpec<MyEntity>(someGuid);
var spec = new ByStringIdSpec<MyEntity>(someIntId);

// Try-pattern
if (ByStringIdSpec<MyEntity>.TryBuild(stringId, out var spec))
    var entity = await repo.FirstOrDefaultAsync(spec);
```

| Property | Type | Description |
|---|---|---|
| `QueryIsEmpty` | `bool` | `true` when `tryParse: true` was used and the value was not parseable |

### `ByStringIdsSpec<T>` where `T : ConstantGuidEntity`

Bulk lookup by a collection of string ids; tolerates mixed Guid / int values.

```csharp
var spec = new ByStringIdsSpec<MyEntity>(new[] { "1", "3fa8...", "bad" });
if (spec.WrongFormat)
    Console.WriteLine("Unrecognised: " + string.Join(",", spec.WrongFormatList));

var entities = await repo.ListAsync(spec);
```

| Property | Description |
|---|---|
| `WrongFormat` | `true` if any id could not be parsed |
| `WrongFormatList` | Array of all unrecognisable id strings |

Also accepts `IEnumerable<Guid>` or `IEnumerable<int>` directly.

---

## Validators — FluentValidation async validators

### `StringIdExistValidator<TDto, TEntity>`

`AsyncPropertyValidator` that checks whether a string id exists in the database via `IReadRepositoryBase<TEntity>`.

```csharp
// Constructor options
new StringIdExistValidator<MyDto, MyEntity>(
    repository,
    validateOnlyIfNotEmpty: false,  // skip check for blank strings
    newGuid: null);                 // treat this Guid as 'always valid' (new-entity scenario)
```

**Extension methods** — attach to a FluentValidation rule builder:

```csharp
// Single property
RuleFor(x => x.EntityId).StringIdExist<MyDto, MyEntity>(repository);
RuleFor(x => x.EntityId).StringIdExist<MyDto, MyEntity>(repository, newGuid: someGuid);

// Collection element
RuleForEach(x => x.EntityIds).StringIdExist<MyDto, MyEntity>(repository);
```

The extension chains `StirngIdFormatValidator` first (stops on format error) then `StringIdExistValidator`.  
Error code: `StringIdDoesNotExist`.

### `IdExistValidator<TDto, TEntity>`

Same concept for plain `int` ids. Looks up the entity using `IReadRepositoryBase<TEntity>.GetByIdAsync`.

**Extension methods:**

```csharp
RuleFor(x => x.Id).IdExist<MyDto, MyEntity>(repository);
RuleForEach(x => x.Ids).StringIdExist<MyDto, MyEntity>(repository);
```

The extension chains `IdFormatValidator` first.  
Error code: `IdDoesNotExist`.

---

## Handlers

### `DbUpdateExceptionHandler`

Extension method on `DbUpdateException` that converts database errors to `Ardalis.Result` without leaking raw exceptions to the caller.

```csharp
try
{
    await _context.SaveChangesAsync();
}
catch (DbUpdateException ex)
{
    return ex.ToResult<MyResponseDto>(_logger, new[]
    {
        new UXConfig { UX_name = "IX_Tags_Name", UX_fields = new[] { "Name" } }
    });
}
```

| Postgres error | Result |
|---|---|
| `23505` (unique violation) + matching `UXConfig` | `Result.Conflict("A record with this identifier (...) already exists.")` |
| `DbUpdateConcurrencyException` | `Result.Conflict("The record has been modified by another user.")` |
| Anything else | Re-throws the original exception |
