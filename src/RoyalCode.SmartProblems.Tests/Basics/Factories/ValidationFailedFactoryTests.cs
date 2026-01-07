namespace RoyalCode.SmartProblems.Tests.Basics.Factories;

public class ValidationFailedFactoryTests
{
    [Fact]
    public void ValidationFailed_Factory_Must_Set_Category_Detail_Property_TypeId()
    {
        // Arrange
        var detail = "Validation failed";
        var property = "prop";
        var typeId = "type";

        // Act
        var problem = Problems.ValidationFailed(detail, property, typeId);

        // Assert
        Assert.Equal(ProblemCategory.ValidationFailed, problem.Category);
        Assert.Equal(detail, problem.Detail);
        Assert.Equal(property, problem.Property);
        Assert.Equal(typeId, problem.TypeId);
    }
}
