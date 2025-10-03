
using System.Reflection;

namespace RoyalCode.SmartProblems;

/// <summary>
/// Attribute to indicate that the method produces problems.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class ProduceProblemsAttribute : Attribute
{
    /// <summary>
    /// The status codes that the method produces for problems results.
    /// </summary>
    public int[]? StatusCodes { get; set; }

    /// <summary>
    /// The problem categories that the method produces for problems results.
    /// </summary>
    public ProblemCategory[]? Categories { get; set; }

    /// <summary>
    /// A related type that can be used to provide additional context for the problems produced by the method.
    /// </summary>
    public Type? RelatedType { get; set; }

    /// <summary>
    /// Define the categories of problems that the method produces for problems results.
    /// </summary>
    /// <param name="categories">The problem categories that the method produces for problems results.</param>
    public ProduceProblemsAttribute(params ProblemCategory[] categories)
    {
        Categories = categories;
    }

    /// <summary>
    /// Define the status codes that the method produces for problems results.
    /// </summary>
    /// <param name="statusCodes">The status codes that the method produces for problems results.</param>
    public ProduceProblemsAttribute(params int[] statusCodes)
    {
        StatusCodes = statusCodes;
    }

    /// <summary>
    /// Define the related type that can be used to provide additional context for the problems produced by the method.
    /// </summary>
    /// <param name="relatedType">The related type that can be used to provide additional context for the problems produced by the method.</param>
    public ProduceProblemsAttribute(Type relatedType)
    {
        RelatedType = relatedType;
    }

    /// <summary>
    /// Define the status codes and categories of problems that the method produces for problems results.
    /// </summary>
    /// <param name="statusCodes">The status codes that the method produces for problems results.</param>
    /// <param name="categories">The problem categories that the method produces for problems results.</param>
    public ProduceProblemsAttribute(int[] statusCodes, ProblemCategory[] categories)
    {
        StatusCodes = statusCodes;
        Categories = categories;
    }

    /// <summary>
    /// Define the status codes and categories of problems that the method produces for problems results.
    /// </summary>
    /// <param name="relatedType">The related type that can be used to provide additional context for the problems produced by the method.</param>
    /// <param name="categories">The problem categories that the method produces for problems results.</param>
    public ProduceProblemsAttribute(Type relatedType, ProblemCategory[] categories)
    {
        RelatedType = relatedType;
        Categories = categories;
    }

    /// <summary>
    /// Define the status codes and categories of problems that the method produces for problems results.
    /// </summary>
    /// <param name="relatedType">The related type that can be used to provide additional context for the problems produced by the method.</param>
    /// <param name="statusCodes">The status codes that the method produces for problems results.</param>
    /// <param name="categories">The problem categories that the method produces for problems results.</param>
    public ProduceProblemsAttribute(Type relatedType, int[] statusCodes, ProblemCategory[] categories)
    {
        RelatedType = relatedType;
        StatusCodes = statusCodes;
        Categories = categories;
    }

    /// <summary>
    /// Creates a new instance of <see cref="ProduceProblemsAttribute"/>.
    /// </summary>
    public ProduceProblemsAttribute() { }

    /// <summary>
    /// Obtains the status codes that this attribute produces for problems results.
    /// </summary>
    /// <returns>
    /// An <see cref="IEnumerable{Int32}"/> containing the status codes that this attribute produces for problems results.
    /// </returns>
    public IEnumerable<int> GetStatusCodes()
    {
        if (StatusCodes is not null)
            for (var i = 0; i < StatusCodes.Length; i++)
                yield return StatusCodes[i];

        if (Categories is not null)
            for (var i = 0; i < Categories.Length; i++)
                yield return Categories[i] switch
                {
                    ProblemCategory.NotFound => 404,
                    ProblemCategory.InvalidParameter => 400,
                    ProblemCategory.ValidationFailed => 422,
                    ProblemCategory.CustomProblem => 422,
                    ProblemCategory.InvalidState => 409,
                    ProblemCategory.InternalServerError => 500,
                    _ => 400
                };

        var attr = RelatedType?.GetCustomAttribute<ProduceProblemsAttribute>();
        if (attr != null)
            foreach (var code in attr.GetStatusCodes())
                yield return code;
    }
}
