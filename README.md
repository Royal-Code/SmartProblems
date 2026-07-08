# SmartProblems

SmartProblems standardizes success/failure handling with `Result`/`Result<T>` and error modeling with `Problem`/`Problems`.
It integrates APIs (ProblemDetails per RFC 9457), EF (`FindResult`/`TryFind*`) and HttpClient (`ToResultAsync`).

Target frameworks: .NET 8, .NET 9, .NET 10.

## Packages/Projects

- Core
  - `RoyalCode.SmartProblems` (Core: `Problem`, `Problems`, `Result`, `Result<T>`, `FindResult`)
- Integrations and extensions
- `RoyalCode.SmartProblems.Conversions` (Conversions to/from extended `ProblemDetails` per RFC 9457)
  - `RoyalCode.SmartProblems.ProblemDetails` (description and configuration of problem types)
  - `RoyalCode.SmartProblems.EntityFramework` (methods `TryFind*` and integration with `FindResult`)
  - `RoyalCode.SmartProblems.Http` (utilities for ASP.NET)
  - `RoyalCode.SmartProblems.ApiResults` (results for APIs)
  - `RoyalCode.SmartProblems.FluentValidation` (integration with FluentValidation)

> Note: The names above follow the repository folders/projects. Publish/consume via NuGet according to your strategy.

## Main Concepts

- `Problem`/`Problems`: categorized errors with `detail`, `property`, `extensions`
- `Result`/`Result<T>`: success/failure results with composition (`Map`, `Continue`, `Match/MatchAsync`)
- `FindResult<T>`: safe lookups with standardized `NotFound`/`InvalidParameter`
- Categories: `NotFound`(404), `InvalidParameter`(400), `ValidationFailed`(422), `NotAllowed`(403), `InvalidState`(409), `InternalServerError`(500), `CustomProblem`

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

Problems and Results:

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

Composition:

```csharp
 Result<User> GetUser() => Problems.NotFound("User");

 var res = GetUser()
     .Match(
         user => Result.Ok(),
         errs => errs.AsResult());
```

## APIs + ProblemDetails (RFC 9457)

Convert `Problems` to `ProblemDetails`:

```csharp
 using RoyalCode.SmartProblems.Conversions;

 var options = new ProblemDetailsOptions();
 Problems problems = Problems.InvalidParameter("Invalid", "prop") + Problems.NotFound("Not found");
 var pd = problems.ToProblemDetails(options);
```

Support:
- Aggregation in `ProblemDetailsExtended`
- Category-to-status mapping
- `ProblemDetailsOptions` descriptors for custom types (`typeId`)

API Matches for Minimal APIs:

```csharp
// Created (201) with Location or Problems
return service.Create(input)
  .Map(dto => new Dto(dto))
  .CreatedMatch(d => $"/api/items/{d.Id}");

// Ok (200) or ProblemDetails
return service.Update(id, cmd); // OkMatch

// NoContent (204) or ProblemDetails
return service.Delete(id); // NoContentMatch
```

## Entity Framework: TryFind/FindResult

Safe lookups with standardized problems:

```csharp
 // by Id
 var entry = await db.TryFindAsync<YourEntity, int>(id);
 if (entry.NotFound(out var notFound))
     return notFound; // implicit Result/Problems

 // by expression
 var byName = await db.TryFindByAsync<YourEntity>(e => e.Name == name);
 var result = byName.ToResult();
```

Compose with `FindResult<T>`:

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

Define domain problems with `typeId` and describe them for ProblemDetails (RFC 9457):

```csharp
 var custom = Problems.Custom("Order on hold", typeId: "order-on-hold", property: "status");

 var options = new ProblemDetailsOptions();
 options.Descriptor.Add(new ProblemDetailsDescription(
     "order-on-hold",
     title: "Order on hold",
     description: "Business rule violation",
     status: System.Net.HttpStatusCode.Conflict));
```

## FluentValidation (optional)

Convert validation errors to `Problems.ValidationFailed`/`InvalidParameter` and return as `ProblemDetails`.

## HttpClient: ToResultAsync

Deserialize responses into `Result`/`Result<T>`:

```csharp
var resp = await http.GetAsync("/users/123");
var result = await resp.ToResultAsync<UserDto>();
if (result.HasValue(out var user)) { /* success */ }
else if (result.HasProblems(out var problems)) { /* handle ProblemDetails */ }
```

## Usage Tips

- Use `With(key, value)` to attach context data
- `ChainProperty("parent")` for nested property paths
- Prefer `Result`/`FindResult` composition over exceptions in expected flows
- Configure `ProblemDetailsOptions` with absolute `type` URIs (RFC 9457)

## Development

- Multi-target: .NET 8, 9, 10
- Tests cover Results/Problems, ProblemDetails conversion, EF lookups, Http ToResultAsync, API Matches

Contributions welcome.
