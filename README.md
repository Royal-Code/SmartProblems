# SmartProblems

SmartProblems is a small set of .NET libraries for modeling expected failures as
values instead of exceptions. It gives you `Problem`, `Problems`, `Result`,
`Result<T>`, `FindResult<T>`, RFC 9457 `ProblemDetails` conversion, Minimal API
results, Entity Framework helpers, HttpClient deserialization and
FluentValidation integration.

Use it when application code should say, plainly:

- this operation succeeded;
- this operation failed with one or more typed problems;
- this entity was not found;
- this API response should become a standard `ProblemDetails` payload.

Unexpected failures are still exceptions. Validation errors, business rules,
authorization denials, invalid state and "not found" are `Problem` values.

## Target Frameworks

The libraries target:

- .NET 8
- .NET 9
- .NET 10

The test project currently targets .NET 10.

## Packages

Install only the packages needed by the project:

```bash
dotnet add package RoyalCode.SmartProblems
dotnet add package RoyalCode.SmartProblems.EntityFramework
dotnet add package RoyalCode.SmartProblems.ProblemDetails
dotnet add package RoyalCode.SmartProblems.ApiResults
dotnet add package RoyalCode.SmartProblems.Http
dotnet add package RoyalCode.SmartProblems.FluentValidation
```

Package overview:

| Package | Main use |
|---|---|
| `RoyalCode.SmartProblems` | Core types: `Problem`, `Problems`, `Result`, `Result<T>`, `FindResult<T>`, `Id<,>` |
| `RoyalCode.SmartProblems.EntityFramework` | EF Core extensions: `TryFindAsync`, `TryFindByAsync`, `FindByCriteria`, `AddTo`, `SaveChanges`, `RemoveFromAsync` |
| `RoyalCode.SmartProblems.ProblemDetails` | RFC 9457 descriptions, options, conversion setup and problem description pages |
| `RoyalCode.SmartProblems.ApiResults` | Minimal API/MVC response helpers: `OkMatch`, `CreatedMatch`, `NoContentMatch`, `WithExceptionFilter` |
| `RoyalCode.SmartProblems.Http` | HttpClient extensions: `ToResultAsync` |
| `RoyalCode.SmartProblems.FluentValidation` | Convert FluentValidation failures to `Problems` |
| `RoyalCode.SmartProblems.Conversions` | Lower-level conversion types used by `ProblemDetails` and `Http` |

Some extension methods intentionally live in Microsoft namespaces after the
package is installed:

| Member | Namespace |
|---|---|
| EF helpers | `Microsoft.EntityFrameworkCore` |
| `.OkMatch()`, `.CreatedMatch()`, `.NoContentMatch()` | `Microsoft.AspNetCore.Http` |
| `WithExceptionFilter`, `MapProblemDetailsDescriptionPage` | `Microsoft.AspNetCore.Builder` |
| `AddProblemDetailsDescriptions` | `Microsoft.Extensions.DependencyInjection` |
| `ToResultAsync` | `System.Net.Http` |

## Core Usage

Create problems and return them as results:

```csharp
using RoyalCode.SmartProblems;

public Result<Order> Confirm(Order order)
{
    if (order.IsCanceled)
    {
        return Problems.InvalidState("Canceled orders cannot be confirmed.")
            .With("orderId", order.Id);
    }

    order.Confirm();
    return order;
}
```

Handle success and failure without exceptions:

```csharp
Result<Order> result = service.Confirm(order);

if (result.HasProblems(out var problems))
    return problems;

result.EnsureHasValue(out var confirmed);
```

Common categories and default HTTP status mapping:

| Category | HTTP status |
|---|---|
| `InvalidParameter` | 400 |
| `ValidationFailed` | 422 |
| `NotAllowed` | 403 |
| `InvalidState` | 409 |
| `NotFound` | 404 |
| `InternalServerError` | 500 |
| `CustomProblem` | From the registered description |

## Entity Framework

Install `RoyalCode.SmartProblems.EntityFramework` and use
`Microsoft.EntityFrameworkCore`.

Lookup by strongly typed id:

```csharp
using Microsoft.EntityFrameworkCore;
using RoyalCode.SmartProblems.Entities;

Id<Order, int> orderId = id;
FindResult<Order, int> find = await db.TryFindAsync(orderId, ct);

return find.ToResult();
```

Lookup by one property:

```csharp
var find = await db.Orders.TryFindByAsync(o => o.Number == number, ct);
return find.ToResult();
```

Lookup by multiple criteria:

```csharp
var find = await db.FindByCriteria<City>()
    .By(c => c.StateId, stateId)
    .By(c => c.Name, name)
    .TryFindAsync(ct);

return find.ToResult();
```

Persist through `Result` pipelines:

```csharp
return await Product.Create(command)
    .AddTo(db)
    .SaveChangesAsync(db, ct);
```

Remove only when the entity is found:

```csharp
return await db.Products
    .TryFindByAsync(p => p.Id == id, ct)
    .RemoveFromAsync(db, ct)
    .SaveChangesAsync(db, ct);
```

## Minimal APIs

Install `RoyalCode.SmartProblems.ApiResults`.

Use `OkMatch<T>`, `CreatedMatch<T>` and `NoContentMatch` to return either the
success response or a `ProblemDetails` response from the same endpoint.

```csharp
using Microsoft.AspNetCore.Http;
using RoyalCode.SmartProblems.HttpResults;

var group = app.MapGroup("/api/orders")
    .WithExceptionFilter();

group.MapGet("/{id:int}",
    async Task<OkMatch<OrderDto>> (int id, OrdersService service, CancellationToken ct)
        => await service.GetAsync(id, ct));

group.MapPost("/",
    async Task<CreatedMatch<OrderDto>> (CreateOrder command, OrdersService service, CancellationToken ct)
        => (await service.CreateAsync(command, ct))
            .CreatedMatch(order => $"/api/orders/{order.Id}"));

group.MapDelete("/{id:int}",
    async Task<NoContentMatch> (int id, OrdersService service, CancellationToken ct)
        => await service.DeleteAsync(id, ct));
```

`WithExceptionFilter` is for unexpected exceptions at the HTTP boundary. Keep
expected failures in `Result`, `Problem` or `Problems`.

## ProblemDetails and Problem Type Documentation

Install `RoyalCode.SmartProblems.ProblemDetails`.

Register descriptions for known problem types, especially custom `typeId`
values, and optionally expose an HTML catalog for consumers.

```csharp
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using RoyalCode.SmartProblems.Descriptions;

builder.Services.AddProblemDetailsDescriptions(options =>
{
    options.Descriptor.Add(new ProblemDetailsDescription(
        typeId: "order-on-hold",
        title: "Order on hold",
        description: "The order cannot move forward while it is on hold.",
        status: HttpStatusCode.Conflict));
});

app.MapProblemDetailsDescriptionPage(); // GET /.problems
```

Create a custom problem using the same `typeId`:

```csharp
return Problems.Custom(
    "The order is on hold.",
    typeId: "order-on-hold",
    property: "status");
```

## HttpClient

Install `RoyalCode.SmartProblems.Http`.

`ToResultAsync` reads successful responses as values and failed
`application/problem+json` responses as `Problems`.

```csharp
HttpResponseMessage response = await http.GetAsync("/api/orders/123", ct);
Result<OrderDto> result = await response.ToResultAsync<OrderDto>(ct: ct);

if (result.HasValue(out var order))
{
    return order;
}

return result;
```

## FluentValidation

Install `RoyalCode.SmartProblems.FluentValidation`.

```csharp
Result<CreateOrder> valid = await validator.EnsureIsValidAsync(command);

if (valid.HasProblems(out var problems))
    return problems;
```

## Documentation

Project documentation lives under `src/.docs`:

- [`src/.docs/problems.md`](src/.docs/problems.md): full guide with examples,
  conventions, edge cases and recommended patterns.
- [`src/.docs/problems.ai-rules.md`](src/.docs/problems.ai-rules.md):
  compact rules for AI-assisted code generation in other repositories.
- [`src/.docs/smart-problems-validations.md`](src/.docs/smart-problems-validations.md):
  validation-oriented examples and checklists.

Prefer the XML documentation exposed by the installed packages for exact method
signatures. The docs above explain the intended usage and composition style.

## Development

From the repository root:

```bash
dotnet test src/RoyalCode.SmartProblems.Tests/RoyalCode.SmartProblems.Tests.csproj
```

The tests cover the core result types, problem conversion, API results,
exception filters, Entity Framework helpers, HttpClient conversion,
FluentValidation integration and problem description pages.

## License

See [`LICENSE`](LICENSE).
