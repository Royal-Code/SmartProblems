using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace RoyalCode.SmartProblems.Entities;

/// <summary>
/// <para>
///     Struct that represents a result of a search operation.
/// </para>
/// <para>
///     It can has a value of type <typeparamref name="TEntity"/> or a <see cref="ProblemCategory.NotFound"/> problem.
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
    public static implicit operator FindResult<TEntity>(TEntity entity) => new(entity);

    /// <summary>
    /// Implicit operator to convert <see cref="Problems"/> into a <see cref="FindResult{TEntity}"/>.
    /// </summary>
    /// <param name="problems"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator FindResult<TEntity>(Problems problems) => new(problems);

    /// <summary>
    /// Implicit operator to convert a <see cref="RoyalCode.SmartProblems.Problem"/> into a <see cref="FindResult{TEntity}"/>.
    /// </summary>
    /// <param name="problem"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator FindResult<TEntity>(Problem problem) => new(problem);

    /// <summary>
    /// Implicit operator to convert a <see cref="Result{TEntity}"/> into a <see cref="FindResult{TEntity}"/>.
    /// </summary>
    /// <param name="result"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator FindResult<TEntity>(Result<TEntity> result) => result.Match(value => new FindResult<TEntity>(value), problems => new FindResult<TEntity>(problems));

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
        var datails = string.Format(R.EntityNotFoundBy, entityName, propertyName, propertyValue);

        return Problems.NotFound(datails)
            .With("entity", typeof(TEntity).Name)
            .With(propertyName, propertyValue);
    }

    private readonly Problems? problems;

    /// <summary>
    /// Constructor to create a <see cref="FindResult{TEntity}"/> from an entity.
    /// </summary>
    /// <param name="entity">The referenced entity.</param>
    public FindResult(TEntity entity)
    {
        Entity = entity;
    }

    /// <summary>
    /// Constructor to create a <see cref="FindResult{TEntity}"/> from a problem.
    /// </summary>
    /// <param name="problems"></param>
    public FindResult(Problems? problems)
    {
        this.problems = problems;
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
    /// <param name="problems">The problems associated with the entity, if not found.</param>
    /// <returns>True if the entity was not found, false otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [MemberNotNullWhen(false, nameof(Entity))]
    public bool NotFound([NotNullWhen(true)] out Problems? problems)
    {
        if (Entity is null)
        {
            var datails = string.Format(R.EntityNotFoundBy, DisplayNames.Instance.GetDisplayName(typeof(TEntity)));
            problems = this.problems ?? Problems.NotFound(datails).With("entity", typeof(TEntity).Name);
            return true;
        }

        problems = null;
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
        if (NotFound(out var notFoundProblems))
            return notFoundProblems;
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
        if (NotFound(out var notFoundProblems))
            return notFoundProblems;
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
        if (NotFound(out var notFoundProblems))
            return notFoundProblems;
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
        if (NotFound(out var notFoundProblems))
            return Task.FromResult(new Result(notFoundProblems));
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
    /// Map the entity to a new value if it was found, otherwise returns a <see cref="Result{TValue}"/> with the problems.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to be returned.</typeparam>
    /// <param name="receiver">The function to be executed if the entity was found.</param>
    /// <returns>A <see cref="Result{TValue}"/> indicating the success or failure of the operation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<TValue> Map<TValue>(Func<TEntity, Result<TValue>> receiver)
    {
        if (NotFound(out var notFoundProblems))
            return notFoundProblems;
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
        if (NotFound(out var notFoundProblems))
            return notFoundProblems;
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
        if (NotFound(out var notFoundProblems))
            return Task.FromResult(new Result<TValue>(notFoundProblems));
        return receiver(Entity);
    }

    /// <summary>
    /// Converts the referenced entity into a <see cref="Result{TEntity}"/>.
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<TEntity> ToResult()
    {
        if (NotFound(out var notFoundProblems))
            return notFoundProblems;
        return Entity;
    }
}

/// <summary>
/// <para>
///     Struct that represents a result of a search operation.
/// </para>
/// <para>
///     It can has a value of type <typeparamref name="TEntity"/> or a <see cref="ProblemCategory.NotFound"/> problem.
/// </para>
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TId">The type of the identifier.</typeparam>
public readonly struct FindResult<TEntity, TId>
    where TEntity : class
{
    #region Implicit Operators

    /// <summary>
    /// Implicit operator to convert an entity and id into a <see cref="FindResult{TEntity, TId}"/>.
    /// </summary>
    /// <param name="p">The entity and the identifier.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator FindResult<TEntity, TId>((TEntity entity, TId id) p) => new(p.entity, p.id);

    /// <summary>
    /// Implicit operator for converting a <see cref="FindResult{TEntity, TId}"/> into a result.
    /// </summary>
    /// <param name="entry"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Result<TEntity>(FindResult<TEntity, TId> entry) => entry.ToResult();

    #endregion

    private readonly TId id;

    /// <summary>
    /// Creates a new <see cref="FindResult{TEntity}"/> from an entity and an identifier.
    /// </summary>
    /// <param name="entity">A entidade referenciada.</param>
    /// <param name="id">O identificador da entidade.</param>
    public FindResult(TEntity? entity, TId id)
    {
        Entity = entity;
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
    /// <param name="problems">The problems associated with the entity, if not found.</param>
    /// <returns>True if the entity was not found, false otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [MemberNotNullWhen(false, nameof(Entity))]
    public bool NotFound([NotNullWhen(true)] out Problems? problems)
    {
        if (Entity is null)
        {
            var datails = string.Format(
                R.EntityNotFoundById,
                DisplayNames.Instance.GetDisplayName(typeof(TEntity)),
                id);

            problems = Problems.NotFound(datails)
                .With("entity", typeof(TEntity).Name)
                .With("id", id);

            return true;
        }

        problems = null;
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
        if (NotFound(out var notFoundProblems))
            return notFoundProblems;
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
        if (NotFound(out var notFoundProblems))
            return notFoundProblems;
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
        if (NotFound(out var notFoundProblems))
            return notFoundProblems;
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
        if (NotFound(out var notFoundProblems))
            return Task.FromResult(new Result(notFoundProblems));
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
    /// Map the entity to a new value if it was found, otherwise returns a <see cref="Result{TValue}"/> with the problems.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to be returned.</typeparam>
    /// <param name="receiver">The function to be executed if the entity was found.</param>
    /// <returns>A <see cref="Result{TValue}"/> indicating the success or failure of the operation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<TValue> Map<TValue>(Func<TEntity, Result<TValue>> receiver)
    {
        if (NotFound(out var notFoundProblems))
            return notFoundProblems;
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
        if (NotFound(out var notFoundProblems))
            return notFoundProblems;
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
        if (NotFound(out var notFoundProblems))
            return Task.FromResult(new Result<TValue>(notFoundProblems));
        return receiver(Entity);
    }

    /// <summary>
    /// Converts the referenced entity into a <see cref="Result{TEntity}"/>.
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<TEntity> ToResult()
    {
        if (NotFound(out var notFoundProblems))
            return notFoundProblems;
        return Entity;
    }
}