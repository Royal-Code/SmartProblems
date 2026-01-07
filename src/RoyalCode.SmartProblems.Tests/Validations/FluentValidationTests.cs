
using FluentValidation;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace RoyalCode.SmartProblems.Tests.Validations;

public class FluentValidationTests
{
    [Fact]
    public void Validation_SimpleClass_Success()
    {
        // Arrange
        var validator = new SimpleClassValidator();
        var simpleClass = new SimpleClass { Name = "John", Age = 20 };

        // Act
        var result = validator.Validate(simpleClass);

        var problems = result.Errors.ToProblems();

        // Assert
        Assert.Empty(problems);
    }

    [Fact]
    public void Validation_SimpleClass_Fail()
    {
        // Arrange
        var validator = new SimpleClassValidator();
        var simpleClass = new SimpleClass { Name = "", Age = 18 };

        // Act
        var result = validator.Validate(simpleClass);

        var problems = result.Errors.ToProblems();

        // Assert
        Assert.NotEmpty(problems);
        Assert.Equal(2, problems.Count);

        var problem = problems[0];
        Assert.Equal("Name", problem.Property);
        Assert.NotNull(problem.Extensions);
        Assert.Contains("error_code", problem.Extensions);

        problem = problems[1];
        Assert.Equal("Age", problem.Property);
        Assert.NotNull(problem.Extensions);
        Assert.Contains("error_code", problem.Extensions);
    }

    [Fact]
    public void Validation_ComplexClass_Success()
    {
        // Arrange
        var validator = new ComplexClassValidator();
        var complexClass = new ComplexClass
        {
            Description = "Description",
            Count = 4,
            Simple = new SimpleClass { Name = "John", Age = 20 }
        };

        // Act
        var result = validator.Validate(complexClass);

        var problems = result.Errors.ToProblems();

        // Assert
        Assert.Empty(problems);
    }

    [Fact]
    public void Validation_ComplexClass_Fail()
    {
        // Arrange
        var validator = new ComplexClassValidator();
        var complexClass = new ComplexClass
        {
            Description = "",
            Count = 3,
            Simple = new SimpleClass { Name = "", Age = 18 }
        };

        // Act
        var result = validator.Validate(complexClass);

        var problems = result.Errors.ToProblems();

        // Assert
        Assert.NotEmpty(problems);
        Assert.Equal(4, problems.Count);

        var problem = problems[0];
        Assert.Equal("Description", problem.Property);

        problem = problems[1];
        Assert.Equal("Count", problem.Property);

        problem = problems[2];
        Assert.Equal("Simple.Name", problem.Property);

        problem = problems[3];
        Assert.Equal("Simple.Age", problem.Property);
    }

    [Fact]
    public void Validation_HighComplexClass_Success()
    {
        // arrange
        var validator = new HighComplexClassValidator();
        var highComplexClass = new HighComplexClass
        {
            Description = "Description",
            Count = 4,
            Simples =
            [
                new SimpleClass { Name = "John", Age = 20 },
                new SimpleClass { Name = "Mary", Age = 21 }
            ]
        };

        // act
        var result = validator.Validate(highComplexClass);

        var problems = result.Errors.ToProblems();

        // assert
        Assert.Empty(problems);
    }

    [Fact]
    public void Validation_HighComplexClass_Fail()
    {
        // arrange
        var validator = new HighComplexClassValidator();
        var highComplexClass = new HighComplexClass
        {
            Description = "",
            Count = 3,
            Simples =
            [
                new SimpleClass { Name = "", Age = 20 },
                new SimpleClass { Name = "", Age = 18 }
            ]
        };

        // act
        var result = validator.Validate(highComplexClass);

        var problems = result.Errors.ToProblems();

        // assert
        Assert.NotEmpty(problems);
        Assert.Equal(5, problems.Count);

        var problem = problems[0];
        Assert.Equal("Description", problem.Property);

        problem = problems[1];
        Assert.Equal("Count", problem.Property);

        problem = problems[2];
        Assert.Equal("Simples[0].Name", problem.Property);

        problem = problems[3];
        Assert.Equal("Simples[1].Name", problem.Property);

        problem = problems[4];
        Assert.Equal("Simples[1].Age", problem.Property);
    }

    [Fact]
    public void Validation_ValidationResult_HasProblems_Success()
    {
        // Arrange
        var validator = new SimpleClassValidator();
        var simpleClass = new SimpleClass { Name = "John", Age = 20 };

        // Act
        var result = validator.Validate(simpleClass);
        var hasProblems = result.HasProblems(out var problems);

        // Assert
        Assert.False(hasProblems);
        Assert.Null(problems);
    }

    [Fact]
    public void Validation_ValidationResult_HasProblems_Fail()
    {
        // Arrange
        var validator = new SimpleClassValidator();
        var simpleClass = new SimpleClass { Name = "", Age = 18 };

        // Act
        var result = validator.Validate(simpleClass);
        var hasProblems = result.HasProblems(out var problems);

        // Assert
        Assert.True(hasProblems);
        Assert.NotNull(problems);
        Assert.Equal(2, problems!.Count);
    }

    [Fact]
    public void Validation_HasProblems_Success()
    {
        // Arrange
        var validator = new SimpleClassValidator();
        var simpleClass = new SimpleClass { Name = "John", Age = 20 };

        // Act
        var hasProblems = validator.HasProblems(simpleClass, out var problems);

        // Assert
        Assert.False(hasProblems);
        Assert.Null(problems);
    }

    [Fact]
    public void Validation_HasProblems_Fail()
    {
        // Arrange
        var validator = new SimpleClassValidator();
        var simpleClass = new SimpleClass { Name = "", Age = 18 };

        // Act
        var hasProblems = validator.HasProblems(simpleClass, out var problems);

        // Assert
        Assert.True(hasProblems);
        Assert.NotNull(problems);
        Assert.Equal(2, problems!.Count);
    }

    [Fact]
    public void Validation_ExtensionData()
    {
        // Arrange
        var validator = new ExDataModelValidator();
        var exDataModel = new ExDataModel { Name = "", Age = 18 };

        // Act
        validator.HasProblems(exDataModel, out var problems);

        // Assert
        Assert.NotNull(problems);
        Assert.Equal(2, problems!.Count);

        var problem = problems[0];
        Assert.NotNull(problem.Extensions);
        Assert.Equal(2, problem.Extensions.Count);
        Assert.Equal("", problem.Extensions["name"]);
        Assert.NotNull(problem.Extensions["error_code"]);

        problem = problems[1];
        Assert.NotNull(problem.Extensions);
        Assert.Equal(3, problem.Extensions.Count);
        Assert.Equal(18, problem.Extensions["current"]);
        Assert.Equal(19, problem.Extensions["required"]);
        Assert.NotNull(problem.Extensions["error_code"]);
    }

    [Fact]
    public void Validation_DisableErrorCode()
    {
        // Arrange
        var validator = new SimpleClassValidator();
        var simpleClass = new SimpleClass { Name = "", Age = 18 };
        var options = new ValidationToProblemOptions { IncludeErrorCode = false };

        // Act
        var result = validator.Validate(simpleClass);
        var problems = result.Errors.ToProblems(options);

        // Assert
        Assert.NotEmpty(problems);
        Assert.Equal(2, problems.Count);

        var problem = problems[0];
        Assert.Null(problem.Extensions);

        problem = problems[1];
        Assert.Null(problem.Extensions);
    }

    [Fact]
    public void Validation_ToResult_SuccessAndFail()
    {
        // Arrange
        var validator = new SimpleClassValidator();
        var ok = new SimpleClass { Name = "John", Age = 20 };
        var bad = new SimpleClass { Name = "", Age = 18 };

        // Act
        var okResult = validator.Validate(ok).ToResult();
        var badResult = validator.Validate(bad).ToResult();

        // Assert
        Assert.True(okResult.IsSuccess);
        Assert.True(badResult.HasProblems(out var problems));
        Assert.Equal(2, problems!.Count);
    }

    [Fact]
    public void Validation_EnsureIsValid_Sync()
    {
        // Arrange
        var validator = new SimpleClassValidator();
        var ok = new SimpleClass { Name = "John", Age = 20 };
        var bad = new SimpleClass { Name = "", Age = 18 };

        // Act
        var okResult = validator.EnsureIsValid(ok);
        var badResult = validator.EnsureIsValid(bad);

        // Assert
        Assert.True(okResult.HasValue(out var model));
        Assert.Equal("John", model.Name);

        Assert.True(badResult.HasProblems(out var problems));
        Assert.Equal(2, problems!.Count);
    }

    [Fact]
    public async Task Validation_EnsureIsValid_Async()
    {
        // Arrange
        var validator = new SimpleClassValidator();
        var ok = new SimpleClass { Name = "John", Age = 20 };
        var bad = new SimpleClass { Name = "", Age = 18 };

        // Act
        var okResult = await validator.EnsureIsValidAsync(ok);
        var badResult = await validator.EnsureIsValidAsync(bad);

        // Assert
        Assert.True(okResult.HasValue(out var model));
        Assert.Equal("John", model.Name);

        Assert.True(badResult.HasProblems(out var problems));
        Assert.Equal(2, problems!.Count);
    }

    [Fact]
    public void Validation_Validate_Result_Sync()
    {
        // Arrange
        var validator = new SimpleClassValidator();
        Result<SimpleClass> invalid = Problems.InvalidParameter("x");
        Result<SimpleClass> valid = new SimpleClass { Name = "John", Age = 20 };

        // Act
        var keepInvalid = invalid.Validate(validator);
        var validated = valid.Validate(validator);

        // Assert
        Assert.True(keepInvalid.HasProblems(out _));
        Assert.True(validated.HasValue(out var model));
        Assert.Equal("John", model.Name);
    }

    [Fact]
    public async Task Validation_Validate_Task_ValueTask_And_Async()
    {
        // Arrange
        var validator = new SimpleClassValidator();
        var model = new SimpleClass { Name = "John", Age = 20 };
        Task<Result<SimpleClass>> task = Task.FromResult<Result<SimpleClass>>(model);
        ValueTask<Result<SimpleClass>> vtask = new ValueTask<Result<SimpleClass>>(model);

        // Act
        var fromTask = await task.Validate(validator);
        var fromVTask = await vtask.Validate(validator);
        var asyncTask = await task.ValidateAsync(validator);
        var asyncVTask = await vtask.ValidateAsync(validator);

        // Assert
        Assert.True(fromTask.IsSuccess);
        Assert.True(fromVTask.IsSuccess);
        Assert.True(asyncTask.IsSuccess);
        Assert.True(asyncVTask.IsSuccess);
    }

    [Fact]
    public void Validation_Options_CustomCategory_And_ErrorCodeField()
    {
        // Arrange
        var validator = new SimpleClassValidator();
        var bad = new SimpleClass { Name = "", Age = 18 };
        var options = new ValidationToProblemOptions
        {
            Category = RoyalCode.SmartProblems.ProblemCategory.ValidationFailed,
            ErrorCodeExtensionField = "code"
        };

        // Act
        var problems = validator.Validate(bad).Errors.ToProblems(options);

        // Assert
        Assert.Equal(RoyalCode.SmartProblems.ProblemCategory.ValidationFailed, problems[0].Category);
        Assert.NotNull(problems[0].Extensions);
        Assert.True(problems[0].Extensions!.ContainsKey("code"));
    }

    [Fact]
    public void Validation_ExtensionData_DuplicatedKey_And_NullKey()
    {
        // Arrange
        var validator = new InlineValidator<SimpleClass>();
        validator.RuleFor(x => x.Name)
            .NotEmpty()
            .WithExtension(data =>
            {
                data.Add("k", "v");
                Assert.Throws<ArgumentNullException>(() => data.Add(null!, "x"));
            });

        // Act
        var result = validator.Validate(new SimpleClass { Name = "", Age = 20 });
        var problems = result.Errors.ToProblems();

        // Assert
        Assert.NotEmpty(problems);
        var ext = problems[0].Extensions!;
        Assert.Equal("v", ext["k"]);

        // Arrange
        var validatorDup = new InlineValidator<SimpleClass>();
        validatorDup.RuleFor(x => x.Name)
            .NotEmpty()
            .WithExtension(data =>
            {
                data.Add("dup", "a");
                Assert.Throws<ArgumentException>(() => data.Add("dup", "b"));
            });

        // Act
        var resultDup = validatorDup.Validate(new SimpleClass { Name = "", Age = 20 });
        var problemsDup = resultDup.Errors.ToProblems();

        // Assert
        var extDup = problemsDup[0].Extensions!;
        Assert.Equal("a", extDup["dup"]);
    }
}

#region Simple

file class SimpleClass
{
    public string Name { get; init; }

    public int Age { get; init; }
}

file class SimpleClassValidator : AbstractValidator<SimpleClass>
{
    public SimpleClassValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Age).GreaterThan(18);
    }
}

#endregion

#region Complex

file class ComplexClass
{
    public string Description { get; init; }

    public int Count { get; init; }

    public SimpleClass Simple { get; init; }
}

file class ComplexClassValidator : AbstractValidator<ComplexClass>
{
    public ComplexClassValidator()
    {
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Count).GreaterThan(3);
        RuleFor(x => x.Simple).SetValidator(new SimpleClassValidator());
    }
}

#endregion

#region HighComplex (Collection)

file class HighComplexClass
{
    public string Description { get; init; }

    public int Count { get; init; }

    public List<SimpleClass> Simples { get; init; }
}

file class HighComplexClassValidator : AbstractValidator<HighComplexClass>
{
    public HighComplexClassValidator()
    {
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Count).GreaterThan(3);
        RuleForEach(x => x.Simples).SetValidator(new SimpleClassValidator());
    }
}

#endregion

#region ExtensionData

file sealed class ExDataModel
{
    public string Name { get; init; }

    public int Age { get; init; }
}

file class ExDataModelValidator : AbstractValidator<ExDataModel>
{
    public ExDataModelValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithExtension(data => data.Add("name", data.Property));
        RuleFor(x => x.Age).GreaterThan(18).WithExtension(data => data.Add("current", data.Property).Add("required", 19));
    }
}

#endregion