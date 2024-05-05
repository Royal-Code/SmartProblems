using FluentValidation;

namespace RoyalCode.SmartProblems.Tests.UseCasesAsync;

public class MapAsyncTests
{
    [Fact]
    public async Task MapAsync_From_EnsureIsValidAsync()
    {
        // Arrange
        var foo = new Foo() { Value = 1 };
        var validator = new FooValidator();
        
        // Act
        var result = await validator.EnsureIsValidAsync(foo).MapAsync(f => new Bar{ Value = f.Value });

        // Assert
        var hasBar = result.HasValue(out var bar);
        Assert.True(hasBar);
        Assert.NotNull(bar);
        Assert.Equal(foo.Value, bar.Value);
    }

    [Fact]
    public async Task MapAsync_From_MapAsync_Use_Task()
    {
        // Arrange
        var foo = new Foo() { Value = 1 };
        var service = new FooBarService();
        var result = new Result<Foo>(foo);
        
        // Act
        var barResult = await result.MapAsync(service, static async (f, s) => await s.FindBarAsync(f))
            .MapAsync(b => new Bar { Value = b.Value + 1 });

        // Assert
        var hasBar = barResult.HasValue(out var bar);
        Assert.True(hasBar);
        Assert.NotNull(bar);
        Assert.Equal(foo.Value + 1, bar.Value);
    }

    [Fact]
    public async Task MapAsync_From_MapAsync_Use_ValueTask()
    {
        // Arrange
        var foo = new Foo() { Value = 1 };
        var service = new FooBarService();
        var result = new Result<Foo>(foo);
        
        // Act
        var barResult = await result.MapAsync(service, static async (f, s) => await s.FindBar(f))
            .MapAsync(b => new Bar { Value = b.Value + 1 });

        // Assert
        var hasBar = barResult.HasValue(out var bar);
        Assert.True(hasBar);
        Assert.NotNull(bar);
        Assert.Equal(foo.Value + 1, bar.Value);
    
    }
}

file class Foo
{
    public int Value { get; set; }
}

file class Bar
{
    public int Value { get; set; }
}

file class FooValidator : AbstractValidator<Foo>
{
    public FooValidator()
    {
        RuleFor(x => x.Value).GreaterThan(0);
    }
}

file class FooBarService
{
    public Task<Bar> FindBarAsync(Foo foo)
    {
        return Task.FromResult(new Bar { Value = foo.Value });
    }

    public ValueTask<Bar> FindBar(Foo foo)
    {
        return new ValueTask<Bar>(new Bar { Value = foo.Value });
    }
}