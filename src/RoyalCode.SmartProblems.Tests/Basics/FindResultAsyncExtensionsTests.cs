using RoyalCode.SmartProblems.Entities;
using RoyalCode.SmartProblems.Tests.UseCases;

namespace RoyalCode.SmartProblems.Tests.Basics;

public class FindResultAsyncExtensionsTests
{
    [Fact]
    public async Task Task_FindResult_ContinueAsync_WithParameterName_NotFound_DoesNotInvokeReceiver()
    {
        // Arrange
        Task<FindResult<Foo>> task = Task.FromResult(new FindResult<Foo>());
        var called = false;

        // Act
        var result = await task.ContinueAsync("foo", foo =>
        {
            called = true;
            return Result.Ok();
        });

        // Assert
        Assert.False(called);
        Assert.True(result.HasProblems(out var problems));
        AssertInvalidParameter(problems, "foo");
    }

    [Fact]
    public async Task Task_FindResult_ContinueAsync_WithParameterName_Async_NotFound_DoesNotInvokeReceiver()
    {
        // Arrange
        Task<FindResult<Foo>> task = Task.FromResult(new FindResult<Foo>());
        var called = false;

        // Act
        var result = await task.ContinueAsync("foo", foo =>
        {
            called = true;
            return Task.FromResult(Result.Ok());
        });

        // Assert
        Assert.False(called);
        Assert.True(result.HasProblems(out var problems));
        AssertInvalidParameter(problems, "foo");
    }

    [Fact]
    public async Task Task_FindResult_ContinueAsync_WithTParam_UsesCancellationToken()
    {
        // Arrange
        Task<FindResult<Foo>> task = Task.FromResult<FindResult<Foo>>(new Foo { Value = 10 });
        using var cts = new CancellationTokenSource();
        var token = cts.Token;
        CancellationToken captured = default;

        // Act
        var result = await task.ContinueAsync(4, (foo, value, ct) =>
        {
            captured = ct;
            foo.Value += value;
            return Task.FromResult(Result.Ok());
        }, token);

        // Assert
        Assert.Equal(token, captured);
        Assert.True(result.HasValue(out var foo));
        Assert.Equal(14, foo!.Value);
    }

    [Fact]
    public async Task Task_FindResult_ContinueAsync_WithParameterNameAndTParam_NotFound_DoesNotInvokeReceiver()
    {
        // Arrange
        Task<FindResult<Foo>> task = Task.FromResult(new FindResult<Foo>());
        var called = false;

        // Act
        var result = await task.ContinueAsync("foo", 4, (foo, value, ct) =>
        {
            called = true;
            return Task.FromResult(Result.Ok());
        }, CancellationToken.None);

        // Assert
        Assert.False(called);
        Assert.True(result.HasProblems(out var problems));
        AssertInvalidParameter(problems, "foo");
    }

    [Fact]
    public async Task Task_FindResult_ById_ContinueAsync_WithParameterName_NotFound_DoesNotInvokeReceiver()
    {
        // Arrange
        Task<FindResult<Foo, int>> task = Task.FromResult(new FindResult<Foo, int>(55));
        var called = false;

        // Act
        var result = await task.ContinueAsync("id", foo =>
        {
            called = true;
            return Result.Ok();
        });

        // Assert
        Assert.False(called);
        Assert.True(result.HasProblems(out var problems));
        AssertInvalidParameter(problems, "id");
    }

    [Fact]
    public async Task Task_FindResult_ById_ContinueAsync_WithTParam_UsesCancellationToken()
    {
        // Arrange
        Task<FindResult<Foo, int>> task = Task.FromResult(new FindResult<Foo, int>(new Foo { Value = 20 }, 7));
        using var cts = new CancellationTokenSource();
        var token = cts.Token;
        CancellationToken captured = default;

        // Act
        var result = await task.ContinueAsync<Foo, int, int>(5, (foo, value, ct) =>
        {
            captured = ct;
            foo.Value += value;
            return Task.FromResult(Result.Ok());
        }, token);

        // Assert
        Assert.Equal(token, captured);
        Assert.True(result.HasValue(out var foo));
        Assert.Equal(25, foo!.Value);
    }

    [Fact]
    public async Task Task_FindResult_CollectAsync_WithParameterName_Action_NotFound_DoesNotInvokeReceiver()
    {
        // Arrange
        Task<FindResult<Foo>> task = Task.FromResult(new FindResult<Foo>());
        var called = false;

        // Act
        var result = await task.CollectAsync("foo", _ => called = true);

        // Assert
        Assert.False(called);
        Assert.True(result.HasProblems(out var problems));
        AssertInvalidParameter(problems, "foo");
    }

    [Fact]
    public async Task Task_FindResult_ById_CollectAsync_WithParameterName_Action_NotFound_DoesNotInvokeReceiver()
    {
        // Arrange
        Task<FindResult<Foo, int>> task = Task.FromResult(new FindResult<Foo, int>(11));
        var called = false;

        // Act
        var result = await task.CollectAsync("id", _ => called = true);

        // Assert
        Assert.False(called);
        Assert.True(result.HasProblems(out var problems));
        AssertInvalidParameter(problems, "id");
    }

    [Fact]
    public async Task Task_FindResult_MapAsync_WithTaskOfResult_Found_ReturnsValue()
    {
        // Arrange
        Task<FindResult<Foo>> task = Task.FromResult<FindResult<Foo>>(new Foo { Value = 30 });

        // Act
        var result = await task.MapAsync(foo => Task.FromResult<Result<Bar>>(new Bar { Value = foo.Value + 2 }));

        // Assert
        Assert.True(result.HasValue(out var bar));
        Assert.Equal(32, bar!.Value);
    }

    [Fact]
    public async Task Task_FindResult_ById_MapAsync_WithParameterNameAndTParam_NotFound_DoesNotInvokeReceiver()
    {
        // Arrange
        Task<FindResult<Foo, int>> task = Task.FromResult(new FindResult<Foo, int>(12));
        var called = false;

        // Act
        var result = await task.MapAsync<Foo, int, Bar, int>("id", 2, (foo, value) =>
        {
            called = true;
            return new Bar { Value = foo.Value + value };
        });

        // Assert
        Assert.False(called);
        Assert.True(result.HasProblems(out var problems));
        AssertInvalidParameter(problems, "id");
    }

    private static void AssertInvalidParameter(Problems? problems, string property)
    {
        Assert.NotNull(problems);
        Assert.Contains(problems!, p =>
            p.Category == ProblemCategory.InvalidParameter
            && p.Property == property);
    }
}
