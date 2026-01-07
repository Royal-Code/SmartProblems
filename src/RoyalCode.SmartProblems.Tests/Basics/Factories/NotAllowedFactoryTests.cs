namespace RoyalCode.SmartProblems.Tests.Basics.Factories;

public class NotAllowedFactoryTests
{
    [Fact]
    public void NotAllowed_Factory_Must_Set_Category_Detail_Property_TypeId()
    {
        // Arrange
        var detail = "Not allowed";
        var property = "prop";
        var typeId = "type";

        // Act
        var problem = Problems.NotAllowed(detail, property, typeId);

        // Assert
        Assert.Equal(ProblemCategory.NotAllowed, problem.Category);
        Assert.Equal(detail, problem.Detail);
        Assert.Equal(property, problem.Property);
        Assert.Equal(typeId, problem.TypeId);
    }
}
