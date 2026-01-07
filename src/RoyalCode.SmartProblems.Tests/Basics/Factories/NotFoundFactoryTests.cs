namespace RoyalCode.SmartProblems.Tests.Basics.Factories;

public class NotFoundFactoryTests
{
    [Fact]
    public void NotFound_Factory_Must_Set_Category_Detail_Property_TypeId()
    {
        // Arrange
        var detail = "Not found";
        var property = "prop";
        var typeId = "type";

        // Act
        var problem = Problems.NotFound(detail, property, typeId);

        // Assert
        Assert.Equal(ProblemCategory.NotFound, problem.Category);
        Assert.Equal(detail, problem.Detail);
        Assert.Equal(property, problem.Property);
        Assert.Equal(typeId, problem.TypeId);
    }
}
