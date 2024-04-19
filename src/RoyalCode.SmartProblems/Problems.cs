using System.Collections;

namespace RoyalCode.SmartProblems;

public sealed class Problems : ICollection<Problem>
{
    private Problem? firstProblem;
    private Node? head;
    private Node? tail;
    
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

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

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

    public void Clear()
    {
        throw new NotSupportedException();
    }

    public bool Contains(Problem item)
    {
        return Equals(firstProblem, item) || head is not null && head.Contains(item);
    }

    public void CopyTo(Problem[] array, int arrayIndex)
    {
        if (Count == 0)
            return;
        
        if (arrayIndex + Count > array.Length)
            throw new ArgumentException(
                "The number of elements is greater than the available space in the array."
                , nameof(array));
        
        array[arrayIndex] = firstProblem!;
        head?.CopyTo(array, arrayIndex + 1);
    }

    public bool Remove(Problem item)
    {
        throw new NotSupportedException();
    }

    public int Count { get; private set; }
    
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
    }
}