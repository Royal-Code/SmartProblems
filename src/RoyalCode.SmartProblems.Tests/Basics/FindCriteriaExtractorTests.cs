using RoyalCode.SmartProblems.Entities;
using System.Linq.Expressions;

namespace RoyalCode.SmartProblems.Tests.Basics;

public class FindCriteriaExtractorTests
{
    [Fact]
    public void Extract_SingleEquality_ReturnsOneCriterion()
    {
        // Act
        var criteria = FindCriteriaExtractor.Extract<Target>(t => t.Name == "abc");

        // Assert
        var criterion = Assert.Single(criteria);
        Assert.Equal(nameof(Target.Name), criterion.PropertyName);
        Assert.Equal("abc", criterion.Value);
    }

    [Fact]
    public void Extract_SingleEquality_Inverted_ReturnsOneCriterion()
    {
        // Act
        var criteria = FindCriteriaExtractor.Extract<Target>(t => "abc" == t.Name);

        // Assert
        var criterion = Assert.Single(criteria);
        Assert.Equal(nameof(Target.Name), criterion.PropertyName);
        Assert.Equal("abc", criterion.Value);
    }

    [Fact]
    public void Extract_ComposedEqualities_PreservesDeclarationOrder()
    {
        // Act
        var criteria = FindCriteriaExtractor.Extract<Target>(t => t.Name == "abc" && t.Code == 7 && t.Active == true);

        // Assert
        Assert.Equal(3, criteria.Length);
        Assert.Equal(nameof(Target.Name), criteria[0].PropertyName);
        Assert.Equal("abc", criteria[0].Value);
        Assert.Equal(nameof(Target.Code), criteria[1].PropertyName);
        Assert.Equal(7, criteria[1].Value);
        Assert.Equal(nameof(Target.Active), criteria[2].PropertyName);
        Assert.Equal(true, criteria[2].Value);
    }

    [Fact]
    public void Extract_ClosureMemberChain_EvaluatesValue()
    {
        // Arrange
        var request = new Request { Email = "who@mail.com" };

        // Act
        var criteria = FindCriteriaExtractor.Extract<Target>(t => t.Name == request.Email);

        // Assert
        var criterion = Assert.Single(criteria);
        Assert.Equal("who@mail.com", criterion.Value);
    }

    [Fact]
    public void Extract_NullValue_ReturnsCriterionWithNull()
    {
        // Act
        var criteria = FindCriteriaExtractor.Extract<Target>(t => t.Optional == null);

        // Assert
        var criterion = Assert.Single(criteria);
        Assert.Equal(nameof(Target.Optional), criterion.PropertyName);
        Assert.Null(criterion.Value);
    }

    [Theory]
    [MemberData(nameof(DegradingFilters))]
    public void Extract_UnsupportedConstruct_DegradesToEmpty_AllOrNothing(Expression<Func<Target, bool>> filter)
    {
        // Act
        var criteria = FindCriteriaExtractor.Extract(filter);

        // Assert
        Assert.Empty(criteria);
    }

    public static TheoryData<Expression<Func<Target, bool>>> DegradingFilters() => new()
    {
        // operator other than ==
        { t => t.Code != 7 },
        { t => t.Code > 7 },
        // OR anywhere degrades everything, including the valid left side
        { t => t.Name == "abc" || t.Code == 7 },
        // OR in one leaf of an AND degrades the whole tree
        { t => t.Name == "abc" && (t.Code == 7 || t.Active) },
        // both sides are members of the parameter
        { t => t.Name == t.Optional },
        // deep member chain is not a direct member
        { t => t.Child!.Name == "abc" },
        // method call on the value side
        { t => t.Name == "abc".ToUpperInvariant() },
        // bare boolean member and negation
        { t => t.Active },
        { t => !t.Active },
    };

    [Fact]
    public void Extract_ThrowingCapturedGetter_DegradesToEmpty_WithoutThrowing()
    {
        // Arrange
        var request = new Request { Email = "x" };

        // Act
        var criteria = FindCriteriaExtractor.Extract<Target>(t => t.Name == request.Throwing);

        // Assert
        Assert.Empty(criteria);
    }

    public class Target
    {
        public string Name { get; set; } = string.Empty;
        public int Code { get; set; }
        public bool Active { get; set; }
        public string? Optional { get; set; }
        public Target? Child { get; set; }
    }

    public class Request
    {
        public string Email { get; set; } = string.Empty;
        public string Throwing => throw new InvalidOperationException("side effect");
    }
}
