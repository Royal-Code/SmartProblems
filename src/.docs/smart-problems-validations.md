# SmartProblems + SmartValidations AI Playbook

> Guia orientado para IA consumir `RoyalCode.SmartProblems` e `RoyalCode.SmartValidations` em outros projetos/repos.
>
> Objetivo: gerar código .NET consistente, composable e compatível com as APIs reais dos repositórios atuais.

---

## 1) Mental model

Use estas libs para modelar sucesso/falha sem exceções em fluxo esperado.

- `Problem`: uma falha categorizada, com `Detail`, `Property`, `TypeId` e `Extensions`.
- `Problems`: coleção de `Problem`, agregável com `+`, conversível para `Result`.
- `Result` / `Result<T>`: resultado de operação, com sucesso ou problemas.
- `FindResult<TEntity>` / `FindResult<TEntity,TId>`: resultado de busca, com entidade encontrada ou problema padronizado.
- `Rules` / `RuleSet` / `IValidable`: validação fluente que produz `Problems`.
- Integrações: EF Core, ASP.NET Minimal APIs/MVC, ProblemDetails RFC 9457, HttpClient e FluentValidation.

Regra de ouro para IA: prefira `Result`, `FindResult` e `Problems` para validação, not-found e regras de domínio. Use exceções para falhas inesperadas ou bordas que exigem exception.

---

## 2) Pacotes/projetos e quando usar

- `RoyalCode.SmartProblems`
  - Núcleo: `Problem`, `Problems`, `Result`, `Result<T>`, `FindResult`, `Id<TEntity,TId>`, `DetailBuilder`.
- `RoyalCode.SmartValidations`
  - Validação fluente: `Rules`, `RuleSet`, `IValidable`, `ValidateFunc`.
  - Depende de `RoyalCode.SmartProblems`.
- `RoyalCode.SmartProblems.EntityFramework`
  - EF Core: `TryFindAsync`, `TryFindByAsync`, `AddTo`, `SaveChanges`, `RemoveFromAsync`.
- `RoyalCode.SmartProblems.ProblemDetails`
  - Descrição/configuração de `ProblemDetails`: `AddProblemDetailsDescriptions`, `ProblemDetailsDescription`, descriptors, `MapProblemDetailsDescriptionPage`.
- `RoyalCode.SmartProblems.ApiResults`
  - Minimal API e MVC: `OkMatch`, `CreatedMatch`, `NoContentMatch`, `MatchErrorResult`, `ToActionResult`, `WithExceptionFilter`.
- `RoyalCode.SmartProblems.Conversions`
  - Conversão para/de `ProblemDetails` e helpers de pointer.
- `RoyalCode.SmartProblems.Http`
  - Cliente HTTP: `HttpResponseMessage.ToResultAsync`, `JsonFailureTypeReader<T>`.
- `RoyalCode.SmartProblems.FluentValidation`
  - Ponte com FluentValidation: `ToProblems`, `ToResult`, `EnsureIsValid`, `Validate`, `WithExtension`.

Usings comuns:

```csharp
using RoyalCode.SmartProblems;
using RoyalCode.SmartProblems.Entities;
using RoyalCode.SmartValidations;
```

Integrações usam namespaces de extensão:

```csharp
using Microsoft.EntityFrameworkCore;     // EF extensions
using RoyalCode.SmartProblems.HttpResults; // OkMatch/CreatedMatch/NoContentMatch
```

---

## 3) Convenções para IA

1. Serviços de aplicação retornam `Task<Result>` ou `Task<Result<T>>`.
2. Factories de domínio retornam `Result<T>` quando podem falhar.
3. DTOs/requests implementam `IValidable` quando usam `RoyalCode.SmartValidations`.
4. Buscas retornam ou consomem `FindResult<TEntity>` / `FindResult<TEntity,TId>`.
5. Minimal APIs retornam `OkMatch<T>`, `CreatedMatch<T>`, `OkMatch` ou `NoContentMatch`.
6. Use `Problems.InvalidParameter` para entrada inválida e `Problems.InvalidState`/`ValidationFailed` para regra/estado de domínio.
7. Use `Problems.NotFound` para recurso ausente.
8. Use `Problems.Custom(detail, typeId, property)` quando o erro precisa de um tipo de ProblemDetails específico.
9. Prefira overloads com `TParam` e callbacks `static` quando precisar passar contexto sem closures.
10. Não retorne `null` como sucesso de `Result<T>`: o construtor de `Result<T>` exige valor não nulo. Para ausência, use `FindResult<T>` ou `Problems.NotFound`.

---

## 4) Problem / Problems

### Categorias

Use a factory mais específica:

| Categoria | Factory | Uso típico |
|---|---|---|
| `NotFound` | `Problems.NotFound(...)` | Recurso/entidade não encontrada |
| `InvalidParameter` | `Problems.InvalidParameter(...)` | Entrada, parâmetro, body, query ou comando inválido |
| `ValidationFailed` | `Problems.ValidationFailed(...)` | Validação semântica ou regra de negócio violada |
| `NotAllowed` | `Problems.NotAllowed(...)` | Operação proibida para usuário/contexto/política |
| `InvalidState` | `Problems.InvalidState(...)` | Estado atual impede a operação |
| `InternalServerError` | `Problems.InternalError(...)` | Exceção inesperada |
| `CustomProblem` | `Problems.Custom(...)` | Tipo de problema próprio, com `typeId` |

Exemplos:

```csharp
var p1 = Problems.InvalidParameter("Name is required.", nameof(request.Name));
var p2 = Problems.NotFound("Contact not found.", nameof(id)).With("id", id);
var p3 = Problems.InvalidState("A contact with this e-mail already exists.")
    .With("email", normalizedEmail);

var custom = Problems.Custom(
    "Order cannot be shipped while payment is pending.",
    typeId: "order-payment-pending",
    property: "status");
```

Regra para IA: não use `Custom` se uma categoria padrão expressa bem o erro. Reserve `Custom` para erros que o contrato HTTP precisa documentar por `typeId`.

### Enriquecimento e propriedades

```csharp
var problem = Problems.InvalidParameter("Invalid format.", nameof(request.Email))
    .With("pattern", "simple-email")
    .With("hint", "Use a valid e-mail address.");

problem.ChainProperty("Customer");     // Customer.Email
problem.ChainProperty("Items", 2);     // Items[2].Email
problem.ReplaceProperty("payload.email");
```

Evite alterar `Property` manualmente; use `ChainProperty` e `ReplaceProperty`.

### Agregação

```csharp
Problems problems = Problems.InvalidParameter("Name is required.", nameof(Name));
problems += Problems.InvalidParameter("Email is required.", nameof(Email));

return problems;
```

`Problems` também pode ser criado vazio com collection expression:

```csharp
Problems errors = [];
errors += Problems.InvalidParameter("Invalid id.", nameof(id));
```

### DetailBuilder

Use quando houver muitos metadados:

```csharp
return Problems.Detail("Stock is not enough.")
    .WithProperty(nameof(request.Quantity))
    .WithTypeId("stock-not-enough")
    .With("productId", product.Id)
    .With("available", product.Stock)
    .InvalidState();
```

Métodos finais do builder: `NotFound`, `InvalidParameter`, `ValidationFailed`, `InvalidState`, `NotAllowed`, `Custom`, `Build(category)`.

---

## 5) Result / Result<T>

### Retorno curto

```csharp
public Result<ContactDetails> ToDetails(Contact contact)
{
    return new ContactDetails
    {
        Id = contact.Id,
        Name = contact.Name,
        Email = contact.Email
    };
}

public Result<ContactDetails> Fail(int id)
{
    return Problems.NotFound("Contact not found.", nameof(id)).With("id", id);
}
```

Conversões implícitas úteis:

- `T -> Result<T>`
- `Problem -> Result` e `Result<T>`
- `Problems -> Result` e `Result<T>`
- `Exception -> Result` e `Result<T>` via `Problems.InternalError(ex)`
- `Result<T> -> Result`
- `Result/Result<T> -> Task<Result>/Task<Result<T>>`

### Leitura segura

```csharp
if (result.HasProblems(out var problems))
    return problems;

if (valued.HasProblemsOrGetValue(out var valueProblems, out var value))
    return valueProblems;

// Use em testes/bootstrap/bordas; lança se falhar.
valued.EnsureHasValue(out var entity);
```

Regra para IA: em serviço de aplicação, prefira `HasProblemsOrGetValue` a `EnsureHasValue`. Use `Ensure*` quando exception for aceitável no ponto de integração.

### Map, Continue e Match

`Map` transforma sucesso. `Continue` executa uma ação/regra no sucesso. `Match` sai do mundo `Result`.

```csharp
return create.Validate()
    .Map(valid => Contact.Create(valid.Name, valid.Email, valid.Phone))
    .Map(contact => contact.ToDetails());
```

Para preservar o valor original depois de uma regra:

```csharp
return productResult
    .Continue(product => product.Activate())
    .Map(product => product.ToDetails());
```

Com `TParam`:

```csharp
return result.Map(config, static (contact, cfg) =>
{
    return new ContactDetails
    {
        Id = contact.Id,
        Name = cfg.NormalizeDisplayName(contact.Name),
        Email = contact.Email
    };
});
```

Com async e cancellation:

```csharp
return await result.ContinueAsync(
    db,
    ct,
    static async (contact, ctx, token) =>
    {
        ctx.Update(contact);
        await ctx.SaveChangesAsync(token);
        return Result.Ok();
    });
```

### Match

```csharp
var message = result.Match(
    onSuccess: contact => $"Contact {contact.Id} saved.",
    onFailure: problems => string.Join("; ", problems.Select(p => p.Detail)));
```

Use `Match` no fim do pipeline, para logs, métricas, DTO alternativo ou integração que não aceita `Result`.

---

## 6) FindResult<TEntity> / FindResult<TEntity,TId>

`FindResult` representa busca segura: entidade encontrada ou problema.

Importante: nesta versão, `TryFindAsync` por id usa `Id<TEntity,TId>`, não `id` cru.

```csharp
Id<Contact, int> contactId = id;
FindResult<Contact, int> found = await db.TryFindAsync(contactId, ct);

return found.ToResult();
```

### NotFound vs InvalidParameter

```csharp
if (found.NotFound(out var notFound))
    return notFound; // 404

if (found.HasInvalidParameter(out var invalid, nameof(id)))
    return invalid; // 400
```

Use `NotFound` quando o recurso solicitado não existe. Use `HasInvalidParameter`/`ToResult(parameterName)` quando a ausência deve ser tratada como parâmetro inválido.

```csharp
Result<Contact> as404 = found.ToResult();
Result<Contact> as400 = found.ToResult(nameof(id));
```

### Collect, Continue e Map

`Collect` executa ação e retorna `Result`.

```csharp
return await db.TryFindAsync(contactId, ct)
    .CollectAsync(db, ct, static async (contact, ctx, token) =>
    {
        ctx.Remove(contact);
        await ctx.SaveChangesAsync(token);
    });
```

`Continue` executa uma operação que retorna `Result` e preserva a entidade como `Result<TEntity>` quando a regra passa.

```csharp
return await db.TryFindAsync(contactId, ct)
    .ContinueAsync(update, ct, static (contact, request, token) =>
    {
        contact.ChangeName(request.Name);
        return Task.FromResult(Result.Ok());
    });
```

`Map` transforma entidade encontrada em outro valor.

```csharp
return await db.TryFindAsync(contactId, ct)
    .MapAsync(static contact => contact.ToDetails());
```

### FindResult<TEntity> manual

Use em repositórios sem EF:

```csharp
public FindResult<Contact, int> FindContact(int id)
{
    var contact = contacts.FirstOrDefault(c => c.Id == id);
    return new FindResult<Contact, int>(contact, id);
}
```

Para filtro nomeado:

```csharp
return FindResult<Contact>.Problem(
    byName: "Email",
    propertyName: nameof(email),
    propertyValue: email);
```

---

## 7) Entity Framework

Usar pacote `RoyalCode.SmartProblems.EntityFramework`.

### Busca por id

```csharp
Id<Contact, int> contactId = id;
var found = await db.TryFindAsync(contactId, ct);
return found.ToResult();
```

Também funciona em `DbSet<TEntity>`:

```csharp
var found = await db.Contacts.TryFindAsync(contactId, ct);
```

### Busca por filtro

```csharp
var found = await db.TryFindByAsync<Contact>(
    c => c.Email == normalizedEmail,
    ct);

return found.ToResult();
```

### Busca por filtro nomeado

Use quando quiser controlar o nome exibido e o campo de extensão:

```csharp
var found = await db.Contacts.TryFindByAsync(
    c => c.Email == normalizedEmail,
    byName: "Email",
    propertyName: nameof(email),
    propertyValue: email,
    ct);
```

### Busca por seletor de propriedade

```csharp
var found = await db.TryFindByAsync<Contact, string>(
    c => c.Email,
    normalizedEmail,
    ct);
```

Regra para IA: use seletor (`c => c.Email`) para igualdade simples; use filtro nomeado quando a mensagem/extensões precisam de nomes diferentes.

### Persistência encadeada

```csharp
return Contact.Create(request.Name, request.Email)
    .AddTo(db)
    .SaveChangesAsync(db, ct);
```

Update explícito:

```csharp
var result = await db.TryFindAsync(contactId, ct)
    .ContinueAsync(request, ct, static (contact, update, token) =>
    {
        contact.ChangeName(update.Name);
        return Task.FromResult(Result.Ok());
    });

return await result.SaveChangesAsync(db, ct);
```

Delete:

```csharp
var found = await db.TryFindAsync(contactId, ct);
if (found.NotFound(out var notFound))
    return notFound;

db.Remove(found.Entity);
await db.SaveChangesAsync(ct);
return Result.Ok();
```

`RemoveFromAsync` existe para `Task<FindResult<TEntity>>`, comum em `TryFindByAsync`:

```csharp
return await db.Contacts
    .TryFindByAsync(c => c.Id == id, ct)
    .RemoveFromAsync(db, ct)
    .SaveChangesAsync(db, ct);
```

---

## 8) SmartValidations

Use `RoyalCode.SmartValidations` para validação fluente que produz `Problems.InvalidParameter`.

### Padrão canônico com IValidable

```csharp
using RoyalCode.SmartProblems;
using RoyalCode.SmartValidations;
using System.Diagnostics.CodeAnalysis;

public sealed class CreateContactRequest : IValidable
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public string? Phone { get; set; }

    public bool HasProblems([NotNullWhen(true)] out Problems? problems)
    {
        return Rules.Set<CreateContactRequest>()
            .NotEmpty(Name)
            .MaxLength(Name, 160)
            .NotEmpty(Email)
            .Email(Email)
            .MaxLength(Email, 320)
            .NullOrMaxLength(Phone, 30)
            .HasProblems(out problems);
    }
}
```

Consumo:

```csharp
public async Task<Result<Contact>> CreateAsync(CreateContactRequest request, CancellationToken ct)
{
    if (request.HasProblems(out var problems))
        return problems;

    var contact = new Contact(request.Name, request.Email, request.Phone);
    db.Contacts.Add(contact);
    await db.SaveChangesAsync(ct);
    return contact;
}
```

Ou:

```csharp
public async Task<Result<Contact>> CreateAsync(CreateContactRequest request, CancellationToken ct)
{
    var result = request.Validate()
        .Map(valid => new Contact(valid.Name, valid.Email, valid.Phone))
        .AddTo(db);

    return await result.SaveChangesAsync(db, ct);
}
```

### RuleSet é ref struct

`RuleSet` é `readonly ref struct`. Regra para IA: crie, encadeie e consuma no mesmo método. Não armazene em campos, não capture para execução posterior e não tente atravessar `await` com uma instância viva.

---

## 9) Catálogo rápido de RuleSet

| Grupo | APIs principais |
|---|---|
| Nulo/vazio | `NotNull`, `NotEmpty`, `NullOrNotEmpty`, `BothNullOrNotEmpty` |
| Igualdade | `Equal`, `NotEqual`, `BothEqual`, `BothNotEqual` |
| Número/range | `Min`, `Max`, `MinMax`, `NullOrMin`, `NullOrMax`, `NullOrMinMax` |
| String/tamanho | `MinLength`, `MaxLength`, `Length`, `NullOrMinLength`, `NullOrMaxLength`, `NullOrLength` |
| Comparação | `LessThan`, `LessThanOrEqual`, `GreaterThan`, `GreaterThanOrEqual` |
| Pattern/string | `Matches`, `NotMatches`, `StartsWith`, `EndsWith`, `Contains`, `NotContain` |
| Formato | `Email`, `Url`, `OnlyLetters`, `OnlyDigits`, `OnlyLettersOrDigits`, `NoWhiteSpace` |
| Custom | `Must`, `Must<TValue,TParam>`, `BothMust` |
| Condicional | `When`, `Unless` |
| Aninhado | `Nested`, `NotNullNested`, `Validate`, `WithPropertyPrefix` |

Metadata produzida em `Problem.Extensions`:

- `Rules.RuleProperty` (`"rule"`): nome da regra.
- `Rules.CurrentValueProperty` (`"current"`): valor atual.
- `Rules.ExpectedValueProperty` (`"expected"`): valor esperado ou range.
- `Rules.PatternProperty` (`"pattern"`): pattern regex.
- `"properties"` e `"values"` para regras de dois operandos.

Regra para IA: `RuleSet` gera `Problems.InvalidParameter`, então é ideal para validação de entrada. Para regra de domínio que deve ser 409/422, use `Problems.InvalidState` ou `Problems.ValidationFailed` explicitamente no domínio.

---

## 10) Regras customizadas

Use `Must` para regra local simples:

```csharp
return Rules.Set<CreateUserRequest>()
    .NotEmpty(Password)
    .Must(
        Password,
        static value => value.Any(char.IsDigit) && value.Any(char.IsUpper),
        static (property, _) => $"{property} must contain a digit and an uppercase letter.",
        ruleName: "password-policy")
    .HasProblems(out problems);
```

Com parâmetro:

```csharp
return Rules.Set<OrderRequest>()
    .Must(
        Quantity,
        maxQuantity,
        static (quantity, max) => quantity <= max,
        static (property, quantity, max) => $"{property} must be less than or equal to {max}.",
        ruleName: "max-quantity")
    .HasProblems(out problems);
```

Para regra envolvendo dois campos:

```csharp
return Rules.Set<PeriodRequest>()
    .BothMust(
        Start,
        End,
        static (start, end) => start <= end,
        static (startName, endName, _, _) => $"{startName} must be before {endName}.",
        ruleName: "period-order")
    .HasProblems(out problems);
```

---

## 11) Validação condicional

`When` aplica regras se a condição for verdadeira.

```csharp
return Rules.Set<CheckoutRequest>()
    .NotEmpty(CustomerId)
    .When(RequiresShipping, s => s
        .NotNullNested(ShippingAddress))
    .HasProblems(out problems);
```

`Unless(bool, builder)` aplica regras quando a condição é falsa.

```csharp
return Rules.Set<UpdateRequest>()
    .Unless(HasAnyField, s => s
        .WithProblem(Problems.InvalidParameter("At least one field must be sent.", "request")))
    .HasProblems(out problems);
```

`Unless(conditionRules, alternativeRules)` funciona como alternativa: só adiciona problemas quando os dois grupos falham.

```csharp
return Rules.Set<LoginRequest>()
    .Unless(
        s => s.NotEmpty(Email).Email(Email),
        s => s.NotEmpty(Phone).OnlyDigits(Phone))
    .HasProblems(out problems);
```

Interpretação: informe e-mail válido ou telefone válido. Se um dos grupos passar, nenhum problema é adicionado.

---

## 12) Validação aninhada

### Objeto IValidable

```csharp
public sealed class OrderRequest : IValidable
{
    public AddressRequest? ShippingAddress { get; set; }

    public bool HasProblems([NotNullWhen(true)] out Problems? problems)
    {
        return Rules.Set<OrderRequest>()
            .NotNullNested(ShippingAddress)
            .HasProblems(out problems);
    }
}
```

`NotNullNested` adiciona problema se o objeto for nulo. `Nested` ignora nulo.

### Validação inline com prefixo

```csharp
return Rules.Set<OrderRequest>()
    .NotNullNested(ShippingAddress, address => Rules.Set<AddressRequest>()
        .WithPropertyPrefix("address")
        .NotEmpty(address.Street)
        .NotEmpty(address.City)
        .NotEmpty(address.ZipCode))
    .HasProblems(out problems);
```

`WithPropertyPrefix("address")` remove o prefixo do `CallerArgumentExpression` dentro do validador inline, para que o caminho final fique como `ShippingAddress.Street`.

### Coleções

```csharp
return Rules.Set<OrderRequest>()
    .NotEmpty(Items)
    .Nested(Items, item => Rules.Set<OrderItemRequest>()
        .WithPropertyPrefix("item")
        .NotEmpty(item.ProductId)
        .GreaterThan(item.Quantity, 0))
    .HasProblems(out problems);
```

Problemas de coleção recebem índice, por exemplo `Items[0].ProductId`.

### Value objects struct

Para structs que implementam `IValidable`, use `Validate`:

```csharp
public readonly struct Money : IValidable
{
    public decimal Amount { get; init; }
    public string Currency { get; init; }

    public bool HasProblems([NotNullWhen(true)] out Problems? problems)
    {
        return Rules.Set<Money>()
            .GreaterThanOrEqual(Amount, 0m)
            .NotEmpty(Currency)
            .HasProblems(out problems);
    }
}

return Rules.Set<CreateInvoiceRequest>()
    .Validate(Total)
    .HasProblems(out problems);
```

---

## 13) SmartValidations + serviços

Padrão explícito:

```csharp
public async Task<Result<Contact>> UpdateAsync(int id, UpdateContactRequest request, CancellationToken ct)
{
    if (request.HasProblems(out var problems))
        return problems;

    Id<Contact, int> contactId = id;
    var found = await db.TryFindAsync(contactId, ct);

    if (found.NotFound(out var notFound))
        return notFound;

    found.Entity.ChangeName(request.Name);
    await db.SaveChangesAsync(ct);
    return found.Entity;
}
```

Padrão pipeline:

```csharp
public async Task<Result<Contact>> UpdateAsync(int id, UpdateContactRequest request, CancellationToken ct)
{
    if (request.HasProblems(out var problems))
        return problems;

    Id<Contact, int> contactId = id;

    var result = await db.TryFindAsync(contactId, ct)
        .ContinueAsync(request, ct, static (contact, update, token) =>
        {
            contact.ChangeName(update.Name);
            return Task.FromResult(Result.Ok());
        });

    return await result.SaveChangesAsync(db, ct);
}
```

Regra para IA: escolha o estilo que mantém a leitura mais clara no método. Não misture pipeline e `if` em excesso no mesmo trecho.

---

## 14) Minimal APIs e ProblemDetails

### Configuração

```csharp
builder.Services.AddProblemDetailsDescriptions(options =>
{
    options.Descriptor.AddFromJsonFile("problem-details.json");
});
```

Ou em código:

```csharp
builder.Services.AddProblemDetailsDescriptions(options =>
{
    options.Descriptor.Add(new ProblemDetailsDescription(
        typeId: "order-payment-pending",
        title: "Order payment pending",
        description: "The order cannot move forward until payment is confirmed.",
        status: HttpStatusCode.Conflict));
});
```

`ProblemDetailsOptions` também permite:

```csharp
options.BaseAddress = "https://api.example.com/problems";
options.TypeComplement = "/";
options.DescriptionFiles = ["problem-details.json"];
```

### Página de documentação dos tipos

Use `MapProblemDetailsDescriptionPage` quando a API deve expor uma página HTML com o catálogo dos tipos RFC 9457 conhecidos pela aplicação.

```csharp
var app = builder.Build();

app.MapProblemDetailsDescriptionPage(); // GET /.problems
```

Rota customizada:

```csharp
app.MapProblemDetailsDescriptionPage("/docs/problems");
```

A página é opt-in: só existe se o app mapear explicitamente o endpoint. Ela lista descrições genéricas e customizadas configuradas no `ProblemDetailsDescriptor`, resolve a URI final do tipo, mostra status HTTP, origem da URI e anchors navegáveis por `TypeId`.

Regras para IA:

- Chame `AddProblemDetailsDescriptions` nos serviços antes de mapear a página.
- Use `MapProblemDetailsDescriptionPage()` para a rota padrão `/.problems`.
- Use o overload com `pattern` quando o projeto já reserva `/.problems` para outra finalidade ou prefere rota de documentação.
- Para `ProblemDetailsDescription` com `Type` explícito, a página preserva essa URI.
- Para descrição sem `Type`, a URI é gerada com `BaseAddress + TypeComplement + TypeId`; se `BaseAddress` estiver no default da lib, a página usa a própria rota documentada como base navegável.

### Handlers

Create:

```csharp
private static async Task<CreatedMatch<ContactDetails>> CreateContact(
    CreateContactRequest request,
    ContactsService service,
    CancellationToken ct)
{
    return (await service.CreateAsync(request, ct))
        .Map(static contact => contact.ToDetails())
        .CreatedMatch(static details => $"/api/contacts/{details.Id}");
}
```

Get:

```csharp
private static async Task<OkMatch<ContactDetails>> GetContact(
    int id,
    ContactsService service,
    CancellationToken ct)
{
    return (await service.GetByIdAsync(id, ct))
        .Map(static contact => contact.ToDetails());
}
```

Update sem payload específico pode usar `OkMatch`; delete normalmente usa `NoContentMatch`.

```csharp
private static async Task<NoContentMatch> DeleteContact(
    int id,
    ContactsService service,
    CancellationToken ct)
{
    return await service.DeleteAsync(id, ct);
}
```

Regra para IA: em Minimal API, não monte `ProblemDetails` manualmente. Retorne `Result`/`Problems` através de `OkMatch`, `CreatedMatch`, `NoContentMatch` ou `MatchErrorResult`.

### Filtro de exceção

```csharp
group.MapPost("/", CreateContact)
    .WithExceptionFilter(LogLevel.Error);
```

Use para exceções inesperadas. Não use `try/catch` ou filtro de exceção para validação esperada.

---

## 15) MVC Controllers

Use `ToActionResult`:

```csharp
[HttpGet("{id:int}")]
public async Task<ActionResult<ContactDetails>> Get(int id, CancellationToken ct)
{
    var result = (await service.GetByIdAsync(id, ct))
        .Map(static contact => contact.ToDetails());

    return result.ToActionResult();
}
```

Create com location:

```csharp
return result.ToActionResult(details => $"/api/contacts/{details.Id}");
```

---

## 16) HttpClient

Use `RoyalCode.SmartProblems.Http`.

```csharp
var response = await http.GetAsync(url, ct);
var result = await response.ToResultAsync<ContactDetails>(token: ct);

if (result.HasProblems(out var problems))
    return problems;

return result;
```

Se a API externa retorna `application/problem+json`, `ToResultAsync` lê os problemas automaticamente.

Para erro não-ProblemDetails em JSON:

```csharp
private sealed class ExternalErrorReader : JsonFailureTypeReader<ExternalError>
{
    protected override Problems Map(ExternalError response)
    {
        return Problems.InvalidParameter(response.Message ?? "External API error.");
    }
}

var result = await response.ToResultAsync<ContactDetails>(
    failureTypeReader: new ExternalErrorReader(),
    options: jsonOptions,
    token: ct);
```

Também há overload com `JsonTypeInfo<T>` para source generation:

```csharp
var result = await response.ToResultAsync(AppJsonContext.Default.ContactDetails, ct);
```

---

## 17) FluentValidation bridge

Use `RoyalCode.SmartProblems.FluentValidation` quando o projeto já usa FluentValidation.

```csharp
var validation = validator.Validate(model);
if (validation.HasProblems(out var problems))
    return problems;
```

Outros padrões:

```csharp
Result result = validator.Validate(model).ToResult();
Result<MyModel> validModel = validator.EnsureIsValid(model);

return createResult.Validate(validator);
return await createResult.ValidateAsync(validator);
```

`Severity.Warning` e `Severity.Info` são ignorados na conversão. Por padrão, erros viram `ProblemCategory.InvalidParameter`.

Configuração global da ponte:

```csharp
ValidationsExtensions.Options.Category = ProblemCategory.ValidationFailed;
ValidationsExtensions.Options.IncludeErrorCode = true;
ValidationsExtensions.Options.ErrorCodeExtensionField = "error_code";
```

Regra para IA: não altere `ValidationsExtensions.Options` globalmente em biblioteca compartilhada sem orientação do projeto consumidor.

Extensões customizadas:

```csharp
RuleFor(x => x.Email)
    .NotEmpty()
    .WithErrorCode("email-required")
    .WithExtension(ext => ext
        .Add("hint", "Provide a valid contact e-mail.")
        .Add("source", "CreateContactRequest"));
```

---

## 18) ProblemDetails e typeId

Para problemas customizados:

```csharp
return Problems.Custom(
    "Order cannot be cancelled after shipping.",
    typeId: "order-already-shipped",
    property: "status");
```

Configure a descrição:

```csharp
options.Descriptor.Add(new ProblemDetailsDescription(
    typeId: "order-already-shipped",
    title: "Order already shipped",
    description: "The order was already shipped and can no longer be cancelled.",
    status: HttpStatusCode.Conflict));
```

Sem descrição específica, `CustomProblem` cai no tipo genérico `problem-occurred` com status default. Para contrato de API estável, sempre descreva `typeId` customizado.

---

## 19) Exceções inesperadas

```csharp
try
{
    await externalGateway.SendAsync(command, ct);
    return Result.Ok();
}
catch (Exception ex)
{
    return Problems.InternalError(ex);
}
```

Configuração:

```csharp
Problems.ExceptionOptions.UseExceptionMessageAsDetail = false;
Problems.ExceptionOptions.IncludeExceptionTypeName = true;
Problems.ExceptionOptions.IncludeStackTrace = false;
Problems.ExceptionHandler = new MyExceptionHandler();
```

Regra para IA: use `InternalError` em bordas de infraestrutura. Não transforme regra esperada em exception só para depois converter em `Problem`.

---

## 20) Receitas rápidas

### DTO validável

```csharp
public sealed class UpdateContactRequest : IValidable
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public bool ClearPhone { get; set; }

    private bool HasAnyField => Name is not null || Email is not null || Phone is not null || ClearPhone;

    public bool HasProblems([NotNullWhen(true)] out Problems? problems)
    {
        return Rules.Set<UpdateContactRequest>()
            .Unless(HasAnyField, s => s.WithProblem(
                Problems.InvalidParameter("At least one field must be sent.", "request")))
            .NullOrMaxLength(Name, 160)
            .When(Email is not null, s => s.Email(Email).MaxLength(Email, 320))
            .NullOrMaxLength(Phone, 30)
            .When(Phone is not null && ClearPhone, s => s.WithProblem(
                Problems.InvalidParameter("Use either Phone or ClearPhone.", nameof(ClearPhone))))
            .HasProblems(out problems);
    }
}
```

### Create com validação e EF

```csharp
public async Task<Result<Contact>> CreateAsync(CreateContactRequest request, CancellationToken ct)
{
    if (request.HasProblems(out var problems))
        return problems;

    var normalizedEmail = request.Email.Trim().ToLowerInvariant();
    var exists = await db.Contacts.AnyAsync(c => c.Email == normalizedEmail, ct);

    if (exists)
        return Problems.InvalidState("A contact with this e-mail already exists.")
            .With("email", normalizedEmail);

    var contact = new Contact(request.Name, normalizedEmail, request.Phone);
    db.Contacts.Add(contact);
    await db.SaveChangesAsync(ct);
    return contact;
}
```

### Get por id

```csharp
public async Task<Result<ContactDetails>> GetAsync(int id, CancellationToken ct)
{
    if (id <= 0)
        return Problems.InvalidParameter("Id must be greater than zero.", nameof(id)).With("id", id);

    Id<Contact, int> contactId = id;

    return await db.TryFindAsync(contactId, ct)
        .MapAsync(static contact => contact.ToDetails());
}
```

### Delete

```csharp
public async Task<Result> DeleteAsync(int id, CancellationToken ct)
{
    if (id <= 0)
        return Problems.InvalidParameter("Id must be greater than zero.", nameof(id)).With("id", id);

    Id<Contact, int> contactId = id;

    var found = await db.TryFindAsync(contactId, ct);
    if (found.NotFound(out var notFound))
        return notFound;

    db.Remove(found.Entity);
    await db.SaveChangesAsync(ct);
    return Result.Ok();
}
```

### Regra de domínio

```csharp
public Result Activate()
{
    if (IsActive)
        return Problems.InvalidState("Contact is already active.", nameof(IsActive));

    IsActive = true;
    return Result.Ok();
}
```

---

## 21) Do / Don't para IA

### Do

- Use `Result`/`Result<T>` em serviços e casos de uso.
- Use `FindResult` para buscas que podem não encontrar entidade.
- Use `Rules.Set<T>()` + `IValidable` para validação de requests/DTOs.
- Use `Problem.Property` com `nameof(...)`.
- Use `With(...)` para metadados úteis e estáveis.
- Use `Id<TEntity,TId>` com `TryFindAsync`.
- Use `OkMatch`, `CreatedMatch` e `NoContentMatch` em Minimal APIs.
- Use `Problems.Custom` com `typeId` descrito em ProblemDetails quando o contrato precisar de tipo próprio.

### Don't

- Não use exception para validação, not-found ou conflito de domínio esperado.
- Não retorne `null` como sucesso em `Result<T>`.
- Não use APIs antigas `Entry<TEntity>` neste repositório; o tipo atual é `FindResult<TEntity>`.
- Não use nomes antigos como `MinOrNull`; os métodos atuais são `NullOrMin`, `NullOrMax`, `NullOrMinMax`.
- Não monte `ProblemDetails` manualmente no handler se os matches resolvem.
- Não mude opções globais (`Problems.ExceptionOptions`, `ValidationsExtensions.Options`) sem alinhamento arquitetural.
- Não guarde `RuleSet` em campo/propriedade; ele é `ref struct`.

---

## 22) Checklist para geração de código

1. O método deve retornar `Result` ou `Result<T>`?
2. O request implementa `IValidable`?
3. A validação usa `Rules.Set<T>()` e termina com `HasProblems(out problems)`?
4. Entrada inválida está usando `InvalidParameter`?
5. Regra/estado de domínio está usando `InvalidState`, `ValidationFailed` ou `Custom`?
6. Busca por id usa `Id<TEntity,TId>` + `TryFindAsync`?
7. Ausência deve ser 404 (`ToResult`) ou 400 (`ToResult(nameof(id))`)?
8. O pipeline preserva o tipo correto (`Result`, `Result<T>`, `FindResult`)?
9. O endpoint usa `OkMatch`, `CreatedMatch` ou `NoContentMatch`?
10. Problemas customizados têm `typeId` descrito?
11. Há necessidade de overload `TParam` para evitar closure?
12. O código evita `EnsureHasValue` em fluxo normal?

---

## 23) Referências internas úteis

SmartProblems:

- `src/RoyalCode.SmartProblems/Problem.cs`
- `src/RoyalCode.SmartProblems/Problems.cs`
- `src/RoyalCode.SmartProblems/Result'0.cs`
- `src/RoyalCode.SmartProblems/Result'1.cs`
- `src/RoyalCode.SmartProblems/AsyncResultExtensions.cs`
- `src/RoyalCode.SmartProblems/Entities/FindResult.cs`
- `src/RoyalCode.SmartProblems/Entities/Id.cs`
- `src/RoyalCode.SmartProblems/DetailBuilder.cs`
- `src/RoyalCode.SmartProblems.EntityFramework/SmartProblemsEFExtensions.TryFind.cs`
- `src/RoyalCode.SmartProblems.EntityFramework/SmartProblemsEFExtensions.EFOperations.cs`
- `src/RoyalCode.SmartProblems.ApiResults/HttpResults/HttpResultsExtensions.cs`
- `src/RoyalCode.SmartProblems.ApiResults/MvcResults/MvcResultsExtensions.cs`
- `src/RoyalCode.SmartProblems.ApiResults/Filters/ExceptionFilterExtensions.cs`
- `src/RoyalCode.SmartProblems.Http/HttpResultExtensions.cs`
- `src/RoyalCode.SmartProblems.FluentValidation/ValidationsExtensions.cs`
- `src/RoyalCode.SmartProblems.TestsApi/samples/contacts/`

SmartValidations:

- `C:\git\RoyalCode\SmartValidations\RoyalCode.SmartValidations\Rules.cs`
- `C:\git\RoyalCode\SmartValidations\RoyalCode.SmartValidations\RuleSet.cs`
- `C:\git\RoyalCode\SmartValidations\RoyalCode.SmartValidations\IValidable.cs`
- `C:\git\RoyalCode\SmartValidations\RoyalCode.SmartValidations\BuildInPredicates.cs`
- `C:\git\RoyalCode\SmartValidations\RoyalCode.SmartValidations.Tests\NestedTests.cs`
- `C:\git\RoyalCode\SmartValidations\RoyalCode.SmartValidations.Tests\NestedCollectionTests.cs`
- `C:\git\RoyalCode\SmartValidations\RoyalCode.SmartValidations.Tests\RuleSetRules\`

---

## 24) Nota final

Quando a IA gerar código em projeto consumidor, priorizar:

- clareza do fluxo;
- retorno tipado com `Result`/`Result<T>`;
- busca segura com `FindResult`;
- validação declarativa com `Rules`/`IValidable`;
- erros esperados como `Problems`;
- conversão HTTP padronizada via ApiResults/ProblemDetails;
- consistência com os padrões locais do projeto consumidor.
