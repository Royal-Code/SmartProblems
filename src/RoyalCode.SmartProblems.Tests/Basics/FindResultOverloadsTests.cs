using RoyalCode.SmartProblems.Entities;
using RoyalCode.SmartProblems.Tests.UseCases;

namespace RoyalCode.SmartProblems.Tests.Basics;

public class FindResultOverloadsTests
{
    [Fact]
    public void FindResult_ToResult_WithParameterName_NotFound_ReturnsInvalidParameter()
    {
        // Arrange
        FindResult<Foo> findResult = new();

        // Act
        var result = findResult.ToResult("foo");

        // Assert
        Assert.True(result.HasProblems(out var problems));
        AssertInvalidParameter(problems, "foo");
    }

    [Fact]
    public void FindResult_ById_ToResult_WithParameterName_NotFound_ReturnsInvalidParameter()
    {
        // Arrange
        FindResult<Foo, int> findResult = new(42);

        // Act
        var result = findResult.ToResult("id");

        // Assert
        Assert.True(result.HasProblems(out var problems));
        AssertInvalidParameter(problems, "id");
        Assert.Contains(problems!, p =>
            p.Extensions is not null
            && p.Extensions.TryGetValue("id", out var id)
            && id is int value
            && value == 42);
    }

    [Fact]
    public async Task FindResult_ContinueAsync_WithTParam_UsesCancellationToken()
    {
        // Arrange
        FindResult<Foo> findResult = new Foo { Value = 10 };
        using var cts = new CancellationTokenSource();
        var token = cts.Token;
        CancellationToken captured = default;

        // Act
        var result = await findResult.ContinueAsync(5, token, (foo, value, ct) =>
        {
            captured = ct;
            foo.Value += value;
            return Task.FromResult(Result.Ok());
        });

        // Assert
        Assert.Equal(token, captured);
        Assert.True(result.HasValue(out var foo));
        Assert.Equal(15, foo!.Value);
    }

    [Fact]
    public async Task FindResult_ContinueAsync_WithParameterNameAndTParam_NotFound_DoesNotInvokeReceiver()
    {
        // Arrange
        FindResult<Foo> findResult = new();
        var called = false;

        // Act
        var result = await findResult.ContinueAsync("foo", 5, CancellationToken.None, (foo, value, ct) =>
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
    public async Task FindResult_ById_ContinueAsync_WithTParam_UsesCancellationToken()
    {
        // Arrange
        FindResult<Foo, int> findResult = new(new Foo { Value = 20 }, 7);
        using var cts = new CancellationTokenSource();
        var token = cts.Token;
        CancellationToken captured = default;

        // Act
        var result = await findResult.ContinueAsync(3, token, (foo, value, ct) =>
        {
            captured = ct;
            foo.Value += value;
            return Task.FromResult(Result.Ok());
        });

        // Assert
        Assert.Equal(token, captured);
        Assert.True(result.HasValue(out var foo));
        Assert.Equal(23, foo!.Value);
    }

    [Fact]
    public async Task FindResult_ById_ContinueAsync_WithParameterNameAndTParam_NotFound_DoesNotInvokeReceiver()
    {
        // Arrange
        FindResult<Foo, int> findResult = new(10);
        var called = false;

        // Act
        var result = await findResult.ContinueAsync("id", 5, CancellationToken.None, (foo, value, ct) =>
        {
            called = true;
            return Task.FromResult(Result.Ok());
        });

        // Assert
        Assert.False(called);
        Assert.True(result.HasProblems(out var problems));
        AssertInvalidParameter(problems, "id");
    }

    [Fact]
    public async Task FindResult_CollectAsync_WithParameterName_NotFound_DoesNotInvokeReceiver()
    {
        // Arrange
        FindResult<Foo> findResult = new();
        var called = false;

        // Act
        var result = await findResult.CollectAsync("foo", _ =>
        {
            called = true;
            return Task.CompletedTask;
        });

        // Assert
        Assert.False(called);
        Assert.True(result.HasProblems(out var problems));
        AssertInvalidParameter(problems, "foo");
    }

    [Fact]
    public void FindResult_ById_Map_WithParameterNameAndTParam_NotFound_DoesNotInvokeReceiver()
    {
        // Arrange
        FindResult<Foo, int> findResult = new(99);
        var called = false;

        // Act
        var result = findResult.Map<Bar, int>("id", 2, (foo, value) =>
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