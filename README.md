# SmartProblems

SmartProblems is a set of libraries aimed at standardizing the handling of results and errors from operations in .NET.
It provides types and utilities to:

- Work with `Result`/`Result<T>` (success/failure with type safety)
- Model errors with `Problem` and `Problems` collections (with categories and extra data)
- Convert problems to `ProblemDetails` (RFC 7807) and return in APIs
- Extensions for Entity Framework (safe lookup with `FindResult`/`TryFind*`)
- Integrations for ASP.NET, Minimal APIs, ProblemDetails, and FluentValidation

Target projects: .NET 8, .NET 9, and .NET 10.

## Packages/Projects

- Core
  - `RoyalCode.SmartProblems` (Core: `Problem`, `Problems`, `Result`, `Result<T>`, `FindResult`)
- Integrations and extensions
  - `RoyalCode.SmartProblems.Conversions` (Conversions to/from extended `ProblemDetails`)
  - `RoyalCode.SmartProblems.ProblemDetails` (description and configuration of problem types)
  - `RoyalCode.SmartProblems.EntityFramework` (methods `TryFind*` and integration with `FindResult`)
  - `RoyalCode.SmartProblems.Http` (utilities for ASP.NET)
  - `RoyalCode.SmartProblems.ApiResults` (results for APIs)
  - `RoyalCode.SmartProblems.FluentValidation` (integration with FluentValidation)

> Note: The names above follow the repository folders/projects. Publish/consume via NuGet according to your strategy.

## Main Concepts

- `Problem`: represents an error (category, detail, property, typeId, and extensions)
- `Problems`: chainable collection of `Problem`
- `Result`/`Result<T>`: result of operations (success/failure). Matches, aggregates, maps
- `FindResult<T>`/`FindResult<T, TId>`: result of lookups (entity or NotFound/InvalidParameter problem)
- Problem categories (`ProblemCategory`): `NotFound`, `InvalidParameter`, `ValidationFailed`, `NotAllowed`, `InvalidState`, `InternalServerError`, `CustomProblem`

## Installation

Add references as needed (Core is mandatory):

```
 dotnet add package RoyalCode.SmartProblems
 dotnet add package RoyalCode.SmartProblems.Conversions
 dotnet add package RoyalCode.SmartProblems.ProblemDetails
 dotnet add package RoyalCode.SmartProblems.EntityFramework
 dotnet add package RoyalCode.SmartProblems.Http
 dotnet add package RoyalCode.SmartProblems.ApiResults
 dotnet add package RoyalCode.SmartProblems.FluentValidation
```

## Quick Start

Creating problems and results:

```csharp
 // a simple problem
 var problem = Problems.InvalidParameter("Name is required", "name");

 // Problems is chainable
 Problems problems = problem + Problems.NotFound("User not found", "userId");

 // Result with value or with problems
 Result<string> ok = "Hello";
 Result<string> fail = problem; // implicit

 // Checking
 if (ok.HasValue(out var value))
 {
     // success
 }

 if (fail.HasProblems(out var errs))
 {
     // handle errors
 }
```

Composing operations (map/continue/collect):

```csharp
 Result<User> GetUser() => Problems.NotFound("User");

 var res = GetUser()
     .Match(
         user => Result.Ok(),
         errs => errs.AsResult());
```

## ASP.NET + ProblemDetails

Convert `Problems` to `ProblemDetails` (RFC 7807):

```csharp
 using RoyalCode.SmartProblems.Conversions;

 var options = new ProblemDetailsOptions();
 Problems problems = Problems.InvalidParameter("Invalid", "prop") + Problems.NotFound("Not found");
 var pd = problems.ToProblemDetails(options);
```

Support for:
- Multiple problems aggregated in `ProblemDetailsExtended` (errors, not_found, and aggregated)
- Mapping of categories to standard HTTP status
- Customization of titles/messages and types via descriptors

## Entity Framework: TryFind/FindResult

Query safely and produce standardized problems:

```csharp
 // by Id
 var entry = await db.TryFindAsync<YourEntity, int>(id);
 if (entry.NotFound(out var notFound))
     return notFound; // implicit Result/Problems

 // by expression
 var byName = await db.TryFindByAsync<YourEntity>(e => e.Name == name);
 var result = byName.ToResult();
```

`FindResult<T>` exposes utilities to compose pipeline:

```csharp
 await entry.ContinueAsync(async entity =>
 {
     // proceed with the entity
     return Result.Ok();
 });
```

## Exception Handling

Create internal error problems with fine control:

```csharp
 var p1 = Problems.InternalError(new Exception("boom"));
 var p2 = Problems.InternalError(); // uses default message

 // global customization
 Problems.ExceptionOptions.IncludeStackTrace = true;
 Problems.ExceptionHandler = new MyHandler(); // map specific exceptions
```

## Custom Problems (CustomProblem)

Define domain problems with `typeId` and describe them for ProblemDetails:

```csharp
 var custom = Problems.Custom("Order on hold", typeId: "order-on-hold", property: "status");

 var options = new ProblemDetailsOptions();
 options.Descriptor.Add(new ProblemDetailsDescription(
     "order-on-hold",
     title: "Order on hold",
     description: "Business rule violation",
     statusCode: System.Net.HttpStatusCode.Conflict));
```

## FluentValidation (optional)

Convert validation errors to `Problems.ValidationFailed`/`InvalidParameter` and return as `ProblemDetails`.

## Usage Tips

- Use `With(key, value)` to attach extra data to problems
- `ChainProperty("parent")` helps compose the property path (e.g., collections with index)
- Prefer composition with `Result`/`FindResult` to keep flow without exceptions

## Development

- Multi-target projects: .NET 8, 9, and 10
- Tests in `RoyalCode.SmartProblems.Tests` cover:
  - `Result`/`Problems` operations
  - `Problems` factories and conversions to `ProblemDetails`
  - Integration with Entity Framework (lookup scenarios)

Feel free to open issues and PRs.
