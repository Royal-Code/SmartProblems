# Plano de implementacao - Descriptions UI para SmartProblems

## Objetivo
Implementar, no projeto `RoyalCode.SmartProblems.ProblemDetails`, uma pagina HTML de documentacao dos tipos de problem details RFC 9457, equivalente em funcao ao prototipo em `.ai/plans/descriptions-ui/*.cs`, mas com linguagem visual diferente e suporte de localizacao por recursos.

## Decisoes validadas
1. Internacionalizacao com arquivo de recursos para dois idiomas: `R.resx` (idioma padrao) e `R.pt-BR.resx`.
2. Mapeamento manual da rota pelo desenvolvedor via `MapProblemDetailsDescriptionPage`.
3. A sobrecarga sem parametro mantem rota padrao `/.problems`, como na outra lib.
4. A pagina deve exibir todos os tipos genericos conhecidos (sem filtro por uso em runtime).
5. O prototipo em `.ai/plans/descriptions-ui/*.cs` e referencia funcional, nao referencia visual.
6. A UI deve ter recursos proprios em `Descriptions/Documentation`, sem reutilizar `DR.resx`, que pertence aos textos de descricao/conversao.
7. Anchors/ids HTML devem ser normalizados a partir de `TypeId`, nao usar o valor cru quando houver caracteres problemáticos para atributo/id.

## Escopo
1. Adicionar endpoint para expor a pagina de descricoes em rota configuravel (padrao `/.problems`).
2. Gerar modelo de visualizacao a partir de `ProblemDetailsOptions` e `ProblemDetailsDescriptor`.
3. Renderizar HTML completo com CSS inline, sem dependencia de JS externo.
4. Aplicar visual novo inspirado em:
- chatgpt.com: superficies neutras, layout limpo, contraste suave.
- github.com: metadados densos, badges e tipografia mono para dados tecnicos.
- claude.ai: espaco em branco generoso e tom editorial.
5. Adicionar textos da pagina em recursos localizaveis (`R.resx` e `R.pt-BR.resx`).
6. Cobrir com testes unitarios e de integracao HTTP.

## Fora de Escopo
1. Copiar CSS/markup exato de sites de referencia.
2. Criar SPA, JS client-side ou dependencia de assets externos.
3. Alterar comportamento de conversao de `Problem` para `ProblemDetails` fora do necessario para leitura das descricoes.
4. Inferir uso real de tipos por analise estatica somente.
5. Reaproveitar a linguagem visual implementada no prototipo da outra lib.

## Mapeamento de Arquivos (proposto)
1. Novo: `RoyalCode.SmartProblems.ProblemDetails/Descriptions/Documentation/ProblemDetailsDescriptionPageModel.cs`
2. Novo: `RoyalCode.SmartProblems.ProblemDetails/Descriptions/Documentation/ProblemDetailsDescriptionPageHtmlRenderer.cs`
3. Novo: `RoyalCode.SmartProblems.ProblemDetails/Descriptions/ProblemDetailsDescriptionPageEndpointRouteBuilderExtensions.cs`
4. Ajuste: `RoyalCode.SmartProblems.ProblemDetails/Descriptions/ProblemDetailsDescriptor.cs`
5. Novo: `RoyalCode.SmartProblems.ProblemDetails/Descriptions/Documentation/R.resx`
6. Novo: `RoyalCode.SmartProblems.ProblemDetails/Descriptions/Documentation/R.pt-BR.resx`
7. Novo (gerado): `RoyalCode.SmartProblems.ProblemDetails/Descriptions/Documentation/R.Designer.cs`
8. Ajuste: `RoyalCode.SmartProblems.ProblemDetails/RoyalCode.SmartProblems.ProblemDetails.csproj`
9. Opcional (fase 2): `RoyalCode.SmartProblems.ProblemDetails/Descriptions/Documentation/ProblemDetailsDescriptionUsageTracker.cs`
10. Testes novos:
- `RoyalCode.SmartProblems.Tests/Descriptions/ProblemDetailsDescriptionPageModelTests.cs`
- `RoyalCode.SmartProblems.Tests/Descriptions/ProblemDetailsDescriptionPageHtmlRendererTests.cs`
- `RoyalCode.SmartProblems.Tests/Web/ProblemDetailsDescriptionPageTests.cs`
11. Ajuste para integracao de teste:
- `RoyalCode.SmartProblems.TestsApi/Program.cs`

## Decisoes Tecnicas
1. API publica minima
- Somente a extensao de endpoint sera publica (`MapProblemDetailsDescriptionPage`).
- Model e renderer permanecem `internal`.

2. Namespace
- Extensao de endpoint em `Microsoft.AspNetCore.Builder` para uso fluido no minimal API.
- Model e renderer em `RoyalCode.SmartProblems.Descriptions.Documentation`.

3. Fonte dos dados da pagina
- Reutilizar `IOptions<ProblemDetailsOptions>`.
- Incluir descricoes custom e descricoes genericas conhecidas.
- Adicionar em `ProblemDetailsDescriptor` um metodo de leitura para listar descricoes da pagina (ex.: `GetAllDescriptions()`).
- O metodo deve combinar `descriptions` e `genericErrorDescriptions`.
- Deduplicacao por `TypeId`, com prioridade para descricoes explicitas/custom sobre genericas quando houver colisao.
- O metodo deve retornar uma leitura estavel sem expor os dicionarios internos para mutacao externa.

4. Regra de roteamento
- Mapeamento sempre explicito pelo desenvolvedor na app.
- Overload sem parametro usa `/.problems`.
- Overload com parametro permite rota custom.

5. Regras de URI de tipo
- Se `description.Type` existir, usar valor explicito.
- Se nao existir, gerar URI por `BaseAddress + TypeComplement + TypeId`.
- Se `BaseAddress` estiver no default (`tag:problemdetails/.problems`), usar `pageUri` como base efetiva para a pagina ficar navegavel na aplicacao.

6. Localizacao
- Substituir strings hardcoded por recursos.
- Definir idioma padrao em `R.resx` e traducao em `R.pt-BR.resx`.
- O renderer usa `CultureInfo.CurrentUICulture` de forma indireta via classe de recursos.
- Os novos recursos ficam em `Descriptions/Documentation/R.resx` e `Descriptions/Documentation/R.pt-BR.resx`.
- Nao reutilizar `Descriptions/DR.resx`, pois ele descreve ProblemDetails e mensagens de conversao, nao textos da pagina HTML.
- Testes de localizacao devem alterar `CultureInfo.CurrentUICulture` de forma isolada e restaurar a cultura anterior ao final.

7. Regra de "efetivamente em uso"
- A listagem principal sempre usa tipos conhecidos retornados pelo descriptor.
- Nesta entrega, nao haverá filtro por tipos observados em runtime.
- Observacao de uso real pode existir apenas como metadado opcional futuro, sem remover itens da listagem.

8. Seguranca e robustez
- Escapar todo texto dinamico com HTML encode.
- Nao inserir dados de usuario em CSS/atributos sem sanitizacao.
- Tratar route pattern vazio/nulo com `ArgumentException`.
- Normalizar `SectionId`/anchors derivados de `TypeId` para evitar ids invalidos, duplicados ou dificeis de navegar.
- Manter um fallback deterministico para anchors vazias ou colidentes.

9. Relacao com o prototipo da outra lib
- Reaproveitar o prototipo somente como mapa funcional: endpoint, modelo, renderer, dados exibidos e testes esperados.
- Nao copiar classes CSS, tokens, hierarquia visual, paleta ou composicao do prototipo.
- A implementacao local deve parecer uma pagina nova do ecossistema RoyalCode.SmartProblems.

## Direcao Visual (diferente do prototipo implementado)
### Regra principal
O que ja existe em `.ai/plans/descriptions-ui/ProblemDetailsDescriptionPageHtmlRenderer.cs` deve ser tratado como prova de conceito. A versao local deve preservar a funcao, mas trocar a linguagem visual: paleta, ritmo, densidade, componentes e nomes de classes.

### Conceito
"Technical Editorial": mistura de docs tecnica (github) com leitura confortavel (chatgpt/claude).

### Tokens de estilo
1. Cores
- Fundo com gradiente muito sutil em tons cinza quentes.
- Superficies com contraste leve e bordas finas.
- Acento primario em verde-azulado (nao roxo) para links e foco.
- Estados HTTP com paleta semaforica discreta (info, warning, severe, danger).

2. Tipografia
- Sans principal com personalidade (ex.: `Segoe UI Variable`, `Noto Sans`, `Helvetica Neue`, sans-serif).
- Mono para type URI e metadados (ex.: `Cascadia Code`, `Consolas`, monospace).
- Hierarquia clara entre `TypeId`, titulo e descricao.

3. Estrutura
- Topbar fixa com contexto RFC e resumo.
- Sidebar com navegacao por tipo.
- Conteudo principal em cards por problem type.
- Grid de metadados (TypeId, URI, Status, Origem).

4. Interacao
- Hover/focus evidentes e acessiveis.
- `:target` destacado no card ancorado.
- Responsividade: sidebar cai para fluxo vertical em telas menores.

## Plano de Implementacao
1. Etapa 1 - Preparar contrato de dados
- Criar metodo de leitura de descricoes no descriptor para suportar a pagina.
- Definir deduplicacao por `TypeId` e prioridade (explicito > generico).
- Garantir que todos os tipos genericos conhecidos sejam incluídos na listagem final.
- Retornar colecao somente leitura/iteravel, sem expor dicionarios internos.
- Resultado esperado: modelo itera sobre todos os tipos documentaveis sem reflection.

2. Etapa 2 - Model da pagina
- Implementar `ProblemDetailsDescriptionPageModel` e item interno.
- Calcular `PageTitle`, `PageDescription`, `PageUri`, `ResolvedTypeUri`, `UsesGeneratedTypeUri`, `StatusDisplay`.
- Calcular tambem `SectionId` e `NavigationHref`.
- Normalizar `SectionId` a partir do `TypeId`, com tratamento de caracteres especiais e colisoes.
- Ordenar itens por `TypeId` para estabilidade de output e testes.
- Resultado esperado: construcao deterministica do modelo.

3. Etapa 3 - Localizacao por recursos
- Criar `R.resx` e `R.pt-BR.resx` para textos da pagina.
- Ajustar `csproj` para gerar `R.Designer.cs`.
- Substituir strings fixas do model/renderer por recursos.
- Manter os recursos da UI separados de `DR.resx`.
- Resultado esperado: pagina muda idioma conforme cultura da thread.

4. Etapa 4 - Renderer HTML
- Implementar renderer por `StringBuilder` com CSS inline.
- Aplicar linguagem visual nova (tokens, layout, cards, badges).
- Garantir HTML sem JS e com encode em todos os campos dinamicos.
- Nao copiar markup, classes ou CSS do prototipo; usar uma identidade local nova.
- Resultado esperado: pagina renderizada com identidade visual nova e legivel.

5. Etapa 5 - Endpoint extension
- Implementar `MapProblemDetailsDescriptionPage()` com overload de pattern.
- Construir `pageUri` a partir de `HttpContext` (scheme, host, pathbase, path).
- Retornar `TypedResults.Content(html, "text/html; charset=utf-8")`.
- Definir metadados: name, display name, exclude from description, `Produces(200, "text/html")`.
- Resultado esperado: endpoint plug-and-play em minimal API, com rota padrao `/.problems`.

6. Etapa 6 - Integracao em app de teste
- Mapear endpoint em `RoyalCode.SmartProblems.TestsApi/Program.cs` para testes funcionais.
- Resultado esperado: rota acessivel no `WebApplicationFactory`.

7. Etapa 7 - Testes unitarios
- Model tests: lista, ordenacao, URI resolvida, override de `description.Type`, deduplicacao, inclusao dos genericos e `SectionId` sanitizado.
- Renderer tests: secoes esperadas, HTML encode, classes de status, textos localizados e ausencia de strings hardcoded principais.
- Testes de localizacao devem restaurar a cultura anterior ao finalizar.
- Resultado esperado: regra de negocio, i18n e seguranca do HTML validados.

8. Etapa 8 - Testes de integracao HTTP
- GET `/.problems` retorna 200.
- Content-Type `text/html`.
- HTML contem type ids conhecidos (ex.: `my-custom-type`, `not-found`).
- Overload de pattern custom responde no caminho configurado.
- Resultado esperado: fluxo ponta a ponta validado.

9. Etapa 9 - Fase opcional: observabilidade de uso real
- Registrar tipo emitido no ponto de conversao (`ProblemDetailsBuilder`/`ProblemDetailsConverter`).
- Manter contador e ultimo uso por tipo em servico singleton.
- Exibir marca "em uso" e metricas na pagina quando habilitado, sem ocultar tipos nao observados.
- Resultado esperado: diferenciar tipos conhecidos x tipos observados em runtime, mantendo listagem completa.

10. Etapa 10 - Revisao final
- Revisar nomes, XML docs e consistencia de namespace.
- Rodar suite de testes do projeto.
- Resultado esperado: feature pronta para merge.

## Criterios de Aceite
1. Endpoint de documentacao funcional em rota padrao e rota custom.
2. Mapeamento do endpoint e manual pelo desenvolvedor, sem auto-mapeamento.
3. Pagina lista tipos custom e genericos com URI final resolvida.
4. Strings da UI vindas de `R.resx` e `R.pt-BR.resx`.
5. HTML encode aplicado em todo dado textual dinamico.
6. Anchors/ids derivados de `TypeId` sao seguros, deterministicos e navegaveis.
7. Visual claramente diferente do prototipo inicial, mantendo usabilidade e responsividade.
8. Testes unitarios e de integracao cobrindo cenarios principais.
9. Nenhuma regressao nos testes existentes.

## Riscos e Mitigacoes
1. Risco: descriptor nao expor leitura agregada de descricoes.
- Mitigacao: criar metodo explicito no descriptor com contrato claro e cobertura de teste.

2. Risco: inconsistencias de URI quando `BaseAddress` default.
- Mitigacao: aplicar regra de base efetiva com `pageUri` na construcao do modelo.

3. Risco: regressao visual em mobile.
- Mitigacao: incluir breakpoints simples e asserts basicos de estrutura em testes de renderer.

4. Risco: custo de manter i18n da pagina.
- Mitigacao: manter chaves curtas e padronizar convencao de nomes no `R.resx`.

5. Risco: entender "em uso" como configurado e nao observado.
- Mitigacao: documentar na UI os dois conceitos ("conhecido" vs "observado").

6. Risco: `TypeId` conter caracteres ruins para anchor HTML.
- Mitigacao: sanitizar ids, manter mapa deterministico e cobrir em teste.

7. Risco: testes de localizacao vazarem cultura para outros testes.
- Mitigacao: encapsular troca de `CurrentUICulture` e restaurar no `finally`/`Dispose`.

## Definicao para "tipos efetivamente em uso"
1. Nao existe forma 100% confiavel de saber "uso real" apenas por leitura estatica de codigo.
2. Para saber uso real, e necessario observar requests em runtime.
3. Estrategia recomendada:
- Exibir todos os tipos conhecidos por configuracao (descriptor).
- Opcionalmente coletar tipos observados durante conversao para `ProblemDetails`.
- Marcar na pagina os tipos observados com contador de ocorrencias e timestamp de ultimo uso, sem filtrar a listagem principal.
