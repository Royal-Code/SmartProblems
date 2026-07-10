# Design — Busca composta com `FindResult` (`FindByCriteria`)

> Complementa a [ideia.md](ideia.md). Define os problemas atuais, as melhorias e o design da nova API fluente.
>
> **Revisões incorporadas**:
> 1ª — nome `FindByCriteria`, guarda contra `default(struct)`, imutabilidade real da cadeia,
> ordem de materialização explícita, política de avaliação sem `Compile()`, degradação para
> `OrElse` e comportamento definido para `propertyName` duplicado.
> 2ª — exemplos com `Id<,>.Value`, regra de rootedness (parser do legado e `By` do builder),
> testes diretos da factory no core e validação de `propertyName` no `FindCriterion` + factory defensiva.
> 3ª — `byName` whitespace tratado como "não especificado" na resolução via `DisplayNames`.

## 1. Problemas de hoje

### 1.1 Não há suporte a busca composta

`TryFindAsync` busca por Id e `TryFindByAsync` por um único campo ou um único predicado.
Não existe forma de combinar N campos e obter a mensagem rica padronizada para todos eles.

### 1.2 Bug latente: predicado composto quebra na geração do problema

Em `SmartProblemsEFExtensions.TryFind.cs`, o `GenerateProblem(Expression<Func<TEntity,bool>>)`
exige que `filter.Body` seja um `BinaryExpression` cujo lado esquerdo é um `MemberExpression`.

Com um predicado composto (`c => c.StateId == sid && c.Name == name`):

- **Encontrado**: funciona (a query executa normalmente).
- **Não encontrado**: `AndAlso` é um `BinaryExpression` cujo `Left` é outro `BinaryExpression`
  → lança `ArgumentException` exatamente no caminho que deveria produzir a mensagem rica.

### 1.3 Parsing de expressão frágil no `GenerateProblem`

Formas suportadas hoje (lado do valor): constante literal, variável local (field de closure),
propriedade de variável capturada (`request.Name`).

Formas que **lançam exceção** hoje:

- `&&` / `||` (item 1.2);
- ordem invertida (`5 == c.Id`);
- conversões implícitas (`Convert` para enums/tipos diferentes);
- chamadas de método (`c.Name == nome.Trim()`);
- membro booleano puro (`c => c.Active`) e negação;
- cadeias profundas (`req.Filtro.Nome`), membros static.

**Princípio de design a adotar: geração de mensagem de erro nunca deve lançar.**
Formas não reconhecidas devem degradar para a mensagem genérica `R.EntityNotFound`.

### 1.4 `Expression.Constant` impede a parametrização do EF

`TryFindByAsync(propertySelector, filterValue)` monta `Expression.Equal(body, Expression.Constant(filterValue))`
(linhas ~160 e ~194). O EF Core só parametriza acessos a membros de closures (funcletização);
`ConstantExpression` manual vira **literal no SQL**. Consequências:

1. O query cache do EF compara constantes por valor → cada valor distinto = nova entrada + recompilação da query;
2. Poluição do plan cache do servidor (um SQL ad hoc por valor).

**Correção**: capturar o valor num closure real (ver §4.4) — vira parâmetro (`@__value_0`).

### 1.5 Mensagem/factory de problema só para um campo

`R.EntityNotFoundBy` = `"The record of '{0}' with {1} '{2}' was not found"` e
`FindResult<TEntity>.Problem(byName, propertyName, value)` suportam apenas um par campo/valor.

## 2. Melhorias no código existente (independentes da nova API)

1. **`GenerateProblem` robusto e seguro**:
   - recursão **apenas em `AndAlso`**, coletando N critérios → nova factory multi-campo (§4.1).
     Qualquer `OrElse`, em qualquer nível, degrada o predicado inteiro para a mensagem genérica
     `EntityNotFound`: o formato multi-campo ("with A 'x', B 'y'") comunica conjunção e seria
     factualmente errado para OR. Quem precisa de OR com mensagem rica usa o overload explícito
     do builder (§4.2);
   - **classificação dos lados de cada igualdade**: o lado "propriedade da entidade" deve ser um
     `MemberExpression` **direto sobre o parâmetro da lambda** (após unwrap de `Convert`); o outro
     lado é o valor. A regra cobre a ordem invertida (`valor == e.Prop`) e elimina ambiguidades:
     `e.A == e.B` (ambos enraizados no parâmetro) e comparações entre membros capturados (nenhum
     enraizado) não geram critério; cadeias profundas (`e.State.Name`) também não — o `Member.Name`
     seria `"Name"` e o `DisplayNames` consultaria o tipo errado (`TEntity` em vez de `State`);
   - **all-or-nothing na cadeia `AndAlso`**: se qualquer folha não render um critério extraível,
     o predicado **inteiro** degrada para a mensagem genérica — mensagem parcial seria factualmente
     errada ("with A 'x' was not found" quando existe registro com A = x que falhou noutra condição);
   - lado do valor: avaliar **somente cadeias puras de membros** (field de closure → propriedades),
     por caminhada iterativa envolta em `try/catch` (getters podem lançar). **Não usar
     `Expression.Lambda(...).Compile()`**: re-executar chamadas de método duplicaria efeitos
     colaterais e poderia exibir valor diferente do consultado — o EF já avaliou essas
     subexpressões uma vez na funcletização, quando a query executou. `MethodCallExpression`
     ou qualquer nó não reconhecido no lado do valor → mensagem genérica;
   - **nunca lançar** — qualquer falha ou forma não suportada degrada para `EntityNotFound`;
   - documentar no XML doc as formas suportadas, sem prometer expressões arbitrárias
     (o builder é o caminho sancionado para mensagens ricas: recebe os valores em mãos,
     sem avaliação de expressão alguma).
2. **Parametrizar** o `TryFindByAsync(propertySelector, value)` existente com closure capture (§4.4).
   Sem mudança de comportamento; o SQL passa de literal a parâmetro.
3. (Opcional) Overloads one-shot para 2 campos (`TryFindByAsync(s1, v1, s2, v2, ct)`) — açúcar; o builder cobre N.

## 3. Nova API — visão de uso

```csharp
// CountryService: cidade por estado + nome
public async Task<Result<City>> GetCityAsync(Id<State, int> stateId, string cityName, CancellationToken ct = default)
{
    var find = await db.FindByCriteria<City>()
        .By(c => c.StateId, stateId.Value)
        .By(c => c.Name, cityName)
        .TryFindAsync(ct);

    return find.ToResult();
}

// Com Include, AsNoTracking e filtro condicional
var criteria = db.Set<City>()
    .Include(c => c.State)
    .FindByCriteria()
    .By(c => c.StateId, stateId.Value);

if (name is not null)
    criteria = criteria.By(c => c.Name, name);

var find = await criteria.TryFindAsync(ct);
```

Problema gerado quando não encontrado (dois critérios, **na ordem dos `By`**):

```json
{
  "status": 404,
  "detail": "The record of 'City' with StateId '5', Name 'Blumenau' was not found",
  "extensions": { "entity": "City", "StateId": 5, "Name": "Blumenau" }
}
```

Os nomes de exibição passam pelo `DisplayNames` (respeitando `[DisplayName]`), como hoje.

## 4. Tipos novos

### 4.1 Core (`RoyalCode.SmartProblems`) — sem dependência de EF

**`FindCriterion`** (novo arquivo `Entities/FindCriterion.cs`):

```csharp
namespace RoyalCode.SmartProblems.Entities;

/// <summary>
/// A criterion used to search for an entity, used to generate problem messages.
/// </summary>
public readonly struct FindCriterion
{
    // Valida com ArgumentException.ThrowIfNullOrWhiteSpace: propertyName vira chave de
    // Extensions do problema — null/vazio estouraria no caminho not-found, que nunca lança (§1.3).
    public FindCriterion(string propertyName, object? value, string? byName = null);

    /// <summary>Name of the property, used as extension data key of the problem.</summary>
    public string PropertyName { get; }

    /// <summary>Value used in the filter.</summary>
    public object? Value { get; }

    /// <summary>
    /// Display name; when null or whitespace, resolved via <see cref="DisplayNames"/> at problem
    /// generation (whitespace is treated as "unspecified" to avoid a blank name in the detail message).
    /// </summary>
    public string? ByName { get; }
}
```

**Nova factory em `FindResult<TEntity>`** (mesmo arquivo `Entities/FindResult.cs`):

```csharp
/// <summary>
/// Creates a not-found problem from multiple search criteria.
/// </summary>
public static FindResult<TEntity> Problem(ReadOnlySpan<FindCriterion> criteria);
```

Comportamento:

- `0` critérios → problema genérico `R.EntityNotFound` (defensivo);
- `1` critério → comportamento atual (`R.EntityNotFoundBy`);
- `N` critérios → novo recurso `R.EntityNotFoundByMany` com pares `"{byName} '{value}'"` unidos por `", "`,
  mais `.With("entity", ...)` e um `.With(propertyName, value)` por critério;
- **`ByName` ausente**: `null` **ou whitespace** (`string.IsNullOrWhiteSpace`) contam como "não
  especificado" → resolve via `DisplayNames.Instance.GetDisplayName(typeof(TEntity), propertyName)`
  (lazy: só no caminho not-found, como hoje). Evita `detail` com nome em branco quando o chamador
  passa `byName: " "` por engano;
- **ordem**: o span chega na ordem de declaração (responsabilidade do chamador — o builder garante, §4.2)
  e o `detail` lista todos os pares nessa ordem;
- **`propertyName` duplicado (comportamento definido)**: o `detail` sempre mostra **todos** os pares,
  mesmo com chaves repetidas; nas `extensions`, vale a semântica já pública do `Problem.With`
  (`Extensions[key] = value`, [Problem.cs:90](../../RoyalCode.SmartProblems/Problem.cs)) — **último vence**,
  silenciosamente. Sem sufixos artificiais e sem exceção: repetição legítima (ranges) acontece no
  overload explícito, onde o usuário controla as chaves (`priceMin`/`priceMax`);
- **defensiva contra `default(FindCriterion)`**: struct dispensa construtor (`new FindCriterion[n]`
  chega com `PropertyName` null sem passar pela validação). Critério com `PropertyName` null/vazio
  é **ignorado**; se todos forem inválidos, aplica-se o caso de 0 critérios. O construtor protege
  o call site; a factory protege o caminho not-found, que **nunca lança** (§1.3).

**Recurso novo no `R.resx`**:

```xml
<data name="EntityNotFoundByMany" xml:space="preserve">
  <value>The record of '{0}' with {1} was not found</value>
</data>
```

(`{1}` = pares já formatados e unidos por vírgula; evita localizar a conjunção "and".)

### 4.2 EF (`RoyalCode.SmartProblems.EntityFramework`)

**Entradas** — `partial SmartProblemsEFExtensions`, novo arquivo `SmartProblemsEFExtensions.FindByCriteria.cs`
(namespace `Microsoft.EntityFrameworkCore`, como as demais):

```csharp
public static FindCriteria<TEntity> FindByCriteria<[DAM(All)] TEntity>(this DbContext db)
    where TEntity : class => new(db.Set<TEntity>());

public static FindCriteria<TEntity> FindByCriteria<[DAM(All)] TEntity>(this DbSet<TEntity> set)
    where TEntity : class => new(set);

// permite Include / AsNoTracking / OrderBy pré-compostos
public static FindCriteria<TEntity> FindByCriteria<[DAM(All)] TEntity>(this IQueryable<TEntity> query)
    where TEntity : class => new(query);
```

**Builder** — novo arquivo `FindCriteria.cs`:

```csharp
namespace RoyalCode.SmartProblems.Entities;

/// <summary>
/// Fluent builder for composed entity searches, producing a <see cref="FindResult{TEntity}"/>
/// with a rich, standardized problem when the entity is not found.
/// </summary>
public readonly struct FindCriteria<[DAM(All)] TEntity>
    where TEntity : class
{
    private readonly IQueryable<TEntity> query;   // filtros já aplicados via Where encadeado
    private readonly CriterionNode? criteria;     // cadeia imutável, para a mensagem

    internal FindCriteria(IQueryable<TEntity> query);
    private FindCriteria(IQueryable<TEntity> query, CriterionNode? criteria);

    // ---- passos fluentes: cada um retorna um NOVO FindCriteria ----

    /// <summary>Filters by equality on a property; display name resolved via DisplayNames.</summary>
    public FindCriteria<TEntity> By<TValue>(Expression<Func<TEntity, TValue>> propertySelector, TValue value);

    /// <summary>Same as above, with an explicit display name.</summary>
    public FindCriteria<TEntity> By<TValue>(Expression<Func<TEntity, TValue>> propertySelector, TValue value, string byName);

    /// <summary>Arbitrary predicate with explicit message data (mirrors the existing extension).</summary>
    public FindCriteria<TEntity> By(Expression<Func<TEntity, bool>> filter, string byName, string propertyName, object? value);

    // ---- terminal ----

    /// <summary>Executes with FirstOrDefaultAsync semantics, returning the entity or a rich problem.</summary>
    public async Task<FindResult<TEntity>> TryFindAsync(CancellationToken ct = default);

    // ---- implementação interna ----

    // Guarda contra default(FindCriteria<TEntity>): struct sempre admite default.
    // Chamada no início de TODOS os By e do TryFindAsync.
    private void EnsureInitialized()
    {
        if (query is null)
            throw new InvalidOperationException(
                "FindCriteria was not initialized. Do not use default(FindCriteria<TEntity>); " +
                "start with FindByCriteria(...) on a DbContext, DbSet or IQueryable.");
    }

    // Realmente imutável: campos readonly via primary constructor.
    private sealed class CriterionNode(FindCriterion criterion, CriterionNode? previous)
    {
        public readonly FindCriterion Criterion = criterion;
        public readonly CriterionNode? Previous = previous;
        public readonly int Count = (previous?.Count ?? 0) + 1;
    }

    // A cabeça da cadeia é o ÚLTIMO critério: preenche o array de trás para
    // frente para preservar a ordem de declaração dos By(...).
    private FindCriterion[] MaterializeCriteria()
    {
        var node = criteria;
        if (node is null)
            return [];

        var array = new FindCriterion[node.Count];
        for (var i = node.Count - 1; i >= 0; i--, node = node!.Previous)
            array[i] = node!.Criterion;

        return array;
    }
}
```

Semântica de `By<TValue>(selector, value)`:

1. `EnsureInitialized()` — `InvalidOperationException` clara para `default(FindCriteria<TEntity>)`
   (sem a guarda, o usuário receberia `ArgumentNullException("source")` das entranhas do EF);
2. Valida o selector: unwrap de `Convert` e exige `MemberExpression` **direto sobre o parâmetro
   do selector** (`member.Expression` == `selector.Parameters[0]`). Sem a checagem de rootedness,
   `By(c => outroObjeto.Prop, value)` compila — a lambda pode ignorar o parâmetro — e viraria
   filtro constante sem sentido no SQL. Aqui **pode** lançar `ArgumentException` — é erro de
   programação, não caminho de erro em runtime;
3. Monta `Expression.Equal(selector.Body, Capture(value))` **reutilizando `selector.Parameters`**
   — não precisa de visitor de rebase de parâmetro;
4. `query.Where(lambda)` — `Where` encadeado gera o mesmo SQL que um único `AndAlso`
   (composição sempre **AND** por construção; OR dentro de um critério = overload explícito);
5. Empilha `new CriterionNode(new FindCriterion(member.Name, value, byName?), criteria)`.

Terminal:

```csharp
public async Task<FindResult<TEntity>> TryFindAsync(CancellationToken ct = default)
{
    EnsureInitialized();

    var entity = await query.FirstOrDefaultAsync(ct);
    if (entity is not null)
        return entity;

    return FindResult<TEntity>.Problem(MaterializeCriteria());
}
```

Nota: a mensagem da guarda é literal no pacote EF — `R` é `internal` do core; o pacote EF já usa
literais em inglês nas `ArgumentException` existentes.

### 4.3 Decisões de design (e porquês)

| Decisão | Porquê |
|---|---|
| Entrada `FindByCriteria` (não `FindByCompose`) | Gramática natural em inglês; coerência de vocabulário com `FindCriterion`; o prefixo `Find` agrupa com `TryFind*` no IntelliSense, onde a API será descoberta. |
| Struct `FindCriteria<TEntity>` | Nomeia o que o builder é: o conjunto de critérios da busca. Usuários raramente escrevem o nome (var + fluent); a proximidade com `FindCriterion` é semântica real (plural/singular), não acidente. |
| `readonly struct`, **não** `readonly ref struct` | `ref struct` gera CS4007 em `await` no C# ≤ 12 (default do net8.0) e não pode cruzar `await` nem no C# 13+; os campos são referências de heap (nenhum ganho real); método `async` não pode ser membro de ref struct. O `readonly struct` já evita a alocação do builder. |
| Guarda `EnsureInitialized` em `By` e `TryFindAsync` | Struct sempre admite `default`; erro claro (`InvalidOperationException`) em vez de `ArgumentNullException("source")` do EF. Mesmo princípio do `DefaultResultNotInitialized` já existente no core para `default(Result<T>)`. |
| Cada `By` retorna **nova instância** (imutável) | Fluent natural; permite composição condicional (`criteria = criteria.By(...)` dentro de `if`). |
| Critérios em **cadeia imutável** (nós com campos `readonly`), não `List<>` compartilhada | Fork-safety: guardar um builder parcial e derivar duas buscas não vaza critérios entre si — e a garantia depende de imutabilidade **real**, imposta pelo compilador, não de disciplina. Custo igual (1 alocação pequena por `By`). |
| Materialização de trás para frente | A cabeça da cadeia é o último `By`; preencher o array `Count-1 → 0` preserva a ordem de declaração no `detail` e nas `extensions`. |
| Filtros via `Where` encadeado, não `AndAlso` manual | Elimina o visitor de rebase de parâmetros; SQL idêntico; garante semântica AND por construção. |
| `propertyName` duplicado: último vence nas `extensions` (documentado) | Semântica já pública do `Problem.With` (`Extensions[key] = value`); o `detail` sempre mostra todos os pares na ordem; sem exceções nem sufixos artificiais. |
| Lado-propriedade = membro **direto do parâmetro** (parser do legado e `By` do builder) | Classificação sem ambiguidade (`e.A == e.B`, membros capturados dos dois lados); cadeias profundas dariam display name do tipo errado; no `By`, impede filtro constante (`c => outroObjeto.Prop`). |
| `FindCriterion` valida `propertyName` no ctor; factory ignora critérios inválidos | `propertyName` é chave de `Extensions`: null estouraria `ArgumentNullException` no caminho not-found, que nunca lança (§1.3). O ctor protege o call site; a factory cobre `default(FindCriterion)`, que dispensa construtor. |
| Terminal `TryFindAsync` (não `FirstOrDefaultAsync`) | Consistência com a família `TryFind*` → `FindResult`. `FirstOrDefaultAsync` no EF retorna `TEntity?`; usar o mesmo nome com outro contrato confundiria. |
| Terminal retorna `Task<FindResult<TEntity>>` (não `ValueTask`) | Encaixa direto nas extensions existentes de `AsyncResultExtensions.cs` sobre `Task<FindResult<TEntity>>`. |
| Não consulta o change tracker local (sempre roundtrip) | Mesmo comportamento do `TryFindByAsync` atual; documentar no XML doc. |

### 4.4 Parametrização trim-safe (closure capture)

Em vez de `Expression.Constant(value)` (§1.4) e sem reflection manual:

```csharp
private static Expression Capture<TValue>(TValue value)
{
    // o compilador gera a closure (DisplayClass.value) — exatamente a forma
    // que a funcletização do EF converte em parâmetro de query.
    Expression<Func<TValue>> capture = () => value;
    return capture.Body; // MemberExpression: Field(Constant(closure), "value")
}
```

Trim-safe (nenhum `GetField`/`GetProperty` manual), funciona em todos os TFMs
(net8/net9/net10), e o EF gera `@__value_0` com query cache estável.
A mesma helper corrige as extensions existentes (§2.2).

## 5. Integração com o pipeline existente

O retorno `Task<FindResult<TEntity>>` compõe com as extensions já existentes:

```csharp
return await db.FindByCriteria<City>()
    .By(c => c.StateId, stateId.Value)
    .By(c => c.Name, name)
    .TryFindAsync(ct)
    .MapAsync(city => new CityDto(city.Id, city.Name));
```

## 6. Trimming / AOT

- `[DynamicallyAccessedMembers(All)]` em `TEntity` nos entry points e no struct (como nas extensions atuais);
- `DisplayNames` já é anotado;
- `Capture` (§4.4) não usa reflection manual → nada novo a anotar;
- Manter `IsTrimmable` verde nos três TFMs.

## 7. Plano de testes

### 7.1 Core — testes diretos da factory (sem EF)

Em `RoyalCode.SmartProblems.Tests/Basics/`, exercitando `FindCriterion` e
`FindResult<TEntity>.Problem(ReadOnlySpan<FindCriterion>)` diretamente:

1. Zero critérios → mensagem genérica `EntityNotFound`;
2. Um critério → formato atual (`EntityNotFoundBy`);
3. Múltiplos critérios → `EntityNotFoundByMany`, pares na ordem do span;
4. `ByName` null → nome resolvido via `DisplayNames`;
5. `[DisplayName]` na propriedade → usado no `detail`;
6. Chave duplicada → `detail` mostra todos os pares; `extensions` com último vencendo;
7. Construtor de `FindCriterion` rejeita `propertyName` null/vazio/whitespace (`ArgumentException`);
8. Factory defensiva: span com `default(FindCriterion)` → critério ignorado, sem lançar;
   todos inválidos → caso de 0 critérios.

### 7.2 EF — testes do builder e das melhorias no legado

No mesmo projeto (já referencia o pacote EF), usando `Country`/`State`/`City`:

1. Encontrado com 2+ critérios → retorna a entidade;
2. Não encontrado → `detail` contém todos os pares **na ordem dos `By`** e `extensions` contém
   `entity` + cada `propertyName`/valor;
3. Overload com `byName` explícito;
4. Overload de predicado arbitrário (ex.: `c => c.Name.StartsWith(prefix)`);
5. Composição condicional (`By` dentro de `if`);
6. Fork do builder não vaza critérios entre ramificações;
7. `default(FindCriteria<TEntity>)` → `InvalidOperationException` clara, tanto em `By` quanto em `TryFindAsync`;
8. `By` com selector não enraizado no parâmetro (`c => outroObjeto.Prop`) → `ArgumentException`;
9. Dois `By` na mesma propriedade → `detail` mostra os dois pares; `extensions` com último vencendo
   (end-to-end do §7.1, caso 6);
10. `ToQueryString()` contém parâmetros (`@__`) e **não** literais — valida §4.4;
11. Regressão (§2.1): `TryFindByAsync` com predicado composto (`AndAlso`) não lança mais e gera mensagem multi-campo;
12. `OrElse` no predicado → degrada para mensagem genérica `EntityNotFound` (não lança);
13. `e.A == e.B` e cadeia profunda (`e.State.Name == x`) → sem critério extraível → mensagem genérica (all-or-nothing);
14. Chamada de método no lado do valor → mensagem genérica **sem re-executar** o método
    (contador de invocações permanece 1 após o caminho not-found).

## 8. Fora de escopo / futuro

- Semântica `Single` (detectar duplicidade → `InvalidState`);
- Overloads one-shot de 2 campos sem builder;
- `By` aceitando `Id<TRelated, TId>` para FKs (mensagem com o display name da entidade relacionada);
- Aplicar o closure capture nas demais extensions (recomendado, pode ser PR separado).
