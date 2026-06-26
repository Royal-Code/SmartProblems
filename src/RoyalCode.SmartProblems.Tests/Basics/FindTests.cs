using RoyalCode.SmartProblems.Entities;
using RoyalCode.SmartProblems;
using RoyalCode.SmartProblems.Tests.UseCases;

namespace RoyalCode.SmartProblems.Tests.Basics;

public class FindTests
{
    [Fact]
    public void FindResult_Implicit_Entity()
    {
        // Arrange
        Foo foo = new() { Value = 1 };

        // Act
        FindResult<Foo> result = foo;
        var notFound = result.NotFound(out var problem);

        // Assert
        Assert.True(result.Found);
        Assert.False(notFound);
        Assert.Null(problem);
        Assert.NotNull(result.Entity);
        Assert.Equal(1, result.Entity.Value);
    }

    [Fact]
    public void FindResult_Implicit_Problems()
    {
        // Arrange
        Problems problems = Problems.NotFound("Error 1") + Problems.NotFound("Error 2");

        // Act
        FindResult<Foo> result = problems;
        var notFound = result.NotFound(out var problem);

        // Assert
        Assert.False(result.Found);
        Assert.True(notFound);
        Assert.NotNull(problem);
        Assert.Equal(string.Format("The record for '{0}' was not found", nameof(Foo)), problem.Detail);
        Assert.NotNull(problem.Extensions);
        Assert.True(problem.Extensions.ContainsKey("problems"));

        var problemsList = problem.Extensions["problems"] as Problems;
        Assert.NotNull(problemsList);
        Assert.Equal(2, problemsList.Count);
    }

    [Fact]
    public void FindResult_Implicit_Problem()
    {
        // Arrange
        Problem problems = Problems.NotFound("Error 1");

        // Act
        FindResult<Foo> result = problems;
        var notFound = result.NotFound(out var problem);

        // Assert
        Assert.False(result.Found);
        Assert.True(notFound);
        Assert.NotNull(problem);
        Assert.Equal("Error 1", problem.Detail);
    }

    [Fact]
    public void FindResult_HasInvalidParameter()
    {
        // Arrange
        FindResult<Foo> result = new();

        // Act
        var notFound = result.HasInvalidParameter(out var problem, "property");

        // Assert
        Assert.False(result.Found);
        Assert.True(notFound);
        Assert.NotNull(problem);
        Assert.Equal("The record for 'Foo' was not found", problem.Detail);
        Assert.Equal(ProblemCategory.InvalidParameter, problem.Category);
        Assert.Equal("property", problem.Property);
    }

    [Fact]
    public void FindResult_Implicit_Result_Entity()
    {
        // Arrange
        Result<Foo> result = new Foo { Value = 1 };

        // Act
        FindResult<Foo> findResult = result;
        var notFound = findResult.NotFound(out var problem);

        // Assert
        Assert.True(findResult.Found);
        Assert.False(notFound);
        Assert.Null(problem);
        Assert.NotNull(findResult.Entity);
        Assert.Equal(1, findResult.Entity.Value);
    }

    [Fact]
    public void FindResult_Implicit_Result_Failure()
    {
        // Arrange
        Result<Foo> result = Problems.NotFound("Error 1") + Problems.NotFound("Error 2");

        // Act
        FindResult<Foo> findResult = result;
        var notFound = findResult.NotFound(out var problem);

        // Assert
        Assert.False(findResult.Found);
        Assert.True(notFound);
        Assert.NotNull(problem);
        Assert.Equal(string.Format("The record for '{0}' was not found", nameof(Foo)), problem.Detail);
        Assert.NotNull(problem.Extensions);
        Assert.True(problem.Extensions.ContainsKey("problems"));

        var problemsList = problem.Extensions["problems"] as Problems;
        Assert.NotNull(problemsList);
        Assert.Equal(2, problemsList.Count);
    }

    [Fact]
    public void FindResult_Implicit_ToResult_Found()
    {
        // Arrange
        FindResult<Foo> findResult = new Foo { Value = 1 };

        // Act
        Result<Foo> result = findResult;
        var hasProblems = result.HasProblems(out var problems);

        // Assert
        Assert.True(findResult.Found);
        Assert.False(hasProblems);
        Assert.Null(problems);
    }

    [Fact]
    public void FindResult_Implicit_ToResult_NotFound()
    {
        // Arrange
        FindResult<Foo> findResult = FindResult<Foo>.Problem("Value", "value", 1);

        // Act
        Result<Foo> result = findResult;
        var hasProblems = result.HasProblems(out var problems);

        // Assert
        Assert.False(findResult.Found);
        Assert.True(hasProblems);
        Assert.NotNull(problems);
    }

    [Fact]
    public void FindResult_ById_Implicit_ToResult_Found()
    {
        // Arrange
        FindResult<Foo, int> findResult = new(new Foo { Value = 1 }, 1);

        // Act
        Result<Foo> result = findResult;
        var hasProblems = result.HasProblems(out var problems);

        // Assert
        Assert.True(findResult.Found);
        Assert.NotNull(findResult.Entity);
        Assert.False(hasProblems);
        Assert.Null(problems);
    }

    [Fact]
    public void FindResult_ById_Implicit_ToResult_NotFound()
    {
        // Arrange
        FindResult<Foo, int> findResult = new(1);

        // Act
        Result<Foo> result = findResult;
        var hasProblems = result.HasProblems(out var problems);

        // Assert
        Assert.False(findResult.Found);
        Assert.Null(findResult.Entity);
        Assert.True(hasProblems);
        Assert.NotNull(problems);
    }

    [Fact]
    public void FindResult_New_Problem()
    {
        // act
        var findResult = new FindResult<Foo>();
        var notFound = findResult.NotFound(out var problem);

        // assert
        Assert.False(findResult.Found);
        Assert.True(notFound);
        Assert.NotNull(problem);
        Assert.Equal("The record for 'Foo' was not found", problem.Detail);
        Assert.Equal(ProblemCategory.NotFound, problem.Category);
    }

    [Fact]
    public void FindResult_New_Problem_InvalidParamter()
    {
        // act
        var findResult = new FindResult<Foo>();
        var notFound = findResult.HasInvalidParameter(out var problem);

        // assert
        Assert.False(findResult.Found);
        Assert.True(notFound);
        Assert.NotNull(problem);
        Assert.Equal("The record for 'Foo' was not found", problem.Detail);
        Assert.Equal(ProblemCategory.InvalidParameter, problem.Category);
    }

    [Fact]
    public void FindResult_New_Problem_InvalidParamter_WithParamterName()
    {
        // act
        var findResult = new FindResult<Foo>();
        var notFound = findResult.HasInvalidParameter(out var problem, "SomeParameterName");

        // assert
        Assert.False(findResult.Found);
        Assert.True(notFound);
        Assert.NotNull(problem);
        Assert.Equal("The record for 'Foo' was not found", problem.Detail);
        Assert.Equal(ProblemCategory.InvalidParameter, problem.Category);
        Assert.Equal("SomeParameterName", problem.Property);
    }

    [Fact]
    public void FindResult_Factory_Problem_ByName()
    {
        // act
        var findResult = FindResult<Foo>.Problem("Value", "value", 1);
        var notFound = findResult.NotFound(out var problem);

        // assert
        Assert.False(findResult.Found);
        Assert.True(notFound);
        Assert.NotNull(problem);
        Assert.Equal("The record of 'Foo' with Value '1' was not found", problem.Detail);
    }

    [Fact]
    public void FindResult_Factory_Problem_ById()
    {
        // act
        var findResult = new FindResult<Foo, int>(1);
        var notFound = findResult.NotFound(out var problem);

        // assert
        Assert.False(findResult.Found);
        Assert.True(notFound);
        Assert.NotNull(problem);
        Assert.Equal("The record of 'Foo' with id '1' was not found", problem.Detail);
        Assert.Equal(ProblemCategory.NotFound, problem.Category);
    }

    [Fact]
    public void FindResult_Factory_Problem_InvalidParamter_ById()
    {
        // act
        var findResult = new FindResult<Foo, int>(1);
        var notFound = findResult.HasInvalidParameter(out var problem);

        // assert
        Assert.False(findResult.Found);
        Assert.True(notFound);
        Assert.NotNull(problem);
        Assert.Equal("The record of 'Foo' with id '1' was not found", problem.Detail);
        Assert.Equal(ProblemCategory.InvalidParameter, problem.Category);
    }

    [Fact]
    public void FindResult_Factory_Problem_InvalidParamter_WithParamterName_ById()
    {
        // act
        var findResult = new FindResult<Foo, int>(1);
        var notFound = findResult.HasInvalidParameter(out var problem, "Id");

        // assert
        Assert.False(findResult.Found);
        Assert.True(notFound);
        Assert.NotNull(problem);
        Assert.Equal("The record of 'Foo' with id '1' was not found", problem.Detail);
        Assert.Equal(ProblemCategory.InvalidParameter, problem.Category);
        Assert.Equal("Id", problem.Property);
    }

    [Fact]
    public void FindResult_Collect_Found()
    {
        // Arrange
        FindResult<Foo> result = new Foo { Value = 1 };
        Bar bar = new() { Value = 0 };

        // Act
        result.Collect(f => bar.Value = f.Value);

        // Assert
        Assert.True(result.Found);
        Assert.Equal(1, result.Entity.Value);
        Assert.Equal(1, bar.Value);
    }

    [Fact]
    public void FindResult_Collect_NotFound()
    {
        // Arrange
        FindResult<Foo> result = FindResult<Foo>.Problem("Value", "value", 1);
        Bar bar = new() { Value = 0 };

        // Act
        result.Collect(f => bar.Value = f.Value);

        // Assert
        Assert.False(result.Found);
        Assert.Equal(0, bar.Value);
    }

    [Fact]
    public async Task FindResult_CollectAsync_Found()
    {
        // Arrange
        FindResult<Foo> result = new Foo { Value = 1 };
        Bar bar = new() { Value = 0 };

        // Act
        await result.CollectAsync(f => Task.Run(() => bar.Value = f.Value));

        // Assert
        Assert.True(result.Found);
        Assert.Equal(1, result.Entity.Value);
        Assert.Equal(1, bar.Value);
    }

    [Fact]
    public async Task FindResult_CollectAsync_NotFound()
    {
        // Arrange
        FindResult<Foo> result = FindResult<Foo>.Problem("Value", "value", 1);
        Bar bar = new() { Value = 0 };

        // Act
        await result.CollectAsync(f => Task.Run(() => bar.Value = f.Value));

        // Assert
        Assert.False(result.Found);
        Assert.Equal(0, bar.Value);
    }

    [Fact]
    public void FindResult_ById_Collect_Found()
    {
        // Arrange
        FindResult<Foo, int> result = new(new Foo { Value = 1 }, 1);
        Bar bar = new() { Value = 0 };

        // Act
        result.Collect(f => bar.Value = f.Value);

        // Assert
        Assert.True(result.Found);
        Assert.Equal(1, result.Entity.Value);
        Assert.Equal(1, bar.Value);
    }

    [Fact]
    public void FindResult_ById_Collect_NotFound()
    {
        // Arrange
        FindResult<Foo, int> result = new(1);
        Bar bar = new() { Value = 0 };

        // Act
        result.Collect(f => bar.Value = f.Value);

        // Assert
        Assert.False(result.Found);
        Assert.Equal(0, bar.Value);
    }

    [Fact]
    public async Task FindResult_ById_CollectAsync_Found()
    {
        // Arrange
        FindResult<Foo, int> result = new(new Foo { Value = 1 }, 1);
        Bar bar = new() { Value = 0 };

        // Act
        await result.CollectAsync(f => Task.Run(() => bar.Value = f.Value));

        // Assert
        Assert.True(result.Found);
        Assert.Equal(1, result.Entity.Value);
        Assert.Equal(1, bar.Value);
    }

    [Fact]
    public async Task FindResult_ById_CollectAsync_NotFound()
    {
        // Arrange
        FindResult<Foo, int> result = new(1);
        Bar bar = new() { Value = 0 };

        // Act
        await result.CollectAsync(f => Task.Run(() => bar.Value = f.Value));

        // Assert
        Assert.False(result.Found);
        Assert.Equal(0, bar.Value);
    }

    [Fact]
    public void FindResult_Continue_Found()
    {
        // Arrange
        FindResult<Foo> result = new Foo { Value = 1 };
        Bar bar = new() { Value = 0 };

        // Act
        result.Continue(CreateContinue(bar));

        // Assert
        Assert.True(result.Found);
        Assert.Equal(1, result.Entity.Value);
        Assert.Equal(1, bar.Value);
    }

    [Fact]
    public void FindResult_Continue_Found_WithProblem()
    {
        // Arrange
        FindResult<Foo> findResult = new Foo { Value = 1 };
        Bar bar = new() { Value = 0 };

        // Act
        var result = findResult.Continue(CreateContinue(bar, Problems.InvalidParameter("Erro 1")));
        var hasProblems = result.HasProblems(out var problems);

        // Assert
        Assert.True(findResult.Found);
        Assert.Equal(1, findResult.Entity.Value);
        Assert.Equal(1, bar.Value);

        Assert.True(hasProblems);
        Assert.NotNull(problems);
        Assert.Single(problems);
        Assert.Equal("Erro 1", problems[0].Detail);
    }

    [Fact]
    public void FindResult_Continue_NotFound()
    {
        // Arrange
        FindResult<Foo> result = FindResult<Foo>.Problem("Value", "value", 1);
        Bar bar = new() { Value = 0 };

        // Act
        result.Continue(CreateContinue(bar));

        // Assert
        Assert.False(result.Found);
        Assert.Equal(0, bar.Value);
    }

    [Fact]
    public async Task FindResult_ContinueAsync_Found()
    {
        // Arrange
        FindResult<Foo> result = new Foo { Value = 1 };
        Bar bar = new() { Value = 0 };

        // Act
        await result.ContinueAsync(CreateContinueAsync(bar));

        // Assert
        Assert.True(result.Found);
        Assert.Equal(1, result.Entity.Value);
        Assert.Equal(1, bar.Value);
    }

    [Fact]
    public async Task FindResult_ContinueAsync_Found_WithProblem()
    {
        // Arrange
        FindResult<Foo> findResult = new Foo { Value = 1 };
        Bar bar = new() { Value = 0 };

        // Act
        var result = await findResult.ContinueAsync(CreateContinueAsync(bar, Problems.InvalidParameter("Erro 1")));
        var hasProblems = result.HasProblems(out var problems);

        // Assert
        Assert.True(findResult.Found);
        Assert.Equal(1, findResult.Entity.Value);
        Assert.Equal(1, bar.Value);
        Assert.True(hasProblems);
        Assert.NotNull(problems);
        Assert.Single(problems);
        Assert.Equal("Erro 1", problems[0].Detail);
    }

    [Fact]
    public async Task FindResult_ContinueAsync_NotFound()
    {
        // Arrange
        FindResult<Foo> result = FindResult<Foo>.Problem("Value", "value", 1);
        Bar bar = new() { Value = 0 };

        // Act
        await result.ContinueAsync(CreateContinueAsync(bar));

        // Assert
        Assert.False(result.Found);
        Assert.Equal(0, bar.Value);
    }

    [Fact]
    public void FindResult_ById_Continue_Found()
    {
        // Arrange
        FindResult<Foo, int> result = new(new Foo { Value = 1 }, 1);
        Bar bar = new() { Value = 0 };

        // Act
        result.Continue(CreateContinue(bar));

        // Assert
        Assert.True(result.Found);
        Assert.Equal(1, result.Entity.Value);
        Assert.Equal(1, bar.Value);
    }

    [Fact]
    public void FindResult_ById_Continue_Found_WithProblem()
    {
        // Arrange
        FindResult<Foo, int> findResult = new(new Foo { Value = 1 }, 1);
        Bar bar = new() { Value = 0 };

        // Act
        var result = findResult.Continue(CreateContinue(bar, Problems.InvalidParameter("Erro 1")));
        var hasProblems = result.HasProblems(out var problems);

        // Assert
        Assert.True(findResult.Found);
        Assert.Equal(1, findResult.Entity.Value);
        Assert.Equal(1, bar.Value);

        Assert.True(hasProblems);
        Assert.NotNull(problems);
        Assert.Single(problems);
        Assert.Equal("Erro 1", problems[0].Detail);
    }

    [Fact]
    public void FindResult_ById_Continue_NotFound()
    {
        // Arrange
        FindResult<Foo, int> result = new(1);
        Bar bar = new() { Value = 0 };

        // Act
        result.Continue(CreateContinue(bar));

        // Assert
        Assert.False(result.Found);
        Assert.Equal(0, bar.Value);
    }

    [Fact]
    public async Task FindResult_ById_ContinueAsync_Found()
    {
        // Arrange
        FindResult<Foo, int> result = new(new Foo { Value = 1 }, 1);
        Bar bar = new() { Value = 0 };

        // Act
        await result.ContinueAsync(CreateContinueAsync(bar));

        // Assert
        Assert.True(result.Found);
        Assert.Equal(1, result.Entity.Value);
        Assert.Equal(1, bar.Value);
    }

    [Fact]
    public async Task FindResult_ById_ContinueAsync_Found_WithProblem()
    {
        // Arrange
        FindResult<Foo, int> findResult = new(new Foo { Value = 1 }, 1);
        Bar bar = new() { Value = 0 };

        // Act
        var result = await findResult.ContinueAsync(CreateContinueAsync(bar, Problems.InvalidParameter("Erro 1")));
        var hasProblems = result.HasProblems(out var problems);

        // Assert
        Assert.True(findResult.Found);
        Assert.Equal(1, findResult.Entity.Value);
        Assert.Equal(1, bar.Value);
        Assert.True(hasProblems);
        Assert.NotNull(problems);
        Assert.Single(problems);
        Assert.Equal("Erro 1", problems[0].Detail);
    }

    [Fact]
    public async Task FindResult_ById_ContinueAsync_NotFound()
    {
        // Arrange
        FindResult<Foo, int> result = new(1);
        Bar bar = new() { Value = 0 };

        // Act
        await result.ContinueAsync(CreateContinueAsync(bar));

        // Assert
        Assert.False(result.Found);
        Assert.Equal(0, bar.Value);
    }

    [Fact]
    public void FindResult_Map_Found()
    {
        // Arrange
        FindResult<Foo> result = new Foo { Value = 1 };

        // Act
        var mappedResult = result.Map(f => new Bar { Value = f.Value});
        var hasProblems = mappedResult.HasProblemsOrGetValue(out var problems, out var bar);

        // Assert
        Assert.True(result.Found);
        Assert.Equal(1, result.Entity.Value);
        
        Assert.False(hasProblems);
        Assert.Null(problems);

        Assert.NotNull(bar);
        Assert.Equal(1, bar.Value);
    }

    [Fact]
    public void FindResult_Map_NotFound()
    {
        // Arrange
        FindResult<Foo> result = FindResult<Foo>.Problem("Value", "value", 1);

        // Act
        var mappedResult = result.Map(f => new Bar { Value = f.Value });
        var hasProblems = mappedResult.HasProblemsOrGetValue(out var problems, out var bar);

        // Assert
        Assert.False(result.Found);
        Assert.Null(bar);

        Assert.True(hasProblems);
        Assert.NotNull(problems);

        Assert.Null(bar);
    }

    [Fact]
    public async Task FindResult_MapAsync_Found()
    {
        // Arrange
        FindResult<Foo> result = new Foo { Value = 1 };

        // Act
        var mappedResult = await result.MapAsync(f => Task.Run(() => new Bar { Value = f.Value }));
        var hasProblems = mappedResult.HasProblemsOrGetValue(out var problems, out var bar);

        // Assert
        Assert.True(result.Found);
        Assert.Equal(1, result.Entity.Value);

        Assert.False(hasProblems);
        Assert.Null(problems);

        Assert.NotNull(bar);
        Assert.Equal(1, bar.Value);
    }

    [Fact]
    public async Task FindResult_MapAsync_NotFound()
    {
        // Arrange
        FindResult<Foo> result = FindResult<Foo>.Problem("Value", "value", 1);

        // Act
        var mappedResult = await result.MapAsync(f => Task.Run(() => new Bar { Value = f.Value }));
        var hasProblems = mappedResult.HasProblemsOrGetValue(out var problems, out var bar);

        // Assert
        Assert.False(result.Found);
        Assert.Null(bar);

        Assert.True(hasProblems);
        Assert.NotNull(problems);

        Assert.Null(bar);
    }

    [Fact]
    public void FindResult_Map_Found_WithResult()
    {
        // Arrange
        FindResult<Foo> result = new Foo { Value = 1 };

        // Act
        var mappedResult = result.Map(f => new Result<Bar>(new Bar { Value = f.Value }));
        var hasProblems = mappedResult.HasProblemsOrGetValue(out var problems, out var bar);

        // Assert
        Assert.True(result.Found);
        Assert.Equal(1, result.Entity.Value);

        Assert.False(hasProblems);
        Assert.Null(problems);

        Assert.NotNull(bar);
        Assert.Equal(1, bar.Value);
    }

    [Fact]
    public void FindResult_Map_NotFound_WithResult()
    {
        // Arrange
        FindResult<Foo> result = FindResult<Foo>.Problem("Value", "value", 1);

        // Act
        var mappedResult = result.Map(f => new Result<Bar>(new Bar { Value = f.Value }));
        var hasProblems = mappedResult.HasProblemsOrGetValue(out var problems, out var bar);

        // Assert
        Assert.False(result.Found);
        Assert.Null(bar);

        Assert.True(hasProblems);
        Assert.NotNull(problems);

        Assert.Null(bar);
    }

    [Fact]
    public async Task FindResult_MapAsync_Found_WithResult()
    {
        // Arrange
        FindResult<Foo> result = new Foo { Value = 1 };

        // Act
        var mappedResult = await result.MapAsync(f => Task.Run(() => new Result<Bar>(new Bar { Value = f.Value })));
        var hasProblems = mappedResult.HasProblemsOrGetValue(out var problems, out var bar);

        // Assert
        Assert.True(result.Found);
        Assert.Equal(1, result.Entity.Value);

        Assert.False(hasProblems);
        Assert.Null(problems);

        Assert.NotNull(bar);
        Assert.Equal(1, bar.Value);
    }

    [Fact]
    public async Task FindResult_MapAsync_NotFound_WithResult()
    {
        // Arrange
        FindResult<Foo> result = FindResult<Foo>.Problem("Value", "value", 1);

        // Act
        var mappedResult = await result.MapAsync(f => Task.Run(() => new Result<Bar>(new Bar { Value = f.Value })));
        var hasProblems = mappedResult.HasProblemsOrGetValue(out var problems, out var bar);

        // Assert
        Assert.False(result.Found);
        Assert.Null(bar);

        Assert.True(hasProblems);
        Assert.NotNull(problems);

        Assert.Null(bar);
    }

    [Fact]
    public void FindResult_ById_Map_Found()
    {
        // Arrange
        FindResult<Foo, int> result = new(new Foo { Value = 1 }, 1);

        // Act
        var mappedResult = result.Map(f => new Bar { Value = f.Value });
        var hasProblems = mappedResult.HasProblemsOrGetValue(out var problems, out var bar);

        // Assert
        Assert.True(result.Found);
        Assert.Equal(1, result.Entity.Value);

        Assert.False(hasProblems);
        Assert.Null(problems);

        Assert.NotNull(bar);
        Assert.Equal(1, bar.Value);
    }

    [Fact]
    public void FindResult_ById_Map_NotFound()
    {
        // Arrange
        FindResult<Foo, int> result = new(1);

        // Act
        var mappedResult = result.Map(f => new Bar { Value = f.Value });
        var hasProblems = mappedResult.HasProblemsOrGetValue(out var problems, out var bar);

        // Assert
        Assert.False(result.Found);
        Assert.Null(bar);

        Assert.True(hasProblems);
        Assert.NotNull(problems);

        Assert.Null(bar);
    }

    [Fact]
    public async Task FindResult_ById_MapAsync_Found()
    {
        // Arrange
        FindResult<Foo, int> result = new(new Foo { Value = 1 }, 1);

        // Act
        var mappedResult = await result.MapAsync(f => Task.Run(() => new Bar { Value = f.Value }));
        var hasProblems = mappedResult.HasProblemsOrGetValue(out var problems, out var bar);

        // Assert
        Assert.True(result.Found);
        Assert.Equal(1, result.Entity.Value);

        Assert.False(hasProblems);
        Assert.Null(problems);

        Assert.NotNull(bar);
        Assert.Equal(1, bar.Value);
    }

    [Fact]
    public async Task FindResult_ById_MapAsync_NotFound()
    {
        // Arrange
        FindResult<Foo, int> result = new(1);

        // Act
        var mappedResult = await result.MapAsync(f => Task.Run(() => new Bar { Value = f.Value }));
        var hasProblems = mappedResult.HasProblemsOrGetValue(out var problems, out var bar);

        // Assert
        Assert.False(result.Found);
        Assert.Null(bar);

        Assert.True(hasProblems);
        Assert.NotNull(problems);

        Assert.Null(bar);
    }

    [Fact]
    public void FindResult_ById_Map_Found_WithResult()
    {
        // Arrange
        FindResult<Foo, int> result = new(new Foo { Value = 1 }, 1);

        // Act
        var mappedResult = result.Map(f => new Result<Bar>(new Bar { Value = f.Value }));
        var hasProblems = mappedResult.HasProblemsOrGetValue(out var problems, out var bar);

        // Assert
        Assert.True(result.Found);
        Assert.Equal(1, result.Entity.Value);

        Assert.False(hasProblems);
        Assert.Null(problems);

        Assert.NotNull(bar);
        Assert.Equal(1, bar.Value);
    }

    [Fact]
    public void FindResult_ById_Map_NotFound_WithResult()
    {
        // Arrange
        FindResult<Foo, int> result = new(1);

        // Act
        var mappedResult = result.Map(f => new Result<Bar>(new Bar { Value = f.Value }));
        var hasProblems = mappedResult.HasProblemsOrGetValue(out var problems, out var bar);

        // Assert
        Assert.False(result.Found);
        Assert.Null(bar);

        Assert.True(hasProblems);
        Assert.NotNull(problems);

        Assert.Null(bar);
    }

    [Fact]
    public async Task FindResult_ById_MapAsync_Found_WithResult()
    {
        // Arrange
        FindResult<Foo, int> result = new(new Foo { Value = 1 }, 1);

        // Act
        var mappedResult = await result.MapAsync(f => Task.Run(() => new Result<Bar>(new Bar { Value = f.Value })));
        var hasProblems = mappedResult.HasProblemsOrGetValue(out var problems, out var bar);

        // Assert
        Assert.True(result.Found);
        Assert.Equal(1, result.Entity.Value);

        Assert.False(hasProblems);
        Assert.Null(problems);

        Assert.NotNull(bar);
        Assert.Equal(1, bar.Value);
    }

    [Fact]
    public async Task FindResult_ById_MapAsync_NotFound_WithResult()
    {
        // Arrange
        FindResult<Foo, int> result = new(1);

        // Act
        var mappedResult = await result.MapAsync(f => Task.Run(() => new Result<Bar>(new Bar { Value = f.Value })));
        var hasProblems = mappedResult.HasProblemsOrGetValue(out var problems, out var bar);

        // Assert
        Assert.False(result.Found);
        Assert.Null(bar);

        Assert.True(hasProblems);
        Assert.NotNull(problems);

        Assert.Null(bar);
    }

    [Fact]
    public async Task FindResult_Async_WithTParam_UsesCancellationToken()
    {
        // Arrange
        FindResult<Foo> findResult = new Foo { Value = 10 };
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        CancellationToken collectCt = default;
        CancellationToken collectNamedCt = default;
        CancellationToken mapCt = default;
        CancellationToken mapNamedCt = default;

        // Act
        var collectResult = await findResult.CollectAsync(3, token, (foo, value, ct) =>
        {
            collectCt = ct;
            return Task.CompletedTask;
        });

        var collectNamedResult = await findResult.CollectAsync("foo", 4, token, (foo, value, ct) =>
        {
            collectNamedCt = ct;
            return Task.CompletedTask;
        });

        var mapResult = await findResult.MapAsync<Bar, int>(5, token, (foo, value, ct) =>
        {
            mapCt = ct;
            return Task.FromResult(new Bar { Value = foo.Value + value });
        });

        var mapNamedResult = await findResult.MapAsync<Bar, int>("foo", 6, token, (foo, value, ct) =>
        {
            mapNamedCt = ct;
            return Task.FromResult(new Bar { Value = foo.Value + value });
        });

        // Assert
        Assert.True(collectResult.IsSuccess);
        Assert.True(collectNamedResult.IsSuccess);
        Assert.Equal(token, collectCt);
        Assert.Equal(token, collectNamedCt);

        Assert.True(mapResult.HasValue(out var bar));
        Assert.Equal(15, bar.Value);
        Assert.True(mapNamedResult.HasValue(out var namedBar));
        Assert.Equal(16, namedBar.Value);
        Assert.Equal(token, mapCt);
        Assert.Equal(token, mapNamedCt);
    }

    [Fact]
    public async Task FindResult_ById_Async_WithTParam_UsesCancellationToken()
    {
        // Arrange
        FindResult<Foo, int> findResult = new(new Foo { Value = 20 }, 7);
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        CancellationToken collectCt = default;
        CancellationToken collectNamedCt = default;
        CancellationToken mapCt = default;
        CancellationToken mapNamedCt = default;

        // Act
        var collectResult = await findResult.CollectAsync(8, token, (foo, value, ct) =>
        {
            collectCt = ct;
            return Task.CompletedTask;
        });

        var collectNamedResult = await findResult.CollectAsync("id", 9, token, (foo, value, ct) =>
        {
            collectNamedCt = ct;
            return Task.CompletedTask;
        });

        var mapResult = await findResult.MapAsync<Bar, int>(10, token, (foo, value, ct) =>
        {
            mapCt = ct;
            return Task.FromResult(new Bar { Value = foo.Value + value });
        });

        var mapNamedResult = await findResult.MapAsync<Bar, int>("id", 11, token, (foo, value, ct) =>
        {
            mapNamedCt = ct;
            return Task.FromResult(new Bar { Value = foo.Value + value });
        });

        // Assert
        Assert.True(collectResult.IsSuccess);
        Assert.True(collectNamedResult.IsSuccess);
        Assert.Equal(token, collectCt);
        Assert.Equal(token, collectNamedCt);

        Assert.True(mapResult.HasValue(out var bar));
        Assert.Equal(30, bar.Value);
        Assert.True(mapNamedResult.HasValue(out var namedBar));
        Assert.Equal(31, namedBar.Value);
        Assert.Equal(token, mapCt);
        Assert.Equal(token, mapNamedCt);
    }

    [Fact]
    public async Task FindResult_Extensions_Async_WithTParam_UseCancellationToken()
    {
        // Arrange
        Task<FindResult<Foo>> task = Task.FromResult<FindResult<Foo>>(new Foo { Value = 30 });
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        CancellationToken collectCt = default;
        CancellationToken collectNamedCt = default;
        CancellationToken mapCt = default;
        CancellationToken mapNamedCt = default;

        // Act
        var collectResult = await task.CollectAsync(12, token, (foo, value, ct) =>
        {
            collectCt = ct;
            return Task.CompletedTask;
        });

        task = Task.FromResult<FindResult<Foo>>(new Foo { Value = 30 });
        var collectNamedResult = await task.CollectAsync("foo", 13, token, (foo, value, ct) =>
        {
            collectNamedCt = ct;
            return Task.CompletedTask;
        });

        task = Task.FromResult<FindResult<Foo>>(new Foo { Value = 30 });
        var mapResult = await task.MapAsync<Foo, Bar, int>(14, token, (foo, value, ct) =>
        {
            mapCt = ct;
            return Task.FromResult(new Bar { Value = foo.Value + value });
        });

        task = Task.FromResult<FindResult<Foo>>(new Foo { Value = 30 });
        var mapNamedResult = await task.MapAsync<Foo, Bar, int>("foo", 15, token, (foo, value, ct) =>
        {
            mapNamedCt = ct;
            return Task.FromResult(new Bar { Value = foo.Value + value });
        });

        // Assert
        Assert.True(collectResult.IsSuccess);
        Assert.True(collectNamedResult.IsSuccess);
        Assert.Equal(token, collectCt);
        Assert.Equal(token, collectNamedCt);

        Assert.True(mapResult.HasValue(out var bar));
        Assert.Equal(44, bar.Value);
        Assert.True(mapNamedResult.HasValue(out var namedBar));
        Assert.Equal(45, namedBar.Value);
        Assert.Equal(token, mapCt);
        Assert.Equal(token, mapNamedCt);
    }

    [Fact]
    public async Task FindResult_ById_Extensions_Async_WithTParam_UseCancellationToken()
    {
        // Arrange
        Task<FindResult<Foo, int>> task = Task.FromResult(new FindResult<Foo, int>(new Foo { Value = 40 }, 16));
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        CancellationToken collectCt = default;
        CancellationToken collectNamedCt = default;
        CancellationToken mapCt = default;
        CancellationToken mapNamedCt = default;

        // Act
        var collectResult = await task.CollectAsync<Foo, int, int>(17, token, (foo, value, ct) =>
        {
            collectCt = ct;
            return Task.CompletedTask;
        });

        task = Task.FromResult(new FindResult<Foo, int>(new Foo { Value = 40 }, 16));
        var collectNamedResult = await task.CollectAsync<Foo, int, int>("id", 18, token, (foo, value, ct) =>
        {
            collectNamedCt = ct;
            return Task.CompletedTask;
        });

        task = Task.FromResult(new FindResult<Foo, int>(new Foo { Value = 40 }, 16));
        var mapResult = await task.MapAsync<Foo, int, Bar, int>(19, token, (foo, value, ct) =>
        {
            mapCt = ct;
            return Task.FromResult(new Bar { Value = foo.Value + value });
        });

        task = Task.FromResult(new FindResult<Foo, int>(new Foo { Value = 40 }, 16));
        var mapNamedResult = await task.MapAsync<Foo, int, Bar, int>("id", 20, token, (foo, value, ct) =>
        {
            mapNamedCt = ct;
            return Task.FromResult(new Bar { Value = foo.Value + value });
        });

        // Assert
        Assert.True(collectResult.IsSuccess);
        Assert.True(collectNamedResult.IsSuccess);
        Assert.Equal(token, collectCt);
        Assert.Equal(token, collectNamedCt);

        Assert.True(mapResult.HasValue(out var bar));
        Assert.Equal(59, bar.Value);
        Assert.True(mapNamedResult.HasValue(out var namedBar));
        Assert.Equal(60, namedBar.Value);
        Assert.Equal(token, mapCt);
        Assert.Equal(token, mapNamedCt);
    }

    #region Continue Functions

    private static Func<Foo, Result> CreateContinue(Bar bar)
    {
        return (foo) =>
        {
            bar.Value = foo.Value;
            return Result.Ok();
        };
    }

    private static Func<Foo, Result> CreateContinue(Bar bar, Problem problem)
    {
        return (foo) =>
        {
            bar.Value = foo.Value;
            return problem;
        };
    }

    private static Func<Foo, Task<Result>> CreateContinueAsync(Bar bar)
    {
        return (foo) =>
        {
            bar.Value = foo.Value;
            return Task.FromResult(Result.Ok());
        };
    }

    private static Func<Foo, Task<Result>> CreateContinueAsync(Bar bar, Problem problem)
    {
        return (foo) =>
        {
            bar.Value = foo.Value;
            Result result = problem;
            return Task.FromResult(result);
        };
    }

    #endregion
}
