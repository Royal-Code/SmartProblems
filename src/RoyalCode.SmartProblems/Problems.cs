using System.Collections;
using System.Runtime.CompilerServices;

namespace RoyalCode.SmartProblems;

/// <summary>
/// A collection of problems that occurred in the system.
/// </summary>
public sealed class Problems : ICollection<Problem>
{
    #region Global Property

    /// <summary>
    /// <para>
    ///     A exception handler to convert exceptions to problems.
    /// </para>
    /// <para>
    ///     Is used in the <see cref="InternalError(Exception)"/> method.
    /// </para>
    /// <para>
    ///     Usefull to customize the creation of problems from exceptions.
    /// </para>
    /// </summary>
    public static IExceptionHandler? ExceptionHandler { get; set; }

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
    /// Create a new problem of InternalServerError category.
    /// </summary>
    /// <param name="detail">The detail of the problem, a description message for users.</param>
    /// <param name="typeId">Optional, the type id of the problem, related to the problem details type.</param>
    /// <returns>A new problem.</returns>
    public static Problem InternalError(string? detail, string? typeId = null)
    {
        return new Problem
        {
            Category = ProblemCategory.InternalServerError,
            Detail = detail ?? R.InternalServerErrorMessage,
            TypeId = typeId
        };
    }

    /// <summary>
    /// Create a new problem of InternalServerError category.
    /// </summary>
    /// <param name="exception">The exception that occurred.</param>
    /// <returns>A new problem.</returns>
    public static Problem InternalError(Exception exception)
    {
        if (ExceptionHandler?.TryHandle(exception, out var problem) is true)
            return problem;

        var property = exception is ArgumentException argumentException 
            ? argumentException.ParamName 
            : null;

        return new Problem
        {
            Category = ProblemCategory.InternalServerError,
            Detail = exception.Message,
            TypeId = exception.GetType().Name,
            Property = property
        };
    }

    /// <summary>
    /// Create a new problem of Custom category.
    /// </summary>
    /// <param name="detail">The detail of the problem, a description message for users.</param>
    /// <param name="typeId">Required, the type id of the problem, related to the problem details type.</param>
    /// <returns>A new problem.</returns>
    /// <exception cref="ArgumentException">
    ///     Callers must provide a type id for custom problems.
    /// </exception>
    public static Problem Custom(string detail, string typeId)
    {
        if (string.IsNullOrWhiteSpace(typeId))
            throw new ArgumentException(R.TypeIdIsRequired, nameof(typeId));

        return new Problem
        {
            Category = ProblemCategory.CustomProblem,
            Detail = detail,
            TypeId = typeId
        };
    }

    #endregion

    #region implicit operators

    /// <summary>
    /// Convert a problem to a collection of problems.
    /// </summary>
    /// <param name="problem"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Problems(Problem problem) => [problem];

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
}