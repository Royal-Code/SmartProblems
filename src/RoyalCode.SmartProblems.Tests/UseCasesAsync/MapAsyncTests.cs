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
    public async Task MapAsync_From_EnsureIsValid()
    {
        // Arrange
        var foo = new Foo() { Value = 1 };
        var validator = new FooValidator();
        var service = new FooBarService();

        // Act
        var result = await validator.EnsureIsValid(foo)
            .MapAsync(service, static async (f, s) => await s.FindBarAsync(f))
            .MapAsync(service, static async (b, s) => await s.ProcessBar(b));

        // Assert
        var hasBaz = result.HasValue(out var baz);
        Assert.True(hasBaz);
        Assert.NotNull(baz);
        Assert.Equal(foo.Value + 1, baz.Value);
    }

    [Fact]
    public void Map_From_EnsureIsValid_Fail()
    {
        // Arrange
        var foo = new Foo() { Value = -1 };
        var validator = new FooValidator();

        // Act
        var result = validator.EnsureIsValid(foo)
            .Map(f => new Bar { Value = f.Value });

        // Assert
        var hasBar = result.HasValue(out var bar);
        Assert.False(hasBar);
        Assert.Null(bar);
        
        var hasProblems = result.HasProblems(out var problems);
        Assert.True(hasProblems);
        Assert.NotNull(problems);
    }

    [Fact]
    public async Task MapAsync_From_MapAsync_Use_Task()
    {
        // Arrange
        var foo = new Foo() { Value = 1 };
        var service = new FooBarService();
        var result = new Result<Foo>(foo);
        
        // Act
        var barResult = await result
            .MapAsync(service, static async (f, s) => await s.FindBarAsync(f))
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
        var barResult = await result
            .MapAsync(service, static async (f, s) => await s.FindBar(f))
            .MapAsync(b => new Bar { Value = b.Value + 1 });

        // Assert
        var hasBar = barResult.HasValue(out var bar);
        Assert.True(hasBar);
        Assert.NotNull(bar);
        Assert.Equal(foo.Value + 1, bar.Value);
    
    }

    [Fact]
    public async Task Result_MapAsync_From_MapAsync_Use_Task()
    {
        // Arrange
        var foo = new Foo() { Value = 1 };
        var service = new FooBarService();
        var result = new Result<Foo>(foo);

        // Act
        var bazResult = await result
            .MapAsync(service, static async (f, s) => await s.CreateBarAsync(f))
            .MapAsync(service, static async (b, s) => await s.ProcessBarAsync(b));

        // Assert
        var hasBaz = bazResult.HasValue(out var baz);
        Assert.True(hasBaz);
        Assert.NotNull(baz);
        Assert.Equal(foo.Value + 1, baz.Value);
    }

    [Fact]
    public async Task Result_MapAsync_From_MapAsync_Use_ValueTask()
    {
        // Arrange
        var foo = new Foo() { Value = 1 };
        var service = new FooBarService();
        var result = new Result<Foo>(foo);

        // Act
        var bazResult = await result
            .MapAsync(service, static async (f, s) => await s.CreateBar(f))
            .MapAsync(service, static async (b, s) => await s.ProcessBar(b));

        // Assert
        var hasBaz = bazResult.HasValue(out var baz);
        Assert.True(hasBaz);
        Assert.NotNull(baz);
        Assert.Equal(foo.Value + 1, baz.Value);
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

file class Baz
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

    public Task<Result<Bar>> CreateBarAsync(Foo foo)
    {
        return Task.FromResult(new Result<Bar>(new Bar { Value = foo.Value }));
    }

    public ValueTask<Result<Bar>> CreateBar(Foo foo)
    {
        return new ValueTask<Result<Bar>>(new Result<Bar>(new Bar { Value = foo.Value }));
    }

    public Task<Result<Baz>> ProcessBarAsync(Bar bar)
    {
        return Task.FromResult(new Result<Baz>(new Baz { Value = bar.Value + 1 } ));
    }

    public ValueTask<Result<Baz>> ProcessBar(Bar bar)
    {
        return new ValueTask<Result<Baz>>(new Result<Baz>(new Baz { Value = bar.Value + 1 } ));
    }
}