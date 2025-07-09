using FluentValidation;

#pragma warning disable IDE0079 // Suppress unused warning disable
#pragma warning disable CA1822 // Mark members as static

namespace RoyalCode.SmartProblems.Tests.UseCases;

public class Foo
{
    public int Value { get; set; }
}

public class Bar
{
    public int Value { get; set; }
}

public class Baz
{
    public int Value { get; set; }
}

#nullable disable

public class FooBarContainer
{
    public Foo Foo { get; set; }
    public Bar Bar { get; set; }
    public Baz Baz { get; set; }
}

#nullable enable

public class FooValidator : AbstractValidator<Foo>
{
    public FooValidator()
    {
        RuleFor(x => x.Value).GreaterThan(0);
    }
}

public class FooBarService
{
    public Result<Foo> CreateFoo(int value)
    {
        return new Result<Foo>(new Foo { Value = value });
    }
    
    public Task<Result<Foo>> CreateFooAsync(int value)
    {
        return Task.FromResult(new Result<Foo>(new Foo { Value = value }));
    }
    
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