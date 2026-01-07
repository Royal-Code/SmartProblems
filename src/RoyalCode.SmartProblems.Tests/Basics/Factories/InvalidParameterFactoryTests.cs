namespace RoyalCode.SmartProblems.Tests.Basics.Factories;

public class InvalidParameterFactoryTests
{
    [Fact]
    public void InvalidParameter_Factory_Must_Set_Category_Detail_Property_TypeId()
    {
        // Arrange
        var detail = "Invalid parameter";
        var property = "prop";
        var typeId = "type";

        // Act
        var problem = Problems.InvalidParameter(detail, property, typeId);

        // Assert
        Assert.Equal(ProblemCategory.InvalidParameter, problem.Category);
        Assert.Equal(detail, problem.Detail);
        Assert.Equal(property, problem.Property);
        Assert.Equal(typeId, problem.TypeId);
    }
}
