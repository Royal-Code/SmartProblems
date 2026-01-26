using RoyalCode.SmartProblems.Entities;
using RoyalCode.SmartProblems.HttpResults;
using System.Diagnostics.CodeAnalysis;

namespace RoyalCode.SmartProblems.TestsApi.Apis;

public static class MatchApi
{
    private readonly static PersonService _personService = new();

    public static IEndpointRouteBuilder MapMatchApi(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/match").WithTags("Match API");

        group.MapPost("/", CreatePerson);
        group.MapGet("/{id:int}", GetPerson);
        group.MapPatch("/{id:int}/name", UpdatePersonName);
        group.MapPatch("/{id:int}/age", UpdatePersonAge);
        group.MapDelete("/{id:int}", DeletePerson);

        return group;

    }

    private static async Task<CreatedMatch<PersonDetails>> CreatePerson(PersonCreate create)
    {
        await Task.Delay(10); // Simulate async work

        return _personService.CreatePerson(create)
            .Map(person => new PersonDetails
            {
                Id = person.Id,
                Name = person.Name,
                Age = person.Age
            }).CreatedMatch(p => $"/api/match/{p.Id}");
    }

    private static async Task<OkMatch<PersonDetails>> GetPerson(int id)
    {
        await Task.Delay(10); // Simulate async work

        return _personService.GetPerson(id)
            .Map(person => new PersonDetails
            {
                Id = person.Id,
                Name = person.Name,
                Age = person.Age
            });
    }

    private static async Task<OkMatch> UpdatePersonName(int id, PersonUpdateName model)
    {
        await Task.Delay(10);

        return _personService.UpdatePersonName(id, model);
    }

    private static async Task<OkMatch> UpdatePersonAge(int id, PersonUpdateAge model)
    {
        await Task.Delay(10);
        return _personService.UpdatePersonAge(id, model);
    }

    private static async Task<NoContentMatch> DeletePerson(int id)
    {
        await Task.Delay(10);
        return _personService.DeletePerson(id);
    }
}

#region MODELS AND SERVICE

public class Person
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
}

public class PersonCreate
{
    public required string Name { get; set; }

    public int Age { get; set; }

    // Manual validation without Rules lib
    public bool HasProblems([NotNullWhen(true)] out Problems? problems)
    {
        Problems? p = null;

        if (string.IsNullOrWhiteSpace(Name))
        {
            p ??= Problems.InvalidParameter("Validation failed.")
                .With("Name", "Name must not be empty.");
        }

        if (Age < 16)
        {
            p ??= Problems.InvalidParameter("Validation failed.")
                .With("Age", "Age must be at least 16.");
        }

        problems = p;
        return problems is not null;
    }
}

public class PersonDetails
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
}

public class PersonUpdateName
{
    public string Name { get; set; } = string.Empty;

    // Manual validation without Rules lib
    public bool HasProblems([NotNullWhen(true)] out Problems? problems)
    {
        Problems? p = null;

        if (string.IsNullOrWhiteSpace(Name))
        {
            p = Problems.InvalidParameter("Validation failed.")
                .With("Name", "Name must not be empty.");
        }

        problems = p;
        return problems is not null;
    }
}

public class PersonUpdateAge
{
    public int Age { get; set; }

    // Manual validation without Rules lib
    public bool HasProblems([NotNullWhen(true)] out Problems? problems)
    {
        Problems? p = null;

        if (Age < 16)
        {
            p = Problems.InvalidParameter("Validation failed.")
                .With("Age", "Age must be at least 16.");
        }

        problems = p;
        return problems is not null;
    }
}

public class PersonService
{
    private readonly List<Person> _people = [];

    public Result<Person> CreatePerson(PersonCreate create)
    {
        if (create.HasProblems(out var problems))
            return problems;

        var person = new Person
        {
            Id = _people.Count + 1,
            Name = create.Name,
            Age = create.Age
        };

        _people.Add(person);

        return person;
    }

    public FindResult<PersonDetails, int> GetPerson(int id)
    {
        var person = _people.FirstOrDefault(p => p.Id == id);
        if (person is null)
            return new(null, id);

        return new(
            new PersonDetails
            {
                Id = person.Id,
                Name = person.Name,
                Age = person.Age
            },
            id);
    }

    public Result UpdatePersonName(int id, PersonUpdateName update)
    {
        if (update.HasProblems(out var problems))
            return problems;

        var person = _people.FirstOrDefault(p => p.Id == id);
        if (person is null)
            return Problems.NotFound("Person not found.")
                .With("id", id);

        person.Name = update.Name;

        return Result.Ok();
    }

    public Result UpdatePersonAge(int id, PersonUpdateAge update)
    {
        if (update.HasProblems(out var problems))
            return problems;

        var person = _people.FirstOrDefault(p => p.Id == id);
        if (person is null)
            return Problems.NotFound("Person not found.")
                .With("id", id);

        person.Age = update.Age;

        return Result.Ok();
    }

    public Result DeletePerson(int id)
    {
        var person = _people.FirstOrDefault(p => p.Id == id);
        if (person is null)
            return Problems.NotFound("Person not found.")
                .With("id", id);

        _people.Remove(person);

        return Result.Ok();
    }
}

#endregion
