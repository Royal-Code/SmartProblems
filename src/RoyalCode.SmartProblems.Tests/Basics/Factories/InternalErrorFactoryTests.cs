namespace RoyalCode.SmartProblems.Tests.Basics.Factories;

public class InternalErrorFactoryTests
{
    [Fact]
    public void InternalError_Factory_Must_Set_Category_Detail_Property_TypeId()
    {
        // Arrange
        var detail = "Internal error";
        var property = "prop";
        var typeId = "type";

        // Act
        var problem = Problems.InternalError(detail, typeId, property);

        // Assert
        Assert.Equal(ProblemCategory.InternalServerError, problem.Category);
        Assert.Equal(detail, problem.Detail);
        Assert.Equal(property, problem.Property);
        Assert.Equal(typeId, problem.TypeId);
    }

    [Fact]
    public void InternalError_NullException_Must_UseDefaultMessage()
    {
        // Arrange
        // handled by default options

        // Act
        var problem = Problems.InternalError();

        // Assert
        Assert.Equal(ProblemCategory.InternalServerError, problem.Category);
        Assert.Equal(Problems.ExceptionOptions.DefaultExceptionMessage, problem.Detail);
        Assert.Null(problem.Property);
    }
}
