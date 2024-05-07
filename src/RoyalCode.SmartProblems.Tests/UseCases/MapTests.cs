using FluentValidation;

namespace RoyalCode.SmartProblems.Tests.UseCases;

public class MapTests
{
    [Fact]
    public void Map_From_EnsureIsValid()
    {
        // Arrange
        var foo = new Foo() { Value = 1 };
        var validator = new FooValidator();

        // Act
        var result = validator.EnsureIsValid(foo)
            .Map(f => new Bar { Value = f.Value });

        // Assert
        var hasBar = result.HasValue(out var bar);
        Assert.True(hasBar);
        Assert.NotNull(bar);
        Assert.Equal(foo.Value, bar.Value);
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
    public async Task Map_ContinueAsync()
    {
        // Arrange
        var validator = new FooValidator();
        var service = new FooBarService();
        
        // Act
        var result = await service.CreateFoo(1)
            .Validate(validator)
            .Map(f => new FooBarContainer { Foo = f })
            .ContinueAsync(service, async (c, s) => c.Bar = await s.FindBar(c.Foo))
            .ContinueAsync(service, async (c, s) => await s.ProcessBar(c.Bar).ContinueAsync(c, (baz, cx) => cx.Baz = baz));
        
        // Assert
        var hasContainer = result.HasValue(out var container);
        Assert.True(hasContainer);
        Assert.NotNull(container);
        Assert.NotNull(container.Foo);
        Assert.NotNull(container.Bar);
        Assert.NotNull(container.Baz);
    }

    [Fact]
    public async Task Not_Map_ContinueAsync()
    {
        // Act
        var result = await CreateFooBarContainerAsync();

        // Assert
        var hasContainer = result.HasValue(out var container);
        Assert.True(hasContainer);
        Assert.NotNull(container);
        Assert.NotNull(container.Foo);
        Assert.NotNull(container.Bar);
        Assert.NotNull(container.Baz);
    }
    
    private async ValueTask<Result<FooBarContainer>> CreateFooBarContainerAsync()
    {
        // Arrange
        var validator = new FooValidator();
        var service = new FooBarService();
        var container = new FooBarContainer();
        
        // Act
        var fooResult = service.CreateFoo(1)
            .Validate(validator);
        if (fooResult.HasProblems(out var problems))
            return problems;
        
        fooResult.Continue(container, (f, c) => c.Foo = f);

        container.Bar = await service.FindBarAsync(container.Foo);
        
        var bazResult = await service.ProcessBarAsync(container.Bar);
        if (bazResult.HasProblems(out problems))
            return problems;
        
        bazResult.Continue(container, (baz, c) => c.Baz = baz);
        
        return container;
    }
}