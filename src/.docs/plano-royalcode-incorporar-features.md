# Plano de implementacao - RoyalCode recebendo features exclusivas da FluentProblems

## Objetivo
Implementar no conjunto RoyalCode (SmartProblems + SmartValidations) as features faltantes que hoje existem na FluentProblems, respeitando modularizacao dos pacotes.

## Escopo fechado deste plano
1. Implementar pagina de descricao de problem details no ecossistema RoyalCode.
2. Implementar APIs de conveniencia que existem na FluentProblems e faltam no RoyalCode, exceto CheckIfNull e variantes.
3. Definir e implementar estrategia de customizacao runtime de mensagens de validacao, se aprovada.

## Restricao obrigatoria
CheckIfNull e todas as variantes ficam fora deste plano e nao entram no backlog.

## Premissas funcionais explicitas

### Premissa A - Pagina de descricao de problem details
A feature alvo deve oferecer:
1. Rota default para pagina de descricao.
2. Rota custom configuravel.
3. Renderizacao HTML a partir de modelo.
4. Uso do catalogo de descritores de problem details ja existente no RoyalCode.
5. Controle de exposicao por ambiente.

### Premissa B - APIs de conveniencia da FluentProblems faltantes no RoyalCode
Candidatas iniciais de implementacao:
1. AsInvalidParameter(this Problem problem, string property)
2. AsResult(this Problem problem)
3. AsResult<T>(this Problem problem)
4. AsResult(this Problems problems)
5. AsResult<T>(this Problems problems)

### Premissa C - Customizacao de mensagens
A FluentProblems permite customizacao runtime de templates e display names no fluxo de validacao.
No RoyalCode, as mensagens atuais seguem recursos internos sem objeto publico equivalente no mesmo formato.

## Backlog de implementacao por fase

### Fase 1 - Pagina de descricao de problem details
Objetivo: entregar feature funcional equivalente no ecossistema RoyalCode.

Tarefas:
1. Definir pacote destino principal da feature (preferencia: camada ProblemDetails).
2. Implementar endpoint com rota default.
3. Implementar opcao de rota custom.
4. Implementar modelador de dados da pagina usando descritores existentes.
5. Implementar renderizador HTML desacoplado do endpoint.
6. Implementar politica de exposicao por ambiente.

Criterio de aceite da fase:
1. Endpoint responde com HTML valido.
2. Conteudo reflete o catalogo de problem details registrado.
3. Rota default e rota custom funcionando.
4. Feature pode ser desabilitada em ambiente produtivo conforme configuracao.

### Fase 2 - APIs de conveniencia (exceto CheckIfNull)
Objetivo: reduzir friccao de conversao Problem/Problems para Result no RoyalCode.

Tarefas:
1. Implementar AsInvalidParameter em local apropriado de extensoes.
2. Implementar AsResult para Problem e Problems, incluindo variante generica.
3. Garantir naming consistente com padrao do RoyalCode.
4. Validar coexistencia com APIs atuais para evitar ambiguidade de chamada.

Criterio de aceite da fase:
1. Conversoes funcionam em cenarios de sucesso e falha.
2. Nao existe conflito de resolucao de overload.
3. Testes de regressao dos fluxos atuais passam sem alteracao de comportamento.

### Fase 3 - Customizacao runtime de mensagens de validacao
Objetivo: decidir e implementar mecanismo configuravel de mensagens no SmartValidations.

Tarefas:
1. Definir decisao de produto:
- adotar integralmente
- adotar parcialmente
- nao adotar
2. Se aprovado, implementar contrato publico minimo para customizacao de templates.
3. Definir suporte a display names customizaveis no mesmo contrato ou contrato separado.
4. Garantir fallback para mensagens default quando nao houver customizacao.

Criterio de aceite da fase:
1. Decisao registrada com trade-offs tecnicos.
2. Se implementado, configuracao runtime muda mensagens sem quebrar regras existentes.
3. Testes cobrem default e customizado.

## Especificacao de impacto por pacote
1. SmartProblems:
- APIs de conveniencia de Problem/Problems para Result.
2. SmartProblems.ProblemDetails:
- endpoint, modelagem e renderizacao da pagina de descricao.
3. SmartValidations:
- contrato de customizacao runtime de mensagens, se aprovado.

## Requisitos nao funcionais
1. Compatibilidade: manter chamadas atuais funcionando.
2. SemVer: registrar breaking change se houver.
3. Observabilidade: logs claros em erros de renderizacao e configuracao.
4. Testabilidade: cada fase precisa de testes unitarios e de integracao.

## Definicao de pronto deste plano
1. As 3 fases implementadas ou com PR aberto por fase.
2. Catalogo de features implementadas exclui CheckIfNull e variantes.
3. Pagina de descricao de problem details entregue e testada.
4. Changelog e notas de compatibilidade atualizados.
