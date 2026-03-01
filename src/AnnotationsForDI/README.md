# InsonusK.Shared.AnnotationsForDI

Attribute-driven automatic DI registration for `Microsoft.Extensions.DependencyInjection`.  
Instead of wiring every class by hand in `Program.cs`, decorate your service classes with `[Service]` and call a single extension method to scan and register them.

- **Target framework:** .NET 9.0
- **NuGet package id:** `InsonusK.Shared.AnnotationsForDI`

## Nuget packages
- [GitHub Packages](https://github.com/InsonusK/DotNet-Shared/pkgs/nuget/InsonusK.Shared.Models)
- [Nuget](https://www.nuget.org/packages/InsonusK.Shared.Models/)

## Contents

| Type | Description |
|---|---|
| `ServiceAttribute` | Marks a class for auto-registration with a chosen lifetime and optional interface list |
| `ServiceCollectionExtensions.AddAnnotatedServices` | Scans an assembly and registers every `[Service]`-decorated class |

## Quick start

### 1. Decorate your services

```csharp
// Register as Scoped under its own type only
[Service]
public class ReportService { ... }

// Register as Singleton under an interface
[Service(ServiceLifetime.Singleton, typeof(ICacheService))]
public class InMemoryCacheService : ICacheService { ... }

// Register as Transient under multiple interfaces
[Service(ServiceLifetime.Transient, typeof(IFooService), typeof(IBarService))]
public class FooBarService : IFooService, IBarService { ... }
```

### 2. Scan and register in `Program.cs`

```csharp
builder.Services.AddAnnotatedServices(typeof(ReportService).Assembly);

// Optional: restrict to a specific namespace
builder.Services.AddAnnotatedServices(
    typeof(ReportService).Assembly,
    namespaceFilter: "MyApp.Services");
```

## `ServiceAttribute` reference

```csharp
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class ServiceAttribute : Attribute
```

| Parameter | Type | Default | Description |
|---|---|---|---|
| `lifetime` | `ServiceLifetime` | `Scoped` | Lifetime of the registered service |
| `interfaces` | `params Type[]` | *(empty)* | Interfaces under which to register the class. The class is always additionally registered under its own type. |

## `AddAnnotatedServices` reference

```csharp
public static void AddAnnotatedServices(
    this IServiceCollection services,
    Assembly assembly,
    string? namespaceFilter = null)
```

- Scans all non-abstract classes in `assembly` that carry `[Service]`.
- If `namespaceFilter` is set, only classes whose `Namespace` starts with the filter string are considered.
- **Throws** `InvalidOperationException` when:
  - A declared interface is not implemented by the class.
  - A type or interface is already registered in the collection (duplicate guard).

## Notes

- A class is **always** registered under its own type in addition to any declared interfaces.
- The attribute is not inheritable (`AllowMultiple = false`).
