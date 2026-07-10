
Para o `DbContext` e `DbSet<TEntity>` tem as extensions `TryFindAsync` e `TryFindByAsync`, na classe `SmartProblemsEFExtensions`.

Essas extensions buscam por uma entidade pelo Id, ou por outro campo.

O que há de bom nisso é uma mensagem padronizada e rica, retornando um `FindResult` que gera um problem detalhado, com campos extendidos.

Pensando nisso, há buscas que requerem mais de um campo, não apenas um Id ou código único, mas alguma combinação, algo composto, sendo chave composta ou não.

No entanto, não temos uma forma de facilitar isso, um api fluente que ajude a aplicar vários filtros e gerar uma mensagem padronizada e rica para vários campos.

Para testar, foi criado `Country`, `State` e `City`, além do `CountryService`.

No entanto precisa de alguma elaboração para isso.

Eu queria criar algo como um ou mais `struct` sendo `readonly` e `ref`.
Ainda algum método que iniciasse uma busca complexa, como um `FindByCompose<TEntity>(...)`
que retorna um `readonly ref struct` e a partir disso se possa adicionar filtros, com métodos que tenham assinatura igual aos de `SmartProblemsEFExtensions`.
Por fim, geria um `FirstOrDefaultAsync`, ou `FindAsync`, um método final que executa, retornando um `FindResult` com a entidade ou o problema detalhado e rico.

