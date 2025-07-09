using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace RoyalCode.SmartProblems.Entities;

/// <summary>
/// <para>
///     Struct that represents a result of a search operation.
/// </para>
/// <para>
///     It can has a value of type <typeparamref name="TEntity"/> or a problem 
///     of type <see cref="ProblemCategory.NotFound"/> or <see cref="ProblemCategory.InvalidParameter"/>.
/// </para>
/// </summary>
public readonly struct FindResult<TEntity>
{
    #region Implicit Operators

    /// <summary>
    /// Implicit operator to convert an entity into a <see cref="FindResult{TEntity}"/>.
    /// </summary>
    /// <param name="entity"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator FindResult<TEntity>(TEntity? entity) => new(entity);

    /// <summary>
    /// Implicit operator to convert a <see cref="RoyalCode.SmartProblems.Problem"/> into a <see cref="FindResult{TEntity}"/>.
    /// </summary>
    /// <param name="problem"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator FindResult<TEntity>(Problem problem) => new(problem);

    /// <summary>
    /// Implicit operator to convert a <see cref="Problems"/> into a <see cref="FindResult{TEntity}"/>.
    /// </summary>
    /// <param name="problems"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator FindResult<TEntity>(Problems problems) => new(problems);

    /// <summary>
    /// Implicit operator to convert a <see cref="Result{TEntity}"/> into a <see cref="FindResult{TEntity}"/>.
    /// </summary>
    /// <param name="result"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator FindResult<TEntity>(Result<TEntity> result) => result.Match(
        value => new FindResult<TEntity>(value),
        problems => new FindResult<TEntity>(problems));

    /// <summary>
    /// Implicit operator to convert a tuple into a <see cref="FindResult{TEntity}"/> creating a problem.
    /// </summary>
    /// <param name="p">
    /// <para>
    ///     Tuple with the data needed to generate a problem:
    /// </para>
    /// <list type="bullet">
    ///     <item>
    ///         <description>byName</description>
    ///         <para>
    ///             Name of the field by which the entity was searched.
    ///         </para>
    ///     </item>
    ///         <item>
    ///         <description>propertyName</description>
    ///         <para>
    ///             Name of the property to be added as extra data to the problem.
    ///         </para>
    ///     </item>
    ///     <item>
    ///         <description>objectValue</description>
    ///         <para>
    ///             Value used to search for the entity.
    ///         </para>
    ///     </item>
    /// </list>
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator FindResult<TEntity>((string byName, string propertyName, object? objectValue) p)
        => Problem(p.byName, p.propertyName, p.objectValue);

    #endregion

    #region Factory Methods

    /// <summary>
    /// <para>
    ///     Creates a new <see cref="FindResult{TEntity}"/> for when the entity is not found, generating
    ///     a category problem <see cref="ProblemCategory.NotFound"/> and using a default message.
    /// </para>
    /// <para>
    ///     The standard message uses the display name of the entity (<typeparamref name="TEntity"/>),
    ///     the name of the field for which the entity was searched (<paramref name="byName"/>),
    ///     and the value used in the search (<paramref name="propertyValue"/>).
    /// </para>
    /// </summary>
    /// <param name="byName">Name of the field by which the entity was searched.</param>
    /// <param name="propertyName">Name of the property to be added as extra data to the problem.</param>
    /// <param name="propertyValue">Value used for research.</param>
    /// <returns>
    ///     A new <see cref="FindResult{TEntity}"/> with the problem generated.
    /// </returns>
    public static FindResult<TEntity> Problem(string byName, string propertyName, object? propertyValue)
    {
        var entityName = DisplayNames.Instance.GetDisplayName(typeof(TEntity));
        var datails = string.Format(R.EntityNotFoundBy, entityName, byName, propertyValue);

        return Problems.NotFound(datails)
            .With("entity", typeof(TEntity).Name)
            .With(propertyName, propertyValue);
    }

    #endregion

    private readonly Problem? problem;

    /// <summary>
    /// Constructor to create a <see cref="FindResult{TEntity}"/>.
    /// </summary>
    public FindResult() { }

    /// <summary>
    /// Constructor to create a <see cref="FindResult{TEntity}"/> from an entity.
    /// </summary>
    /// <param name="entity">The referenced entity.</param>
    public FindResult(TEntity? entity)
    {
        Entity = entity;
    }

    /// <summary>
    /// Constructor to create a <see cref="FindResult{TEntity}"/> from a problem.
    /// </summary>
    /// <param name="problem"></param>
    public FindResult(Problem? problem)
    {
        this.problem = problem;
    }

    /// <summary>
    /// Constructor to create a <see cref="FindResult{TEntity}"/> from a problem.
    /// </summary>
    /// <param name="problems"></param>
    public FindResult(Problems problems)
    {
        string message = string.Format(
                    R.EntityNotFound,
                    DisplayNames.Instance.GetDisplayName(typeof(TEntity)));

        problem = Problems.NotFound(message)
            .With("entity", typeof(TEntity).Name)
            .With("problems", problems);
    }

    /// <summary>
    /// The referenced entity, if available.
    /// </summary>
    public TEntity? Entity { get; }

    /// <summary>
    /// Checks if the entity was found, i.e. if the value of <see cref="Entity"/> is not null.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Entity))]
    public bool Found => Entity is not null;

    /// <summary>
    /// Checks if the entity was not found, i.e. if the value of <see cref="Entity"/> is null.
    /// </summary>
    /// <param name="problem">The problems (<see cref="ProblemCategory.NotFound"/>) associated with the entity, if not found.</param>
    /// <returns>True if the entity was not found, false otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [MemberNotNullWhen(false, nameof(Entity))]
    public bool NotFound([NotNullWhen(true)] out Problem? problem)
    {
        if (Entity is null)
        {
            var datails = string.Format(R.EntityNotFound, DisplayNames.Instance.GetDisplayName(typeof(TEntity)));
            problem = this.problem ?? Problems.NotFound(datails).With("entity", typeof(TEntity).Name);
            return true;
        }

        problem = null;
        return false;
    }

    /// <summary>
    /// Checks if the entity was not found, i.e. if the value of <see cref="Entity"/> is null.
    /// </summary>
    /// <param name="problem">The problems (<see cref="ProblemCategory.InvalidParameter"/>) associated with the entity, if not found.</param>
    /// <param name="parameterName">The name of the parameter that caused the problem, if applicable.</param>
    /// <returns>True if the entity was not found, false otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [MemberNotNullWhen(false, nameof(Entity))]
    public bool HasInvalidParameter([NotNullWhen(true)] out Problem? problem, string? parameterName = null)
    {
        if (Entity is null)
        {
            var datails = string.Format(R.EntityNotFound, DisplayNames.Instance.GetDisplayName(typeof(TEntity)));
            problem = this.problem ?? Problems.InvalidParameter(datails, parameterName).With("entity", typeof(TEntity).Name);
            return true;
        }

        problem = null;
        return false;
    }

    /// <summary>
    /// Converts this find result into a <see cref="Result"/> and executes the receiver action if the entity was found.
    /// </summary>
    /// <param name="receiver">The action to be executed if the entity was found.</param>
    /// <returns>A <see cref="Result"/> indicating the success or failure of the operation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result Collect(Action<TEntity> receiver)
    {
        if (NotFound(out var notFoundProblem))
            return notFoundProblem;
        receiver(Entity);
        return Result.Ok();
    }

    /// <summary>
    /// Converts this find result into a <see cref="Result"/> and executes the receiver action if the entity was found.
    /// </summary>
    /// <param name="parameterName">The name of the parameter that caused the problem, if applicable.</param>
    /// <param name="receiver">The action to be executed if the entity was found.</param>
    /// <returns>A <see cref="Result"/> indicating the success or failure of the operation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result Collect(string parameterName, Action<TEntity> receiver)
    {
        if (HasInvalidParameter(out var problem, parameterName))
            return problem;
        receiver(Entity);
        return Result.Ok();
    }

    /// <summary>
    /// Converts this find result into a <see cref="Result"/> and executes the receiver function if the entity was found.
    /// </summary>
    /// <param name="receiver">The function to be executed if the entity was found.</param>
    /// <returns>A <see cref="Result"/> indicating the success or failure of the operation.</returns>
    public async Task<Result> CollectAsync(Func<TEntity, Task> receiver)
    {
        if (NotFound(out var notFoundProblem))
            return notFoundProblem;
        await receiver(Entity);
        return Result.Ok();
    }

    /// <summary>
    /// Converts this find result into a <see cref="Result"/> and executes the receiver function if the entity was found.
    /// </summary>
    /// <param name="parameterName">The name of the parameter that caused the problem, if applicable.</param>
    /// <param name="receiver">The function to be executed if the entity was found.</param>
    /// <returns>A <see cref="Result"/> indicating the success or failure of the operation.</returns>
    public async Task<Result> CollectAsync(string parameterName, Func<TEntity, Task> receiver)
    {
        if (HasInvalidParameter(out var problem, parameterName))
            return problem;
        await receiver(Entity);
        return Result.Ok();
    }

    /// <summary>
    /// Continues the operation with the entity if it was found, otherwise returns a <see cref="Result"/> with the problems.
    /// </summary>
    /// <param name="receiver">The function to be executed if the entity was found.</param>
    /// <returns>A <see cref="Result"/> indicating the success or failure of the operation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result Continue(Func<TEntity, Result> receiver)
    {
        if (NotFound(out var notFoundProblem))
            return notFoundProblem;
        return receiver(Entity);
    }

    /// <summary>
    /// Continues the operation with the entity if it was found, otherwise returns a <see cref="Result"/> with the problems.
    /// </summary>
    /// <param name="parameterName">The name of the parameter that caused the problem, if applicable.</param>
    /// <param name="receiver">The function to be executed if the entity was found.</param>
    /// <returns>A <see cref="Result"/> indicating the success or failure of the operation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result Continue(string parameterName, Func<TEntity, Result> receiver)
    {
        if (HasInvalidParameter(out var problem, parameterName))
            return problem;
        return receiver(Entity);
    }

    /// <summary>
    /// Continues the operation with the entity if it was found, otherwise returns a <see cref="Result"/> with the problems.
    /// </summary>
    /// <param name="receiver">The function to be executed if the entity was found.</param>
    /// <returns>A <see cref="Result"/> indicating the success or failure of the operation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task<Result> ContinueAsync(Func<TEntity, Task<Result>> receiver)
    {
        if (NotFound(out var notFoundProblem))
            return Task.FromResult(new Result(notFoundProblem));
        return receiver(Entity);
    }

    /// <summary>
    /// Continues the operation with the entity if it was found, otherwise returns a <see cref="Result"/> with the problems.
    /// </summary>
    /// <param name="parameterName">The name of the parameter that caused the problem, if applicable.</param>
    /// <param name="receiver">The function to be executed if the entity was found.</param>
    /// <returns>A <see cref="Result"/> indicating the success or failure of the operation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task<Result> ContinueAsync(string parameterName, Func<TEntity, Task<Result>> receiver)
    {
        if (HasInvalidParameter(out var problem, parameterName))
            return Task.FromResult(new Result(problem));
        return receiver(Entity);
    }

    /// <summary>
    /// Map the entity to a new value if it was found, otherwise returns a <see cref="Result{TValue}"/> with the problems.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to be returned.</typeparam>
    /// <param name="receiver">The function to be executed if the entity was found.</param>
    /// <returns>A <see cref="Result{TValue}"/> indicating the success or failure of the operation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<TValue> Map<TValue>(Func<TEntity, TValue> receiver)
    {
        if (NotFound(out var notFoundProblem))
            return notFoundProblem;
        return receiver(Entity);
    }

    /// <summary>
    /// Map the entity to a new value if it was found, otherwise returns a <see cref="Result{TValue}"/> with the problems.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to be returned.</typeparam>
    /// <param name="parameterName">The name of the parameter that caused the problem, if applicable.</param>
    /// <param name="receiver">The function to be executed if the entity was found.</param>
    /// <returns>A <see cref="Result{TValue}"/> indicating the success or failure of the operation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<TValue> Map<TValue>(string parameterName, Func<TEntity, TValue> receiver)
    {
        if (HasInvalidParameter(out var problem, parameterName))
            return problem;
        return receiver(Entity);
    }

    /// <summary>
    /// Map the entity to a new value if it was found, otherwise returns a <see cref="Result{TValue}"/> with the problems.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to be returned.</typeparam>
    /// <param name="receiver">The function to be executed if the entity was found.</param>
    /// <returns>A <see cref="Result{TValue}"/> indicating the success or failure of the operation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<TValue> Map<TValue>(Func<TEntity, Result<TValue>> receiver)
    {
        if (NotFound(out var notFoundProblem))
            return notFoundProblem;
        return receiver(Entity);
    }

    /// <summary>
    /// Map the entity to a new value if it was found, otherwise returns a <see cref="Result{TValue}"/> with the problems.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to be returned.</typeparam>
    /// <param name="parameterName">The name of the parameter that caused the problem, if applicable.</param>
    /// <param name="receiver">The function to be executed if the entity was found.</param>
    /// <returns>A <see cref="Result{TValue}"/> indicating the success or failure of the operation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<TValue> Map<TValue>(string parameterName, Func<TEntity, Result<TValue>> receiver)
    {
        if (HasInvalidParameter(out var problem, parameterName))
            return problem;
        return receiver(Entity);
    }

    /// <summary>
    /// Map the entity to a new value if it was found, otherwise returns a <see cref="Result{TValue}"/> with the problems.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to be returned.</typeparam>
    /// <param name="receiver">The function to be executed if the entity was found.</param>
    /// <returns>A <see cref="Result{TValue}"/> indicating the success or failure of the operation.</returns>
    public async Task<Result<TValue>> MapAsync<TValue>(Func<TEntity, Task<TValue>> receiver)
    {
        if (NotFound(out var notFoundProblem))
            return notFoundProblem;
        return await receiver(Entity);
    }

    /// <summary>
    /// Map the entity to a new value if it was found, otherwise returns a <see cref="Result{TValue}"/> with the problems.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to be returned.</typeparam>
    /// <param name="parameterName">The name of the parameter that caused the problem, if applicable.</param>
    /// <param name="receiver">The function to be executed if the entity was found.</param>
    /// <returns>A <see cref="Result{TValue}"/> indicating the success or failure of the operation.</returns>
    public async Task<Result<TValue>> MapAsync<TValue>(string parameterName, Func<TEntity, Task<TValue>> receiver)
    {
        if (HasInvalidParameter(out var problem, parameterName))
            return problem;
        return await receiver(Entity);
    }

    /// <summary>
    /// Map the entity to a new value if it was found, otherwise returns a <see cref="Result{TValue}"/> with the problems.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to be returned.</typeparam>
    /// <param name="receiver">The function to be executed if the entity was found.</param>
    /// <returns>A <see cref="Result{TValue}"/> indicating the success or failure of the operation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task<Result<TValue>> MapAsync<TValue>(Func<TEntity, Task<Result<TValue>>> receiver)
    {
        if (NotFound(out var notFoundProblem))
            return Task.FromResult(new Result<TValue>(notFoundProblem));
        return receiver(Entity);
    }

    /// <summary>
    /// Map the entity to a new value if it was found, otherwise returns a <see cref="Result{TValue}"/> with the problems.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to be returned.</typeparam>
    /// <param name="parameterName">The name of the parameter that caused the problem, if applicable.</param>
    /// <param name="receiver">The function to be executed if the entity was found.</param>
    /// <returns>A <see cref="Result{TValue}"/> indicating the success or failure of the operation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task<Result<TValue>> MapAsync<TValue>(string parameterName, Func<TEntity, Task<Result<TValue>>> receiver)
    {
        if (HasInvalidParameter(out var problem, parameterName))
            return Task.FromResult(new Result<TValue>(problem));
        return receiver(Entity);
    }

    /// <summary>
    /// Converts the referenced entity into a <see cref="Result{TEntity}"/>.
    /// </summary>
    /// <returns>
    ///     A <see cref="Result{TEntity}"/> containing the entity if found, or a <see cref="ProblemCategory.NotFound"/> problem if not found.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<TEntity> ToResult()
    {
        if (NotFound(out var notFoundProblem))
            return notFoundProblem;
        return Entity;
    }

    /// <summary>
    /// Converts the referenced entity into a <see cref="Result{TEntity}"/>.
    /// </summary>
    /// <param name="parameterName">The name of the parameter that caused the problem, if applicable.</param>
    /// <returns>
    ///     A <see cref="Result{TEntity}"/> containing the entity if found, or a <see cref="ProblemCategory.InvalidParameter"/> problem if not found.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<TEntity> ToResult(string parameterName)
    {
        if (HasInvalidParameter(out var problem, parameterName))
            return problem;
        return Entity;
    }
}

/// <summary>
/// <para>
///     Struct that represents a result of a search operation.
/// </para>
/// <para>
///     It can has a value of type <typeparamref name="TEntity"/> or a problem 
///     of type <see cref="ProblemCategory.NotFound"/> or <see cref="ProblemCategory.InvalidParameter"/>.
/// </para>
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TId">The type of the identifier.</typeparam>
public readonly struct FindResult<TEntity, TId>
    where TEntity : class
{
    #region Implicit Operators

    /// <summary>
    /// Implicit operator for converting a <see cref="FindResult{TEntity, TId}"/> into a result.
    /// </summary>
    /// <param name="entry"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Result<TEntity>(FindResult<TEntity, TId> entry) => entry.ToResult();

    #endregion

    private readonly TId id;

    /// <summary>
    /// Creates a new <see cref="FindResult{TEntity, TId}"/> from an entity and an identifier.
    /// </summary>
    /// <param name="entity">A entidade referenciada.</param>
    /// <param name="id">O identificador da entidade.</param>
    public FindResult(TEntity? entity, TId id)
    {
        Entity = entity;
        this.id = id;
    }

    /// <summary>
    /// Creates a new <see cref="FindResult{TEntity, TId}"/> from an entity and an identifier.
    /// </summary>
    /// <param name="id">O identificador da entidade.</param>
    public FindResult(TId id)
    {
        this.id = id;
    }

    /// <summary>
    /// The referenced entity, if available.
    /// </summary>
    public TEntity? Entity { get; }

    /// <summary>
    /// Checks if the entity was found, i.e. if the value of <see cref="Entity"/> is not null.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Entity))]
    public bool Found => Entity is not null;

    /// <summary>
    /// Checks if the entity was not found, i.e. if the value of <see cref="Entity"/> is null.
    /// </summary>
    /// <param name="problem">The problem (<see cref="ProblemCategory.NotFound"/>) associated with the entity, if not found.</param>
    /// <returns>True if the entity was not found, false otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [MemberNotNullWhen(false, nameof(Entity))]
    public bool NotFound([NotNullWhen(true)] out Problem? problem)
    {
        if (Entity is null)
        {
            var datails = string.Format(
                R.EntityNotFoundById,
                DisplayNames.Instance.GetDisplayName(typeof(TEntity)),
                id);

            problem = Problems.NotFound(datails)
                .With("entity", typeof(TEntity).Name)
                .With("id", id);

            return true;
        }

        problem = null;
        return false;
    }

    /// <summary>
    /// Checks if the entity was not found, i.e. if the value of <see cref="Entity"/> is null.
    /// </summary>
    /// <param name="problem">The problem (<see cref="ProblemCategory.InvalidParameter"/>) associated with the entity, if not found.</param>
    /// <param name="parameterName">The name of the parameter that caused the problem, if applicable.</param>
    /// <returns>True if the entity was not found, false otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [MemberNotNullWhen(false, nameof(Entity))]
    public bool HasInvalidParameter([NotNullWhen(true)] out Problem? problem, string? parameterName = null)
    {
        if (Entity is null)
        {
            var datails = string.Format(
                R.EntityNotFoundById,
                DisplayNames.Instance.GetDisplayName(typeof(TEntity)),
                id);

            problem = Problems.InvalidParameter(datails, parameterName)
                .With("entity", typeof(TEntity).Name)
                .With("id", id);

            return true;
        }

        problem = null;
        return false;
    }


    /// <summary>
    /// Converts this find result into a <see cref="Result"/> and executes the receiver action if the entity was found.
    /// </summary>
    /// <param name="receiver">The action to be executed if the entity was found.</param>
    /// <returns>A <see cref="Result"/> indicating the success or failure of the operation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result Collect(Action<TEntity> receiver)
    {
        if (NotFound(out var notFoundProblem))
            return notFoundProblem;
        receiver(Entity);
        return Result.Ok();
    }

    /// <summary>
    /// Converts this find result into a <see cref="Result"/> and executa a ação se a entidade foi encontrada, ou retorna um problema de parâmetro inválido.
    /// </summary>
    /// <param name="parameterName">O nome do parâmetro que causou o problema, se aplicável.</param>
    /// <param name="receiver">A ação a ser executada se a entidade for encontrada.</param>
    /// <returns>Um <see cref="Result"/> indicando sucesso ou falha.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result Collect(string parameterName, Action<TEntity> receiver)
    {
        if (HasInvalidParameter(out var problem, parameterName))
            return problem;
        receiver(Entity);
        return Result.Ok();
    }

    /// <summary>
    /// Converts this find result into a <see cref="Result"/> and executes the receiver function if the entity was found.
    /// </summary>
    /// <param name="receiver">The function to be executed if the entity was found.</param>
    /// <returns>A <see cref="Result"/> indicating the success or failure of the operation.</returns>
    public async Task<Result> CollectAsync(Func<TEntity, Task> receiver)
    {
        if (NotFound(out var notFoundProblem))
            return notFoundProblem;
        await receiver(Entity);
        return Result.Ok();
    }

    /// <summary>
    /// Executa a função assíncrona se a entidade foi encontrada, ou retorna um problema de parâmetro inválido.
    /// </summary>
    /// <param name="parameterName">O nome do parâmetro que causou o problema, se aplicável.</param>
    /// <param name="receiver">A função assíncrona a ser executada se a entidade for encontrada.</param>
    /// <returns>Um <see cref="Result"/> indicando sucesso ou falha.</returns>
    public async Task<Result> CollectAsync(string parameterName, Func<TEntity, Task> receiver)
    {
        if (HasInvalidParameter(out var problem, parameterName))
            return problem;
        await receiver(Entity);
        return Result.Ok();
    }

    /// <summary>
    /// Continues the operation with the entity if it was found, otherwise returns a <see cref="Result"/> with the problems.
    /// </summary>
    /// <param name="receiver">The function to be executed if the entity was found.</param>
    /// <returns>A <see cref="Result"/> indicating the success or failure of the operation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result Continue(Func<TEntity, Result> receiver)
    {
        if (NotFound(out var notFoundProblem))
            return notFoundProblem;
        return receiver(Entity);
    }

    /// <summary>
    /// Continua a operação com a entidade se encontrada, ou retorna um problema de parâmetro inválido.
    /// </summary>
    /// <param name="parameterName">O nome do parâmetro que causou o problema, se aplicável.</param>
    /// <param name="receiver">A função a ser executada se a entidade for encontrada.</param>
    /// <returns>Um <see cref="Result"/> indicando sucesso ou falha.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result Continue(string parameterName, Func<TEntity, Result> receiver)
    {
        if (HasInvalidParameter(out var problem, parameterName))
            return problem;
        return receiver(Entity);
    }

    /// <summary>
    /// Continues the operation with the entity if it was found, otherwise returns a <see cref="Result"/> with the problems.
    /// </summary>
    /// <param name="receiver">The function to be executed if the entity was found.</param>
    /// <returns>A <see cref="Result"/> indicating the success or failure of the operation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task<Result> ContinueAsync(Func<TEntity, Task<Result>> receiver)
    {
        if (NotFound(out var notFoundProblem))
            return Task.FromResult(new Result(notFoundProblem));
        return receiver(Entity);
    }

    /// <summary>
    /// Continua a operação assíncrona com a entidade se encontrada, ou retorna um problema de parâmetro inválido.
    /// </summary>
    /// <param name="parameterName">O nome do parâmetro que causou o problema, se aplicável.</param>
    /// <param name="receiver">A função assíncrona a ser executada se a entidade for encontrada.</param>
    /// <returns>Um <see cref="Result"/> indicando sucesso ou falha.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task<Result> ContinueAsync(string parameterName, Func<TEntity, Task<Result>> receiver)
    {
        if (HasInvalidParameter(out var problem, parameterName))
            return Task.FromResult(new Result(problem));
        return receiver(Entity);
    }

    /// <summary>
    /// Map the entity to a new value if it was found, otherwise returns a <see cref="Result{TValue}"/> with the problems.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to be returned.</typeparam>
    /// <param name="receiver">The function to be executed if the entity was found.</param>
    /// <returns>A <see cref="Result{TValue}"/> indicating the success or failure of the operation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<TValue> Map<TValue>(Func<TEntity, TValue> receiver)
    {
        if (NotFound(out var notFoundProblems))
            return notFoundProblems;
        return receiver(Entity);
    }

    /// <summary>
    /// Mapeia a entidade para um novo valor se encontrada, ou retorna um problema de parâmetro inválido.
    /// </summary>
    /// <typeparam name="TValue">O tipo do valor a ser retornado.</typeparam>
    /// <param name="parameterName">O nome do parâmetro que causou o problema, se aplicável.</param>
    /// <param name="receiver">A função a ser executada se a entidade for encontrada.</param>
    /// <returns>Um <see cref="Result{TValue}"/> indicando sucesso ou falha.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<TValue> Map<TValue>(string parameterName, Func<TEntity, TValue> receiver)
    {
        if (HasInvalidParameter(out var problem, parameterName))
            return problem;
        return receiver(Entity);
    }

    /// <summary>
    /// Map the entity to a new value if it was found, otherwise returns a <see cref="Result{TValue}"/> with the problems.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to be returned.</typeparam>
    /// <param name="receiver">The function to be executed if the entity was found.</param>
    /// <returns>A <see cref="Result{TValue}"/> indicating the success or failure of the operation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<TValue> Map<TValue>(Func<TEntity, Result<TValue>> receiver)
    {
        if (NotFound(out var notFoundProblem))
            return notFoundProblem;
        return receiver(Entity);
    }

    /// <summary>
    /// Mapeia a entidade para um novo valor se encontrada, ou retorna um problema de parâmetro inválido.
    /// </summary>
    /// <typeparam name="TValue">O tipo do valor a ser retornado.</typeparam>
    /// <param name="parameterName">O nome do parâmetro que causou o problema, se aplicável.</param>
    /// <param name="receiver">A função a ser executada se a entidade for encontrada.</param>
    /// <returns>Um <see cref="Result{TValue}"/> indicando sucesso ou falha.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<TValue> Map<TValue>(string parameterName, Func<TEntity, Result<TValue>> receiver)
    {
        if (HasInvalidParameter(out var problem, parameterName))
            return problem;
        return receiver(Entity);
    }

    /// <summary>
    /// Map the entity to a new value if it was found, otherwise returns a <see cref="Result{TValue}"/> with the problems.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to be returned.</typeparam>
    /// <param name="receiver">The function to be executed if the entity was found.</param>
    /// <returns>A <see cref="Result{TValue}"/> indicating the success or failure of the operation.</returns>
    public async Task<Result<TValue>> MapAsync<TValue>(Func<TEntity, Task<TValue>> receiver)
    {
        if (NotFound(out var notFoundProblem))
            return notFoundProblem;
        return await receiver(Entity);
    }

    /// <summary>
    /// Mapeia a entidade para um novo valor de forma assíncrona se encontrada, ou retorna um problema de parâmetro inválido.
    /// </summary>
    /// <typeparam name="TValue">O tipo do valor a ser retornado.</typeparam>
    /// <param name="parameterName">O nome do parâmetro que causou o problema, se aplicável.</param>
    /// <param name="receiver">A função assíncrona a ser executada se a entidade for encontrada.</param>
    /// <returns>Um <see cref="Result{TValue}"/> indicando sucesso ou falha.</returns>
    public async Task<Result<TValue>> MapAsync<TValue>(string parameterName, Func<TEntity, Task<TValue>> receiver)
    {
        if (HasInvalidParameter(out var problem, parameterName))
            return problem;
        return await receiver(Entity);
    }

    /// <summary>
    /// Map the entity to a new value if it was found, otherwise returns a <see cref="Result{TValue}"/> with the problems.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to be returned.</typeparam>
    /// <param name="receiver">The function to be executed if the entity was found.</param>
    /// <returns>A <see cref="Result{TValue}"/> indicating the success or failure of the operation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task<Result<TValue>> MapAsync<TValue>(Func<TEntity, Task<Result<TValue>>> receiver)
    {
        if (NotFound(out var notFoundProblem))
            return Task.FromResult(new Result<TValue>(notFoundProblem));
        return receiver(Entity);
    }

    /// <summary>
    /// Mapeia a entidade para um novo valor de forma assíncrona se encontrada, ou retorna um problema de parâmetro inválido.
    /// </summary>
    /// <typeparam name="TValue">O tipo do valor a ser retornado.</typeparam>
    /// <param name="parameterName">O nome do parâmetro que causou o problema, se aplicável.</param>
    /// <param name="receiver">A função assíncrona a ser executada se a entidade for encontrada.</param>
    /// <returns>Um <see cref="Result{TValue}"/> indicando sucesso ou falha.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task<Result<TValue>> MapAsync<TValue>(string parameterName, Func<TEntity, Task<Result<TValue>>> receiver)
    {
        if (HasInvalidParameter(out var problem, parameterName))
            return Task.FromResult(new Result<TValue>(problem));
        return receiver(Entity);
    }

    /// <summary>
    /// Converts the referenced entity into a <see cref="Result{TEntity}"/>.
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<TEntity> ToResult()
    {
        if (NotFound(out var notFoundProblem))
            return notFoundProblem;
        return Entity;
    }

    /// <summary>
    /// Converte a entidade referenciada em um <see cref="Result{TEntity}"/>.
    /// </summary>
    /// <param name="parameterName">O nome do parâmetro que causou o problema, se aplicável.</param>
    /// <returns>Um <see cref="Result{TEntity}"/> contendo a entidade se encontrada, ou um problema de parâmetro inválido se não encontrada.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<TEntity> ToResult(string parameterName)
    {
        if (HasInvalidParameter(out var problem, parameterName))
            return problem;
        return Entity;
    }
}