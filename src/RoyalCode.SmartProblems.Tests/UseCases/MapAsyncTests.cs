using FluentValidation;

namespace RoyalCode.SmartProblems.Tests.UseCases;

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