# InsonusK.Shared.Mediator.ExceptionHandler

This library provides MediatR pipeline behaviors focused on mapping application exceptions and validation errors into standardized `Ardalis.Result` objects.

## Nuget packages
- [GitHub Packages](https://github.com/InsonusK/DotNet-Shared/pkgs/nuget/InsonusK.Shared.Mediator.ExceptionHandler)
- [Nuget](https://www.nuget.org/packages/InsonusK.Shared.Mediator.ExceptionHandler/)

## Core Features

- **Exception Handling Pipeline**: Intercepts unhandled exceptions (like `InvalidOperationException` or custom `ResultException`) and unexpected faults. It gracefully transforms these exceptions into unsuccessful `IResult` responses, ensuring the application handles errors consistently without crashing or spilling generic exception stacks.
- **Validation Pipeline**: Checks any request implementing `IClientActionTimeStamp` against FluentValidation rules (such as `CommandValidator`). If validation fails, it short-circuits execution and returns a typed `IResult` populated with the validation errors.
- **Ardalis Result Factory**: Uses a reflection-based factory (`ArdalisResultReflectionFactory<TResult>`) to construct `Result<T>` records dynamically, accommodating the generic nature of MediatR's `TResponse` type when returning error responses.
- **Custom Exceptions**: Incorporates `ResultException` wrapper to explicitly throw domain errors from deeper code layers and have them correctly mapped to `IResult`.

## Dependency Injection Registration

Register standard pipeline behaviors, validator scanning, and result factories to your DI container:

```csharp
services.Register(config); // From ExceptionHandlerModuleRegister
```
