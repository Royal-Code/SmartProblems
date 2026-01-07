namespace RoyalCode.SmartProblems.Tests.Basics.Factories;

public class InvalidStateFactoryTests
{
    [Fact]
    public void InvalidState_Factory_Must_Set_Category_Detail_Property_TypeId()
    {
        // Arrange
        var detail = "Invalid state";
        var property = "prop";
        var typeId = "type";

        // Act
        var problem = Problems.InvalidState(detail, property, typeId);

        // Assert
        Assert.Equal(ProblemCategory.InvalidState, problem.Category);
        Assert.Equal(detail, problem.Detail);
        Assert.Equal(property, problem.Property);
        Assert.Equal(typeId, problem.TypeId);
    }
}
