# Problems AI Playbook v2 (consumo em outros projetos)

> Guia orientado para IA (Copilot/assistentes) consumir as bibliotecas Problems com segurança, consistência e bom estilo.
>
> Baseado no estado atual do repositório (incluindo exemplos CRUD em `OutraLib.Problems.Tests.Api/ExemplosDoc/CRUD`, testes e APIs recentes com sobrecargas `TParam`).

---

## 1) Objetivo e mental model

Use Problems para modelar fluxo de sucesso/falha sem exceções para controle de fluxo:

- **Problem / Problems**: descrevem falhas (categoria, detalhe, propriedade, extensões).
- **Result / Result<T>**: encapsulam sucesso/falha de qualquer operação.
- **Entry<TEntity> / Entry<TEntity,TId>**: encapsulam busca de entidade (encontrada ou não) e permitem continuação e.
- **Rules / RuleSet / IValidable**: validação declarativa.
- **EF + Web + HTTP integrations**: composição de persistência, endpoints e consumo HTTP em cima de `Result`.

Regra de ouro para IA: **prefira composição e com Result/Entry** em vez de `if` aninhado e exceptions para cenário esperado.

---

## 2) Pacotes/projetos e quando usar

- `OutraLib.Problems`:
  - Núcleo: `Problem`, `Problems`, `Result`, `Result<T>`, `Entry`, `Rules`, `IValidable`.
- `OutraLib.Problems.EntityFramework`:
  - `TryFindAsync`, `TryFindByAsync`, `AddTo`, `SaveChangesAsync`, `RemoveFrom` etc.
- `OutraLib.Problems.ProblemDetails`:
  - Conversão para ProblemDetails (RFC 9457), ApiResults (`OkMatch`, `CreatedMatch`, `NoContentMatch`), `ToResultAsync` para `HttpResponseMessage`.
- `OutraLib.Problems.Validation`:
  - Ponte com Validation (`ToResult`, `ToProblems`, `HasProblems`).

---

## 3) Convenções de estilo para IA (importante)

1. **Retorno padrão**
   - Serviço/aplicação: `Task<Result>` ou `Task<Result<T>>`.
   - Busca de entidade: `Task<Entry<TEntity>>` ou `Task<Entry<TEntity,TId>>`.

2. **Falhas esperadas**
   - Use `Problems.*` (`NotFound`, `InvalidParameter`, `ValidationFailed`, etc.)
   - Evite `throw` para validação/domínio esperado.

3. **Persistência EF**
   - Prefira encadeamento com extensões (`AddTo`, `SaveChangesAsync`, `RemoveFrom`) quando fizer sentido.
   - Também é aceitável padrão híbrido (checagem explícita + `Continue/Map`) quando melhora legibilidade.

4. **Evite null-forgiving (`!`) sem necessidade**
   - Use `HasProblemsOrGetValue`, `NotFound`, `HasInvalidParameter`.

5. **Sobrecargas com `TParam`**
   - Prefira quando quiser evitar closure/captura de contexto.
   - Passe dependências/contexto explicitamente para `Collect/Continue/Map`.

6. **APIs web**
   - Retorne `OkMatch` / `CreatedMatch` / `NoContentMatch` nos handlers quando usar ProblemDetails.

---

## 4) Problem / Problems (resumo prático)

### Criação

```csharp
var p1 = Problems.InvalidParameter("Nome obrigatório", property: "Nome");
var p2 = Problems.NotFound("Produto não encontrado", property: "id");
var p3 = Problems.Custom("Regra violada", typeId: "pedido-estado-invalido", property: "Status");
```

### Enriquecimento

```csharp
var p = Problems.InvalidParameter("Formato inválido", "Email")
    .With("pattern", "...")
    .With("hint", "Use um e-mail válido");
```

### Agregação

```csharp
var all = Problems.InvalidParameter("A", "campoA")
        + Problems.ValidationFailed("B", "campoB");
```

### Categorias disponíveis

Use a factory da categoria mais específica possível:

- `Problems.NotFound(...)`: recurso/entidade inexistente.
- `Problems.InvalidParameter(...)`: entrada, parâmetro ou query inválida.
- `Problems.ValidationFailed(...)`: falha de validação de modelo/regra declarativa.
- `Problems.NotAllowed(...)`: operação não permitida para o usuário/contexto atual.
- `Problems.InvalidState(...)`: estado atual do domínio não permite a operação.
- `Problems.Custom(..., typeId: ...)`: regra de negócio rastreável por tipo de ProblemDetails.
- `Problems.InternalError(exception)`: exceção inesperada convertida em problema.

Regra para IA: **não use `Custom` quando uma categoria semântica padrão resolver**. Reserve `Custom` para regras de negócio que precisam de `typeId` específico.

### DetailBuilder

Quando a montagem do problema tiver muitos dados, use `Problems.Detail(...)` / `DetailBuilder` em vez de encadear várias factories manuais:

```csharp
return Problems.Detail("Estoque insuficiente")
    .WithProperty(nameof(request.Quantidade))
    .WithTypeId("produto-estoque-insuficiente")
    .With("produtoId", produto.Id)
    .With("estoqueAtual", produto.Estoque)
    .Custom();
```

### Ajuste de propriedade

Use quando precisar adaptar problemas vindos de validação aninhada ou domínio para o nome esperado no contrato de API:

```csharp
var problem = Problems.ValidationFailed("Nome obrigatório", "Nome")
    .ChainProperty("Produto"); // Produto.Nome

var asInput = problem.AsInvalidParameter("dto.nome");
```

Evite manipular `Problem.Property` manualmente depois da criação; prefira `ChainProperty`, `ReplaceProperty` e `AsInvalidParameter`.

---

## 5) Result / Result<T> playbook

### Regras

- Sucesso: `Result.Ok()` ou retorno implícito de valor em `Result<T>`.
- Falha: retorne `Problems`/`Problem` diretamente (conversão implícita).

### Exemplo

```csharp
public async Task<Result<ProdutoDto>> ObterAsync(Guid id)
{
    var entity = await repo.FindAsync(id);
    if (entity is null)
        return Problems.NotFound("Produto não encontrado", nameof(id));

    return new ProdutoDto(entity.Id, entity.Nome);
}
```

### Composição

```csharp
var result = await CriarAsync(cmd)
    .ContinueAsync(context, async (id, db, ct) =>
    {
        await db.SaveChangesAsync(ct);
        return Result.Ok();
    }, ct);
```

### `Map`, `Continue` e `Match`

- `Map`: transforma sucesso em outro valor (`Result<TIn>` -> `Result<TOut>`), preservando falhas.
- `Continue`: executa uma operação se houve sucesso, preservando/propagando falhas.
- `Match`: encerra o pipeline e escolhe um valor final para sucesso/falha.

```csharp
return dto.Validate()
    .Map(valid => Produto.Criar(valid.Nome, valid.Preco))
    .Continue(produto => produto.Ativar())
    .Map(produto => produto.ToDetails());
```

Use `Match` quando precisar sair do mundo `Result`, por exemplo para log, métrica, texto, DTO alternativo ou integração que não aceita `Result`:

```csharp
var message = result.Match(
    onSuccess: produto => $"Produto {produto.Id} criado",
    onFailure: problems => string.Join("; ", problems.Select(p => p.Detail)));
```

### `MatchAsync` e `TParam`

Prefira sobrecargas com `TParam` quando callbacks precisarem de contexto externo:

```csharp
return await result.MatchAsync(
    logger,
    static async (produto, log) =>
    {
        log.LogInformation("Produto criado: {Id}", produto.Id);
        return produto.ToDetails();
    },
    static (problems, log) =>
    {
        log.LogWarning("Falha: {Count}", problems.Count);
        return Task.FromResult(default(ProdutoDetails));
    });
```

### Utilitários de leitura segura

Use estes métodos para evitar `!` e acesso indevido a valor/problemas:

```csharp
if (result.HasProblems(out var problems))
    return problems;

if (result.HasProblemsOrGetValue(out var p, out var value))
    return p;

result.EnsureHasValue(out var dto); // use só em bordas/testes; lança se falhar
```

Regra para IA: em código de aplicação, prefira `HasProblemsOrGetValue` a `EnsureHasValue`; reserve `Ensure*` para testes, bootstrap ou integrações onde exceção é aceitável.

---

## 6) Entry<TEntity> e Entry<TEntity,TId> (incluindo TParam novo)

`Entry` representa “entidade encontrada ou problema”.

### Padrão básico

```csharp
var entry = await db.TryFindAsync<Produto, int>(id, ct);
if (entry.NotFound(out var problems))
    return problems;

return entry.Entity;
```

### Continuação e

```csharp
return await db.TryFindAsync<Produto, int>(id, ct)
    .ContinueAsync(async produto =>
    {
        if (produto.Atualizar(nome, preco).HasProblems(out var p))
            return p;

        await db.SaveChangesAsync(ct);
        return Result.Ok();
    });
```

### Novas sobrecargas `TParam` (fortemente recomendadas para IA)

Agora `Entry<TEntity>` e `Entry<TEntity,TId>` têm sobrecargas com parâmetro extra para:

- `Collect<TParam>(...)`
- `CollectAsync<TParam>(...)`
- `Continue<TParam>(...)`
- `ContinueAsync<TParam>(...)`
- `Map<TParam, TValue>(...)`
- `MapAsync<TParam, TValue>(...)`

Exemplo (sem closure):

```csharp
return await db.Produtos.TryFindAsync<Produto, int>(id, ct)
    .CollectAsync(dto.Delta, static (produto, delta) => produto.AjustarEstoque(delta))
    .SaveChangesAsync(db, ct);
```

### Diferenças importantes entre `Entry<TEntity>` e `Entry<TEntity,TId>`

- `Entry<TEntity>` pode carregar um `Problem` customizado gerado por filtro/propriedade.
- `Entry<TEntity,TId>` gera `NotFound`/`InvalidParameter` com extensão `id` automaticamente.
- Algumas sobrecargas de `Collect` preservam a entidade (`Result<TEntity>`) em `Entry<TEntity>` e retornam apenas `Result` em `Entry<TEntity,TId>`.

Regra para IA: se a operação precisa retornar o entity atualizado, confirme o tipo do pipeline. Se só precisa sinalizar sucesso/falha em update/delete, prefira `Task<Result>`.

### `NotFound` vs `HasInvalidParameter`

Use conforme o significado HTTP/domínio desejado:

```csharp
if (entry.NotFound(out var notFound))
    return notFound; // tipicamente 404

if (entry.HasInvalidParameter(out var invalid, nameof(id)))
    return invalid; // tipicamente 400
```

Use `HasInvalidParameter` quando a busca falha por parâmetro inválido de entrada; use `NotFound` quando a ausência é o estado esperado do recurso.

---

## 7) AsyncExtensions (Task e ValueTask) com TParam

Foram adicionadas sobrecargas `TParam` também para wrappers assíncronos:

- `Task<Entry<TEntity>>` e `Task<Entry<TEntity,TId>>`
- `ValueTask<Entry<TEntity>>` e `ValueTask<Entry<TEntity,TId>>`

Suportando `CollectAsync`, `ContinueAsync`, `MapAsync` com `TParam`.

Exemplo Task:

```csharp
Task<Entry<Produto, int>> task = db.TryFindAsync<Produto, int>(id, ct);
var r = await task.ContinueAsync(dto,
    static (produto, request) => produto.AjustarEstoque(request.Delta)); // AjustarEstoque retorna Result
```

Exemplo ValueTask:

```csharp
ValueTask<Entry<Produto, int>> task = ValueTask.FromResult(new Entry<Produto, int>(produto, produto.Id));
var mapped = await task.MapAsync(config, (p, cfg) => new ProdutoDto(p.Id, p.Nome, p.Preco));
```

---

## 8) Extensões de EF Core (recomendação de uso)

Disponível em `OutraLib.Problems.EntityFramework`:

- Busca:
  - `TryFindAsync<TEntity, TId>(...)`
  - `TryFindByAsync<TEntity>(filtro...)`
  - `TryFindByAsync<TEntity, TValue>(propertySelector, value...)`
- Persistência:
  - `AddTo`, `AddToAsync`
  - `SaveChanges`, `SaveChangesAsync`
  - `RemoveFrom`, `RemoveFromAsync`

Pipeline típico:

```csharp
return dto.Validate()
    .Map(v => Categoria.Criar(v.Nome, v.Descricao))
    .AddTo(db)
    .SaveChangesAsync(db, ct);
```

> Observação: se `Validate()` não existir no projeto consumidor, use o padrão explícito `dto.HasProblems(out var p)`.

### Busca com `Id<TEntity,TId>`

Quando o projeto usa ids fortemente tipados, `TryFindAsync` aceita `Id<TEntity,TId>`:

```csharp
Id<Produto, int> produtoId = new(id);
var entry = await db.TryFindAsync(produtoId, ct);
```

### `TryFindByAsync` nomeado

Use overloads nomeados quando a mensagem de not-found precisa apontar para o parâmetro de entrada:

```csharp
return await db.Produtos.TryFindByAsync(
    p => p.Sku == sku,
    byName: "SKU",
    propertyName: nameof(sku),
    propertyValue: sku,
    ct);
```

Também existe busca por seletor de propriedade:

```csharp
var entry = await db.TryFindByAsync<Produto, string>(p => p.Sku, sku, ct);
```

Regra para IA: use `TryFindByAsync<TEntity,TValue>(selector, value)` para igualdade simples por propriedade; use filtro nomeado quando precisar controlar texto/extensões do problema.

### Remoção e

`RemoveFrom`/`RemoveFromAsync` está disponível para `Entry<TEntity>`:

```csharp
return await db.TryFindByAsync<Produto>(p => p.Id == id, ct)
    .RemoveFromAsync(db)
    .SaveChangesAsync(db, ct);
```

Se a entidade tiver regra de domínio para remoção, aplique a regra antes de remover explicitamente:

```csharp
var entry = await db.TryFindByAsync<Produto>(p => p.Id == id, ct);
if (entry.NotFound(out var notFound))
    return notFound;

if (entry.Entity.Remover().HasProblems(out var problems))
    return problems;

db.Remove(entry.Entity);
await db.SaveChangesAsync(ct);
return Result.Ok();
```

---

## 9) Validação (Rules / RuleSet / HasProblems / IValidable / Validation)

> Esta seção é crítica para IA: a maior parte do fluxo de erro deve nascer aqui.

### Quando usar `Rules` e `RuleSet`

- Use `Rules.Set<T>()` para validar DTOs/entidades de forma e.
- Use `HasProblems(out problems)` como **porta padrão** para converter validação em `Problems`.
- Em casos de serviço, valide no início do método e faça short-circuit com `return problems;`.

### Padrão canônico `HasProblems`

```csharp
public class ProdutoCreate : IValidable
{
    public required string Sku { get; set; }
    public required string Nome { get; set; }
    public decimal Preco { get; set; }
    public int Estoque { get; set; }

    public bool HasProblems([NotNullWhen(true)] out Problems? problems) =>
        Rules.Set<ProdutoCreate>()
            .NotEmpty(Sku)
            .NotEmpty(Nome)
            .Must(Preco, v => v > 0m, (campo, _) => $"O campo '{campo}' deve ser maior que zero.")
            .Min(Estoque, 0)
            .HasProblems(out problems);
}
```

### Padrão de consumo em serviço

```csharp
public async Task<Result<Produto>> CreateAsync(ProdutoCreate dto, CancellationToken ct)
{
    if (dto.HasProblems(out var problems))
        return problems;

    if (Produto.Criar(dto.Sku, dto.Nome, dto.Preco, dto.Estoque, 1, 1)
        .HasProblemsOrGetValue(out var criarProblems, out var produto))
        return criarProblems;

    db.Produtos.Add(produto);
    await db.SaveChangesAsync(ct);
    return produto;
}
```

### Regras práticas para IA

1. Prefira validação declarativa (`Rules`) a `if` manual repetitivo.
2. Sempre exponha `HasProblems(out problems)` em tipos de entrada (`IValidable`).
3. Em domínio, retorne `Result`/`Result<T>` ao validar invariantes (`Criar`, `Atualizar`, etc.).
4. Em aplicação, combine:
   - `dto.HasProblems(...)`
   - `factoryResult.HasProblemsOrGetValue(...)`
   - `Entry.NotFound(...)`

### Catálogo rápido de `Rules` para IA

Use como referência ao escolher validações declarativas:

| Grupo | APIs comuns | Quando usar |
|---|---|---|
| Nulo/vazio | `NotNull`, `NotEmpty`, `NullOrNotEmpty`, `BothNullOrNotEmpty` | Campos obrigatórios e pares opcionais |
| Igualdade | `Equal`, `NotEqual`, `BothEqual`, `BothNotEqual` | Confirmação, comparação entre campos, duplicidade local |
| Número/tamanho | `Min`, `Max`, `MinMax`, `MinOrNull`, `MaxOrNull`, `MinMaxOrNull` | Valores numéricos, datas, ranges |
| String/tamanho | `MinLength`, `MaxLength`, `Length`, `NullOrLength`, `NullOrMinLength`, `NullOrMaxLength` | Contratos de texto |
| Comparação | `LessThan`, `LessThanOrEqual`, `GreaterThan`, `GreaterThanOrEqual` | Datas, quantidades, limites |
| Custom | `Must`, `Must<TValue,TParam>` | Regra simples local sem criar validator separado |
| Condicional | `When`, `Unless` | Validar apenas sob condição |
| Aninhado | `Nested`, `NotNullNested`, `WithPropertyPrefix` | DTOs compostos e coleções |
| Formato string | `Email`, `Url`, `Matches`, `NotMatches`, `StartsWith`, `EndsWith`, `Contains`, `NotContain`, `OnlyLetters`, `OnlyDigits`, `OnlyLettersOrDigits`, `NoWhiteSpace` | Formato textual conhecido |

Exemplo compacto:

```csharp
public bool HasProblems([NotNullWhen(true)] out Problems? problems) =>
    Rules.Set<ProdutoCreate>()
        .NotEmpty(Sku)
        .Length(Sku, 3, 30)
        .NotEmpty(Nome)
        .MaxLength(Nome, 120)
        .GreaterThan(Preco, 0m)
        .Min(Estoque, 0)
        .Email(ContatoEmail)
        .HasProblems(out problems);
```

### Validação condicional

```csharp
return Rules.Set<PedidoCreate>()
    .NotEmpty(ClienteId)
    .When(EhEntrega, rules => rules
        .NotNull(Endereco)
        .NotEmpty(Endereco!.Cep))
    .Unless(EhRascunho, rules => rules
        .NotEmpty(Itens))
    .HasProblems(out problems);
```

### Validação aninhada e coleções

Quando um DTO filho também implementa `IValidable`, prefira `Nested`/`NotNullNested` para preservar prefixo de propriedade:

```csharp
return Rules.Set<PedidoCreate>()
    .NotNullNested(Cliente)
    .Nested(Endereco)
    .Nested(Itens)
    .HasProblems(out problems);
```

Se o nome exposto no contrato difere do nome da propriedade, use `WithPropertyPrefix` ou ajuste com `ChainProperty`.

### Validation bridge

Com `OutraLib.Problems.Validation`:

- `validator.Validate(model).ToResult()`
- `validator.HasProblems(model, out problems)`
- `ValidationResult.HasProblems(out problems)`
- `ValidationFailure` severidade `Warning`/`Info` é ignorada na conversão para `Problems`.
- O `ErrorCode` do Validation é adicionado como extensão `error_code`.

Use quando o projeto já adota Validation; caso contrário, `Rules` + `IValidable` costuma ser mais direto.

#### Extensões customizadas do Validation

Use `WithExtension` para anexar dados que serão copiados para `Problem.Extensions`:

```csharp
RuleFor(x => x.Cnpj)
    .NotEmpty()
    .WithErrorCode("cnpj-obrigatorio")
    .WithExtension(ext => ext
        .Add("hint", "Informe somente números")
        .Add("source", "FornecedorCreate"));
```

É possível alterar a categoria padrão gerada pela ponte:

```csharp
ValidationsExtensions.ProblemCategory = ProblemCategory.ValidationFailed;
```

Regra para IA: não altere `ProblemCategory` globalmente em biblioteca compartilhada sem orientação do projeto consumidor.

### IValidable

```csharp
public class ProdutoCreate : IValidable
{
    public required string Nome { get; set; }

    public bool HasProblems([NotNullWhen(true)] out Problems? problems) =>
        Rules.Set<ProdutoCreate>()
            .NotEmpty(Nome)
            .HasProblems(out problems);
}
```

---

## 10) Minimal API + ProblemDetails (guia completo para IA)

### Objetivo

Transformar `Result`/`Result<T>` em respostas HTTP consistentes sem boilerplate, mantendo erros em `application/problem+json`.

### Quando usar cada abordagem

1. **`OkMatch<T>`**
   - Use em `GET`/operações que retornam payload no sucesso (`200 OK`).
2. **`CreatedMatch<T>` / `CreatedMatch`**
   - Use em `POST` de criação (`201 Created`), com rota de localização.
3. **`NoContentMatch`**
   - Use em `PUT/PATCH/DELETE` sem payload (`204 No Content`).
4. **`ApiResults.Result.ToResult(...)`**
   - Use quando quiser `TypedResults` explícitos (`Results<..., MatchErrorResult>`).

### Padrões recomendados de handlers

#### Create (POST)

```csharp
private static async Task<CreatedMatch<ProdutoDetails>> CriarProduto(
    ProdutoCreate dto, ProdutoService service, CancellationToken ct)
{
    var result = await service.CreateAsync(dto, ct);
    return result.Map(p => p.ToDetails())
        .CreatedMatch(d => $"/api/produtos/{d.Id}");
}
```

#### Get (GET)

```csharp
private static async Task<OkMatch<ProdutoDetails>> ObterProduto(
    int id, ProdutoService service, CancellationToken ct)
{
    var entry = await service.GetAsync(id, ct);
    return entry.Map(p => p.ToDetails());
}
```

#### Update/Delete (PUT/PATCH/DELETE)

```csharp
private static async Task<NoContentMatch> AtualizarProduto(
    int id, ProdutoUpdate dto, ProdutoService service, CancellationToken ct) =>
    await service.UpdateAsync(id, dto, ct);
```

### Configuração mínima de ProblemDetails

```csharp
builder.Services.AddProblemDetailsDescriptions(options =>
{
    options.Descriptor.ConfigureDefaults();
    options.DescriptionFiles = ["problem-details.json"];
});
```

Configuração com descritores em código:

```csharp
builder.Services.AddProblemDetailsDescriptions(options =>
{
    options.Descriptor.ConfigureDefaults();
    options.Descriptor.DescribeCategory(ProblemCategory.ValidationFailed, description =>
    {
        description.Title = "Falha de Validação";
        description.Status = HttpStatusCode.UnprocessableEntity;
    });

    options.Descriptor.Add(new ProblemDetailsDescription(
        "produto-estoque-insuficiente",
        "Estoque insuficiente",
        "Não há estoque suficiente para concluir a operação.",
        HttpStatusCode.Conflict));

    options.DescriptionFiles = ["problem-details.json"];
});
```

### MVC Controllers

Em controllers MVC, use `ToActionResult`:

```csharp
[HttpGet("{id:int}")]
public async Task<ActionResult<ProdutoDetails>> Get(int id, CancellationToken ct)
{
    var result = await service.GetAsync(id, ct)
        .MapAsync(p => p.ToDetails());

    return result.ToActionResult();
}
```

Para criação:

```csharp
return result.ToActionResult($"/api/produtos/{id}");
```

### Página de descrição dos Problems

Para expor documentação HTML dos ProblemDetails configurados:

```csharp
app.MapProblemDetailsDescriptionPage(); // rota padrão: /.problems
app.MapProblemDetailsDescriptionPage("/problems");
```

### Filtro de exceções em Minimal API

Use `WithExceptionFilter` para transformar exceções inesperadas em ProblemDetails padronizado na borda HTTP:

```csharp
app.MapPost("/processar", Processar)
    .WithExceptionFilter(LogLevel.Error);
```

Regra para IA: filtro de exceção é para falhas inesperadas. Para validação/domínio esperado, continue retornando `Result`/`Problems`.

### Regras práticas para IA (Minimal API)

1. No handler, **não** monte `ProblemDetails` manualmente; retorne `Result`/`Entry` convertidos via `*Match`.
2. Faça mapeamento para DTO com `.Map(...)` imediatamente antes do `*Match`.
3. Em erros de domínio, prefira `Problems.Custom(..., typeId: ...)` para rastreabilidade de tipo.
4. Mantenha assinatura de handler enxuta: `(dto, service, CancellationToken)`.
5. Use MVC `ToActionResult` apenas em controllers; em Minimal API prefira `OkMatch`/`CreatedMatch`/`NoContentMatch`.

### Anti-patterns

- Misturar `TypedResults` manuais de erro em um endpoint e `*Match` em outro sem motivo.
- Retornar `IResult` genérico quando o tipo forte de retorno (`OkMatch<T>`, `CreatedMatch<T>`) já resolve.
- Fazer `try/catch` para validação de entrada esperada.

---

## 11) API HTTP cliente (`HttpResponseMessage -> Result`)

### `ToResultAsync` e `ToResultAsync<T>`

```csharp
var response = await http.SendAsync(request, ct);
var result = await response.ToResultAsync(ct: ct);
```

Para conteúdo de sucesso:

```csharp
var response = await http.GetAsync(url, ct);
var result = await response.ToResultAsync<MyDto>(ct: ct);
```

### Erro não-ProblemDetails com `FailureTypeReader`

Se a API externa não retorna `application/problem+json`, use reader customizado.

### JsonFailureTypeReader (atual)

A versão atual é genérica:

```csharp
private sealed class Reader : JsonFailureTypeReader<MyError>
{
    protected override Problems Map(MyError response)
        => Problems.InvalidParameter(response.Message ?? "error");
}
```

### Overloads úteis de `ToResultAsync<T>`

Para sucesso com JSON, é possível informar opções ou source generation metadata:

```csharp
var result = await response.ToResultAsync<ProdutoDetails>(jsonOptions, ct);
var result2 = await response.ToResultAsync(responseJsonContext.ProdutoDetails, ct);
```

Para erro customizado não-ProblemDetails:

```csharp
var result = await response.ToResultAsync<ProdutoDetails>(new Reader(), jsonOptions, ct);
```

Regra para IA: se a API externa retorna `application/problem+json`, não crie reader customizado; deixe `ToResultAsync` ler ProblemDetails.

---

## 12) Padrões de composição do CRUD (referência para IA)

Nos exemplos CRUD há mistura intencional de estilos (útil para IA escolher conforme contexto):

- Fluxo explícito (`if (HasProblems)` / `if (entry.NotFound)`)
- Fluxo e com `ContinueAsync`
- Fluxo e com `MapAsync`
- Encadeamento com EF helpers (`AddTo`, `SaveChangesAsync`)

Recomendação: manter consistência dentro do método; misturar estilos no projeto é ok quando melhora leitura.

---

## 13) Do / Don’t para IA

### Do

- Retorne `Result`/`Result<T>` em serviços.
- Use `Problems.NotFound/InvalidParameter/...` para falhas de negócio/entrada.
- Use `Entry` para busca de entidade e short-circuit de not-found.
- Prefira sobrecargas com `TParam` para evitar closures em pipelines.
- Em APIs, use `*Match` para respostas HTTP padronizadas.
- Em validação, adote `Rules.Set<T>()` + `HasProblems(out problems)`.

### Don’t

- Não usar exceção para fluxo normal de validação/not found.
- Não retornar `null` para representar falha.
- Não ocultar erros (`catch` vazio).
- Não misturar múltiplas estratégias no mesmo método sem ganho claro.

---

## 14) Checklist rápido para geração de código por IA

1. O método deve retornar `Result`/`Result<T>`?
2. Há validação de entrada com `Rules` / `HasProblems`?
3. Se busca entidade: usar `TryFind*` + `Entry`?
4. Melhor fluxo aqui: explícito ou  (`Continue/Map`)?
5. Vale usar sobrecarga `TParam` para contexto?
6. Persistência deve encadear `AddTo/SaveChangesAsync`?
7. Endpoint deve usar `OkMatch/CreatedMatch/NoContentMatch`?
8. Problemas têm `property` e `typeId` quando necessário?

---

## 15) APIs avançadas do núcleo (usar com critério)

Estas APIs existem e são úteis, mas não devem substituir o fluxo normal `Result`/`Problems`.

### Exceções inesperadas

`Problems.InternalError(exception)` converte exceção inesperada em problema. Pode ser customizado por:

- `Problems.ExceptionHandler`
- `Problems.ExceptionOptions`
- `IExceptionHandler`

```csharp
try
{
    await integracao.EnviarAsync(cmd, ct);
    return Result.Ok();
}
catch (Exception ex)
{
    return Problems.InternalError(ex);
}
```

Regra para IA: use isso em bordas de infraestrutura/integração. Não envolva validação esperada em `try/catch`.

### `EnsureSuccess` / `EnsureHasValue`

Esses métodos lançam exceção quando o `Result` falha:

```csharp
result.EnsureSuccess();
valuedResult.EnsureHasValue(out var value);
```

Use principalmente em testes, jobs de bootstrap ou integração com APIs que exigem exceção. Em serviço de aplicação, prefira retornar `Problems`.

### `CheckIfNull` / `CheckIfNullAsync`

Útil para adaptar APIs legadas que retornam `null`:

```csharp
return await repo.FindAsync(id, ct)
    .CheckIfNullAsync(id, static id => Problems.NotFound("Produto não encontrado", nameof(id)));
```

### `AsResult` e `AsInvalidParameter`

Use para resolver ambiguidade de tipo ou adaptar categoria/propriedade:

```csharp
if (dto.HasProblems(out var problems))
    return problems.AsResult<Produto>();

return domainProblem.AsInvalidParameter(nameof(dto.Status));
```

### `Problems.ToException`

Use apenas em bordas onde a API consumidora exige exception:

```csharp
if (result.HasProblems(out var problems))
    throw problems.ToException();
```

Regra para IA: converter `Problems` para exception deve ser exceção arquitetural, não padrão de aplicação.

---

## 16) Referências internas úteis

- `src/.github/instructions-docs.md`
- `src/doc-para-devs-new.md`
- `README.md`
- `src/OutraLib.Problems.Tests.Api/ExemplosDoc/CRUD/`
- `src/OutraLib.Problems/Entities/Entry.cs`
- `src/OutraLib.Problems/AsyncExtensions.Entry'1.cs`
- `src/OutraLib.Problems/AsyncExtensions.Entry'2.cs`
- `src/OutraLib.Problems/RuleSet.cs`
- `src/OutraLib.Problems/Extensions.cs`
- `src/OutraLib.Problems.EntityFramework/ProblemsEFExtensions.TryFind.cs`
- `src/OutraLib.Problems.EntityFramework/ProblemsEFExtensions.DbOperations.cs`
- `src/OutraLib.Problems.ProblemDetails/ApiResults/ApiResults.cs`
- `src/OutraLib.Problems.ProblemDetails/MvcResults/MvcResultsExtensions.cs`
- `src/OutraLib.Problems.Validation/ValidationsExtensions.cs`

---

## 17) Nota final

Se a IA estiver gerando código em outro projeto, priorizar:

- clareza + fluência do pipeline,
- tipagem forte com `Result`/`Entry`,
- padronização de erro com `Problems`,
- validação consistente com `Rules`/`HasProblems`,
- e consistência com convenções locais do projeto consumidor.

---

## 18) Receitas por cenário (pronto para IA aplicar)

### Receita A — CRUD simples com EF + Result

Quando usar:
- operações CRUD tradicionais, baixa complexidade de regra.

Passos:
1. Validar DTO com `HasProblems`.
2. Criar entidade (`Result<T>` de factory).
3. Persistir com `AddTo` + `SaveChangesAsync`.
4. Retornar entidade/DTO via `Result<T>`.

```csharp
public Task<Result<Categoria>> CreateAsync(CategoriaCreate dto, CancellationToken ct)
{
    if (dto.HasProblems(out var problems))
        return problems.AsResult<Categoria>();

    return Categoria.Criar(dto.Nome, dto.Descricao)
        .AddTo(db)
        .SaveChangesAsync(db, ct);
}
```

### Receita B — Operação com busca + not-found sem payload

Quando usar:
- update/delete/ação sobre registro existente.

Passos:
1. Buscar com `TryFindAsync`.
2. Continuar com `Continue/ContinueAsync`.
3. Salvar alterações.

```csharp
public async Task<Result> AtualizarAsync(int id, ProdutoUpdate dto, CancellationToken ct)
{
    if (dto.HasProblems(out var p))
        return p;

    return await db.TryFindAsync<Produto, int>(id, ct)
        .ContinueAsync(async produto =>
        {
            if (produto.Atualizar(dto.Nome, dto.Preco).HasProblems(out var updateP))
                return updateP;

            await db.SaveChangesAsync(ct);
            return Result.Ok();
        });
}
```

Se o método precisa retornar a entidade atualizada, use `Task<Result<Produto>>` e confirme que o pipeline preserva o valor (`Entry.ContinueAsync` pode retornar `Result<TEntity>` dependendo da sobrecarga).

### Receita C — Pipeline sem closure com `TParam`

Quando usar:
- há contexto extra (dto/config/usuário/token) e você quer evitar capturas.

```csharp
return await db.Produtos.TryFindAsync<Produto, int>(id, ct)
    .CollectAsync(dto.Delta, static (produto, delta, _) =>
    {
        produto.AjustarEstoque(delta);
        return Task.CompletedTask;
    }, ct)
    .SaveChangesAsync(db, ct);
```

### Receita D — Endpoint Minimal API padrão

Quando usar:
- handlers de API com ProblemDetails padronizado.

```csharp
app.MapPost("/produtos", async (
    ProdutoCreate dto,
    ProdutoService service,
    CancellationToken ct) =>
{
    var result = await service.CreateAsync(dto, ct);
    return result.Map(p => p.ToDetails())
        .CreatedMatch(d => $"/produtos/{d.Id}");
});
```

### Receita E — Cliente HTTP resiliente com `ToResultAsync`

Quando usar:
- integração com API externa e necessidade de manter semântica `Result`.

```csharp
var response = await http.GetAsync(url, ct);
var result = await response.ToResultAsync<ProdutoDetails>(ct: ct);

if (result.HasProblems(out var problems))
    return problems;

result.EnsureHasValue(out var dto);
return dto;
```

---

## 19) Operações implícitas (conversões e operadores) — guia para IA

> Esta seção é essencial para IA gerar código mais enxuto e idiomático no ecossistema Problems.

### 19.1 Conversões implícitas mais úteis

#### `T -> Result<T>`

```csharp
Result<ProdutoDto> r = new ProdutoDto(id, nome);
```

#### `Problem / Problems -> Result` e `Result<T>`

```csharp
Result r1 = Problems.InvalidParameter("Nome obrigatório", "Nome");
Result<ProdutoDto> r2 = Problems.NotFound("Produto não encontrado");
```

#### `Entry -> Result<T>`

```csharp
Entry<Produto> entry = await db.TryFindByAsync<Produto>(p => p.Id == id, ct);
Result<Produto> r = entry; // implícito
```

#### `Problem / Problems / Result -> Task<Result>` e `Task<Result<T>>`

Permite `return` direto em métodos assíncronos sem `Task.FromResult` manual.

```csharp
public Task<Result> DeleteAsync(int id)
{
    if (id <= 0)
        return Problems.InvalidParameter("Id inválido", nameof(id));

    return Result.Ok();
}
```

#### `Exception -> Result` e `Result<T>`

```csharp
try
{
    await op.ExecuteAsync(ct);
    return Result.Ok();
}
catch (Exception ex)
{
    Result result = ex; // usa Problems.InternalError(ex)
    return result;
}
```

Prefira `Problems.InternalError(ex)` se quiser deixar a intenção mais explícita.

#### `Result<T> -> Result`

```csharp
Result<Produto> produtoResult = await service.CreateAsync(dto, ct);
Result onlyStatus = produtoResult; // descarta valor quando sucesso
```

Use quando a operação chamadora não precisa do valor.

#### `Problem / Problems -> OkMatch<T>`, `CreatedMatch<T>` e `NoContentMatch`

Os tipos `*Match` aceitam implicitamente problemas, facilitando early return em handlers:

```csharp
private static OkMatch<ProdutoDetails> Obter(int id)
{
    if (id <= 0)
        return Problems.InvalidParameter("Id inválido", nameof(id));

    return new ProdutoDetails(id, "Produto");
}
```

### 19.2 Operadores importantes

#### `+` em `Problems`

```csharp
Problems all = Problems.InvalidParameter("A", "CampoA");
all += Problems.ValidationFailed("B", "CampoB");
```

#### `+` em `Result`

Permite acumular falhas:

```csharp
var baixa = Result.Ok();
baixa += produtoEntry.Continue(p => p.AjustarEstoque(-qtd));
```

### 19.3 Quando usar implícito vs explícito

Use implícito quando:
- melhora legibilidade e reduz boilerplate.
- tipo de destino está claro no contexto.

Prefira explícito quando:
- há ambiguidade de overload.
- o trecho fica menos óbvio para leitura humana.

### 19.4 Armadilhas comuns para IA

1. Misturar `Result` e `Result<T>` em chain sem observar tipo de retorno final.
2. Assumir assinatura antiga sem `CancellationToken` em sobrecargas com `TParam`.
3. Usar implícito em excesso em trechos críticos e perder clareza do fluxo.

### 19.5 Mini-cheatsheet de escrita idiomática

```csharp
// falha curta
return Problems.NotFound("Registro não encontrado", nameof(id));

// sucesso curto
return entity;

// short-circuit de validação
if (dto.HasProblems(out var p)) return p;

// entity lookup + atualização simplificada
return await db.TryFindAsync<Produto, int>(id, ct)
    .ContinueAsync(dto, static (produto, request) => produto.Atualizar(request.Nome, request.Preco))
    .SaveChangesAsync(db, ct);
```
