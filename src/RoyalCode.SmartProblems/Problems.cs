using System.Collections;
using System.Runtime.CompilerServices;

namespace RoyalCode.SmartProblems;

#pragma warning disable S4050 // implement operatiors == != Equals GetHashCode

/// <summary>
/// A collection of problems that occurred in the system.
/// </summary>
public sealed class Problems : ICollection<Problem>
{
    #region Global Property

    /// <summary>
    /// <para>
    ///     An exception handler to convert exceptions to problems.
    /// </para>
    /// <para>
    ///     Is used in the <see cref="InternalError(Exception)"/> method.
    /// </para>
    /// <para>
    ///     Useful to customize the creation of problems from exceptions.
    /// </para>
    /// </summary>
    public static IExceptionHandler? ExceptionHandler { get; set; }

    /// <summary>
    /// <para>
    ///     Options to customize the creation of problems from exceptions.
    /// </para>
    /// <para>
    ///     Used in the <see cref="InternalError(Exception)"/> method.
    /// </para>
    /// </summary>
    public static ExceptionOptions ExceptionOptions { get; } = new();

    #endregion

    #region implicit operators

    /// <summary>
    /// Convert a problem to a collection of problems.
    /// </summary>
    /// <param name="problem"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Problems(Problem problem) => [problem];

    /// <summary>
    /// Convert a <see cref="Problem"/> to a <see cref="Task"/> of <see cref="Result"/>.
    /// </summary>
    /// <param name="problems"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Task<Result>(Problems problems) => Task.FromResult(new Result(problems));

    /// <summary>
    /// Add a problem to the collection of problems.
    /// </summary>
    /// <param name="collection">The collection of problems.</param>
    /// <param name="problem">The problem to add.</param>
    /// <returns>The same instance of the collection with the problem added.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Problems operator +(Problems collection, Problem problem)
    {
        collection.Add(problem);
        return collection;
    }

    /// <summary>
    /// Add a problem to the collection of problems.
    /// </summary>
    /// <param name="collection">The collection of problems.</param>
    /// <param name="problems">Other collection of problems to add.</param>
    /// <returns>The same instance of the collection with the problem added.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Problems operator +(Problems collection, IEnumerable<Problem> problems)
    {
        collection.AddRange(problems);
        return collection;
    }

    #endregion

    #region Factory Methods

    /// <summary>
    /// Create a new problem of NotFound category.
    /// </summary>
    /// <param name="detail">The detail of the problem, a description message for users.</param>
    /// <param name="property">Optional, the property that the problem is related to.</param>
    /// <param name="typeId">Optional, the type id of the problem, related to the problem details type.</param>
    /// <returns>A new problem.</returns>
    public static Problem NotFound(string detail, string? property = null, string? typeId = null)
    {
        return new Problem
        {
            Category = ProblemCategory.NotFound,
            Detail = detail,
            Property = property,
            TypeId = typeId
        };
    }

    /// <summary>
    /// Create a new problem of InvalidParameter category.
    /// </summary>
    /// <param name="detail">The detail of the problem, a description message for users.</param>
    /// <param name="property">Optional, the property that the problem is related to.</param>
    /// <param name="typeId">Optional, the type id of the problem, related to the problem details type.</param>
    /// <returns>A new problem.</returns>
    public static Problem InvalidParameter(string detail, string? property = null, string? typeId = null)
    {
        return new Problem
        {
            Category = ProblemCategory.InvalidParameter,
            Detail = detail,
            Property = property,
            TypeId = typeId
        };
    }

    /// <summary>
    /// Create a new problem of ValidationFailed category.
    /// </summary>
    /// <param name="detail">The detail of the problem, a description message for users.</param>
    /// <param name="property">Optional, the property that the problem is related to.</param>
    /// <param name="typeId">Optional, the type id of the problem, related to the problem details type.</param>
    /// <returns>A new problem.</returns>
    public static Problem ValidationFailed(string detail, string? property = null, string? typeId = null)
    {
        return new Problem
        {
            Category = ProblemCategory.ValidationFailed,
            Detail = detail,
            Property = property,
            TypeId = typeId
        };
    }

    /// <summary>
    /// Create a new problem of Not Allowed category.
    /// </summary>
    /// <param name="detail">The detail of the problem, a description message for users.</param>
    /// <param name="property">Optional, the property that the problem is related to.</param>
    /// <param name="typeId">Optional, the type id of the problem, related to the problem details type.</param>
    /// <returns>A new problem.</returns>
    public static Problem NotAllowed(string detail, string? property = null, string? typeId = null)
    {
        return new Problem
        {
            Category = ProblemCategory.NotAllowed,
            Detail = detail,
            Property = property,
            TypeId = typeId
        };
    }
    
    /// <summary>
    /// Create a new problem of InvalidState category.
    /// </summary>
    /// <param name="detail">The detail of the problem, a description message for users.</param>
    /// <param name="property">Optional, the property that the problem is related to.</param>
    /// <param name="typeId">Optional, the type id of the problem, related to the problem details type.</param>
    /// <returns>A new problem.</returns>
    public static Problem InvalidState(string detail, string? property = null, string? typeId = null)
    {
        return new Problem
        {
            Category = ProblemCategory.InvalidState,
            Detail = detail,
            Property = property,
            TypeId = typeId
        };
    }

    /// <summary>
    /// Create a new problem of InternalServerError category.
    /// </summary>
    /// <param name="detail">The detail of the problem, a description message for users.</param>
    /// <param name="typeId">Optional, the type id of the problem, related to the problem details type.</param>
    /// <param name="property">Optional, the property that the problem is related to.</param>
    /// <returns>A new problem.</returns>
    public static Problem InternalError(string? detail, string? typeId = null, string? property = null)
    {
        return new Problem
        {
            Category = ProblemCategory.InternalServerError,
            Detail = detail ?? R.InternalServerErrorMessage,
            TypeId = typeId,
            Property = property
        };
    }

    /// <summary>
    /// Create a new problem of InternalServerError category.
    /// </summary>
    /// <param name="exception">The exception that occurred.</param>
    /// <returns>A new problem.</returns>
    public static Problem InternalError(Exception exception) => InternalError(exception, ExceptionOptions);

    /// <summary>
    /// Create a new problem of InternalServerError category.
    /// </summary>
    /// <param name="exception">The exception that occurred.</param>
    /// <param name="options">The options to customize the creation of problems from exceptions.</param>
    /// <returns>A new problem.</returns>
    public static Problem InternalError(Exception exception, ExceptionOptions options)
    {
        if (ExceptionHandler?.TryHandle(exception, out var problem) is true)
            return problem;

        var detail = options.UseExceptionMessageAsDetail 
            ? exception.Message 
            : options.DefaultExceptionMessage;
        
        var property = exception is ArgumentException argumentException 
            ? argumentException.ParamName 
            : null;

        problem = new Problem
        {
            Category = ProblemCategory.InternalServerError,
            Detail = detail,
            Property = property
        };

        if (options.IncludeExceptionTypeName)
            problem.With("exception", exception.GetType().FullName);

        if (options.IncludeStackTrace)
            problem.With("stack_trace", exception.StackTrace);
        
        return problem;
    }

    /// <summary>
    /// Create a new problem of Custom category.
    /// </summary>
    /// <param name="detail">The detail of the problem, a description message for users.</param>
    /// <param name="typeId">Required, the type id of the problem, related to the problem details type.</param>
    /// <param name="property">Optional, the property that the problem is related to.</param>
    /// <returns>A new problem.</returns>
    /// <exception cref="ArgumentException">
    ///     Callers must provide a type id for custom problems.
    ///     <br/>
    ///     The type id must be a valid URI part.
    /// </exception>
    public static Problem Custom(string detail, string typeId, string? property = null)
    {
        if (string.IsNullOrWhiteSpace(typeId))
            throw new ArgumentException(R.TypeIdIsRequired, nameof(typeId));

        // type must be a valid URI part
        if (!Uri.IsWellFormedUriString(typeId, UriKind.Relative))
            throw new ArgumentException(R.TypeIdMustBeValidUri, nameof(typeId));
        
        return new Problem
        {
            Category = ProblemCategory.CustomProblem,
            Detail = detail,
            TypeId = typeId,
            Property = property
        };
    }

    /// <summary>
    /// Create a builder for a problem.
    /// </summary>
    /// <param name="details"></param>
    /// <returns></returns>
    public static DetailBuilder Detail(string details) => new() { Details = details };

    #endregion

    #region ICollection Implementation

    private Problem? firstProblem;
    private Node? head;
    private Node? tail;

    /// <summary>
    /// Get the problem at the specified index.
    /// </summary>
    /// <param name="index">The index of the problem.</param>
    /// <returns>A problem at the specified index.</returns>
    /// <exception cref="IndexOutOfRangeException">
    ///     Check if the index is out of range.
    /// </exception>
    public Problem this[int index]
    {
        get
        {
            #pragma warning disable S112

            if (index < 0 || index >= Count)
                throw new IndexOutOfRangeException();

            if (index == 0)
                return firstProblem!;

            return head![index - 1];
        }
    }

    /// <inheritdoc />
    public IEnumerator<Problem> GetEnumerator()
    {
        if (firstProblem is not null)
        {
            yield return firstProblem;
        }

        if (head is null)
            yield break;
        
        var current = head;
        while (current is not null)
        {
            yield return current.Problem;
            current = current.Next;
        }
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <inheritdoc />
    public void Add(Problem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        
        Count++;
        
        if (firstProblem is null)
        {
            firstProblem = item;
            return;
        }
        
        if (head is null)
        {
            head = new(item);
            tail = head;
        }
        else
        {
            tail = tail!.MakeNext(item);
        }
    }

    /// <summary>
    /// Add a range of problems to the collection.
    /// </summary>
    /// <param name="problems">A collection of problems to add.</param>
    public void AddRange(IEnumerable<Problem> problems)
    {
        ArgumentNullException.ThrowIfNull(problems);
        
        foreach (var problem in problems)
        {
            Add(problem);
        }
    }

    /// <summary>
    /// Not supported.
    /// </summary>
    /// <exception cref="NotSupportedException"></exception>
    public void Clear()
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public bool Contains(Problem item)
    {
        return Equals(firstProblem, item) || head is not null && head.Contains(item);
    }

    /// <inheritdoc />
    public void CopyTo(Problem[] array, int arrayIndex)
    {
        if (Count == 0)
            return;
        
        if (arrayIndex + Count > array.Length)
            throw new ArgumentException(R.CopyToArgumentErrorMessage, nameof(array));
        
        array[arrayIndex] = firstProblem!;
        head?.CopyTo(array, arrayIndex + 1);
    }

    /// <summary>
    /// Not supported.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public bool Remove(Problem item)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public int Count { get; private set; }
    
    /// <summary>
    /// False, the collection is not read-only.
    /// </summary>
    public bool IsReadOnly => false;
    
    private sealed class Node
    {
        public Problem Problem { get; }
        public Node? Next { get; private set; }
        
        public Node(Problem problem)
        {
            Problem = problem;
        }
        
        public Node MakeNext(Problem problem)
        {
            Next = new(problem);
            return Next;
        }
        
        public bool Contains(Problem problem)
        {
            return Equals(Problem, problem) || Next is not null && Next.Contains(problem);
        }

        public void CopyTo(Problem[] array, int arrayIndex)
        {
            array[arrayIndex] = Problem;
            Next?.CopyTo(array, arrayIndex + 1);
        }

        public Problem this[int index]
        {
            get
            {
                if (index == 0)
                    return Problem;

                return Next![index - 1];
            }
        }
    }

    #endregion

    /// <summary>
    /// Creates an <see cref="InvalidOperationException"/> from the collection of problems.
    /// The exception message will contain the details of all problems in the collection.
    /// </summary>
    /// <returns>A new instance of <see cref="InvalidOperationException"/>.</returns>
    public InvalidOperationException ToException()
    {
        return new InvalidOperationException(string.Join("\n", this.Select(p => p.ToString())));
    }

    /// <summary>
    /// Creates an <see cref="InvalidOperationException"/> from the collection of problems.
    /// The exception message will be formatted using the provided pattern,
    /// where the first argument is the details of all problems in the collection.
    /// </summary>
    /// <remarks>
    ///     The message pattern should contain a single placeholder (e.g., <c>"{0}"</c>, <c>"Errors: {0}"</c>),
    /// </remarks>
    /// <param name="messagePattern">The message pattern to format the exception message.</param>
    /// <param name="separator">The separator to use between problem details in the message, optional, defaults to newline character.</param>
    /// <returns>A new instance of <see cref="InvalidOperationException"/>.</returns>
    public InvalidOperationException ToException(string messagePattern, string separator = "\n")
    {
        var message = string.Format(messagePattern, string.Join(separator, this.Select(p => p.ToString())));
        return new InvalidOperationException(message);
    }

    /// <summary>
    /// Convert the collection of problems to a <see cref="Result"/> instance.
    /// </summary>
    /// <returns></returns>
    public Result AsResult()
    {
        return new Result(this);
    }

    /// <summary>
    /// Convert the collection of problems to a <see cref="Result{TValue}"/> instance with the specified value.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public Result<TValue> AsResult<TValue>(TValue value)
    {
        return new Result<TValue>(this);
    }
}