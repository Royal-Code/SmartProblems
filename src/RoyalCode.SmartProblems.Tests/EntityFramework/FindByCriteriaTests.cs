using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RoyalCode.SmartProblems.Entities;
using RoyalCode.SmartProblems.TestsApi.Apis;

namespace RoyalCode.SmartProblems.Tests.EntityFramework;

public class FindByCriteriaTests
{
    private static void SeedCities(ContactsDbContext db, out int scId, out int spId)
    {
        var brazil = new Country("Brazil", "BR");
        db.Set<Country>().Add(brazil);
        db.SaveChanges();

        var sc = new State("Santa Catarina", "SC", brazil.Id);
        var sp = new State("São Paulo", "SP", brazil.Id);
        db.Set<State>().AddRange(sc, sp);
        db.SaveChanges();

        db.Set<City>().AddRange(
            new City("Blumenau", sc.Id),
            new City("Joinville", sc.Id),
            new City("Blumenau", sp.Id));
        db.SaveChanges();

        scId = sc.Id;
        spId = sp.Id;
    }

    private sealed class CityFixture : IDisposable
    {
        private readonly SqliteConnection connection;

        public ContactsDbContext Db { get; }
        public int ScId { get; }
        public int SpId { get; }

        public CityFixture()
        {
            connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var optionsBuilder = new DbContextOptionsBuilder<ContactsDbContext>();
            optionsBuilder.UseSqlite(connection);

            Db = new ContactsDbContext(optionsBuilder.Options);
            Db.Database.EnsureCreated();

            SeedCities(Db, out var scId, out var spId);
            ScId = scId;
            SpId = spId;
        }

        public void Dispose()
        {
            Db.Dispose();
            connection.Dispose();
        }
    }

    [Fact]
    public async Task TryFindAsync_MultipleCriteria_Found_ReturnsEntity()
    {
        // Arrange
        using var fixture = new CityFixture();

        // Act
        var find = await fixture.Db.FindByCriteria<City>()
            .By(c => c.StateId, fixture.ScId)
            .By(c => c.Name, "Blumenau")
            .TryFindAsync();

        // Assert
        Assert.True(find.Found);
        Assert.Equal(fixture.ScId, find.Entity!.StateId);
        Assert.Equal("Blumenau", find.Entity.Name);
    }

    [Fact]
    public async Task TryFindAsync_MultipleCriteria_NotFound_DetailListsAllInDeclarationOrder()
    {
        // Arrange
        using var fixture = new CityFixture();

        // Act
        var find = await fixture.Db.FindByCriteria<City>()
            .By(c => c.StateId, fixture.ScId)
            .By(c => c.Name, "Nonexistent")
            .TryFindAsync();
        var notFound = find.NotFound(out var problem);

        // Assert
        Assert.True(notFound);
        Assert.NotNull(problem);
        Assert.Equal(
            $"The record of 'City' with StateId '{fixture.ScId}', Name 'Nonexistent' was not found",
            problem.Detail);

        Assert.NotNull(problem.Extensions);
        problem.Extensions.TryGetValue("entity", out var entityValue);
        Assert.Equal("City", (string)entityValue!);
        problem.Extensions.TryGetValue("StateId", out var stateIdValue);
        Assert.Equal(fixture.ScId, (int)stateIdValue!);
        problem.Extensions.TryGetValue("Name", out var nameValue);
        Assert.Equal("Nonexistent", (string)nameValue!);
    }

    [Fact]
    public async Task TryFindAsync_ExplicitByName_UsedInDetail()
    {
        // Arrange
        using var fixture = new CityFixture();

        // Act
        var find = await fixture.Db.FindByCriteria<City>()
            .By(c => c.StateId, fixture.ScId, "State")
            .By(c => c.Name, "Nonexistent")
            .TryFindAsync();
        var notFound = find.NotFound(out var problem);

        // Assert
        Assert.True(notFound);
        Assert.NotNull(problem);
        Assert.Equal(
            $"The record of 'City' with State '{fixture.ScId}', Name 'Nonexistent' was not found",
            problem.Detail);
    }

    [Fact]
    public async Task TryFindAsync_ArbitraryPredicateOverload_Found()
    {
        // Arrange
        using var fixture = new CityFixture();

        // Act
        var find = await fixture.Db.FindByCriteria<City>()
            .By(c => c.StateId, fixture.ScId)
            .By(c => c.Name.StartsWith("Blu"), "Name", "name", "Blu")
            .TryFindAsync();

        // Assert
        Assert.True(find.Found);
        Assert.Equal("Blumenau", find.Entity!.Name);
    }

    [Fact]
    public async Task TryFindAsync_ConditionalBy_BothBranchesWork()
    {
        // Arrange
        using var fixture = new CityFixture();

        async Task<FindResult<City>> SearchAsync(string? name)
        {
            var criteria = fixture.Db.FindByCriteria<City>().By(c => c.StateId, fixture.ScId);
            if (name is not null)
                criteria = criteria.By(c => c.Name, name);

            return await criteria.TryFindAsync();
        }

        // Act
        var withName = await SearchAsync("Joinville");
        var withoutName = await SearchAsync(null);

        // Assert
        Assert.True(withName.Found);
        Assert.Equal("Joinville", withName.Entity!.Name);
        Assert.True(withoutName.Found);
    }

    [Fact]
    public async Task TryFindAsync_ForkedBuilder_DoesNotLeakCriteriaBetweenBranches()
    {
        // Arrange
        using var fixture = new CityFixture();
        var baseCriteria = fixture.Db.FindByCriteria<City>().By(c => c.StateId, fixture.ScId);

        var left = baseCriteria.By(c => c.Name, "NotThere1");
        var right = baseCriteria.By(c => c.Name, "NotThere2");

        // Act
        var leftNotFound = (await left.TryFindAsync()).NotFound(out var leftProblem);
        var rightNotFound = (await right.TryFindAsync()).NotFound(out var rightProblem);

        // Assert
        Assert.True(leftNotFound);
        Assert.True(rightNotFound);
        Assert.NotNull(leftProblem);
        Assert.NotNull(rightProblem);

        Assert.Equal(
            $"The record of 'City' with StateId '{fixture.ScId}', Name 'NotThere1' was not found",
            leftProblem.Detail);
        Assert.Equal(
            $"The record of 'City' with StateId '{fixture.ScId}', Name 'NotThere2' was not found",
            rightProblem.Detail);
    }

    [Fact]
    public async Task By_And_TryFindAsync_OnDefaultFindCriteria_ThrowsInvalidOperationException()
    {
        // Arrange
        FindCriteria<City> broken = default;

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => broken.By(c => c.Name, "x"));
        await Assert.ThrowsAsync<InvalidOperationException>(() => broken.TryFindAsync());
    }

    [Fact]
    public void By_SelectorNotRootedOnParameter_ThrowsArgumentException()
    {
        // Arrange
        using var fixture = new CityFixture();
        var other = new City("Other", 999);
        var criteria = fixture.Db.FindByCriteria<City>();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => criteria.By(c => other.Name, "x"));
    }

    [Fact]
    public async Task TryFindAsync_DuplicateProperty_DetailListsBoth_ExtensionsLastWins()
    {
        // Arrange
        using var fixture = new CityFixture();

        // Act
        var find = await fixture.Db.FindByCriteria<City>()
            .By(c => c.Name, "A")
            .By(c => c.Name, "B")
            .TryFindAsync();
        var notFound = find.NotFound(out var problem);

        // Assert
        Assert.True(notFound);
        Assert.NotNull(problem);
        Assert.Equal("The record of 'City' with Name 'A', Name 'B' was not found", problem.Detail);

        Assert.NotNull(problem.Extensions);
        problem.Extensions.TryGetValue("Name", out var nameValue);
        Assert.Equal("B", (string)nameValue!);
    }

    [Fact]
    public async Task TryFindAsync_GeneratesParameterizedQuery_NotLiteralConstants()
    {
        // Arrange
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var logs = new List<string>();
        var optionsBuilder = new DbContextOptionsBuilder<ContactsDbContext>();
        optionsBuilder.UseSqlite(connection);
        optionsBuilder.LogTo(logs.Add, LogLevel.Information);

        using var db = new ContactsDbContext(optionsBuilder.Options);
        db.Database.EnsureCreated();
        SeedCities(db, out var scId, out _);

        logs.Clear(); // ignore setup noise, focus on the search query below

        // Act
        await db.FindByCriteria<City>()
            .By(c => c.StateId, scId)
            .By(c => c.Name, "Blumenau")
            .TryFindAsync();

        // Assert: EF parameterizes closure-captured values (e.g. "@value", "@value0") instead of
        // inlining them as SQL literals — see ExpressionCapture.
        var combined = string.Join(Environment.NewLine, logs);
        Assert.Contains("@value", combined);
        Assert.DoesNotContain("'Blumenau'", combined);
    }

    [Fact]
    public async Task TryFindByAsync_ComposedAndAlsoPredicate_NoLongerThrows_GeneratesMultiFieldMessage()
    {
        // Arrange
        using var fixture = new CityFixture();

        // Act
        var entry = await fixture.Db.TryFindByAsync<City>(c => c.StateId == fixture.ScId && c.Name == "Nonexistent");
        var notFound = entry.NotFound(out var problem);

        // Assert
        Assert.True(notFound);
        Assert.NotNull(problem);
        Assert.Equal(
            $"The record of 'City' with StateId '{fixture.ScId}', Name 'Nonexistent' was not found",
            problem.Detail);
    }

    [Fact]
    public async Task TryFindByAsync_OrElsePredicate_DegradesToGenericMessage()
    {
        // Arrange
        using var fixture = new CityFixture();

        // Act
        var entry = await fixture.Db.TryFindByAsync<City>(c => c.Name == "NotThere1" || c.Name == "NotThere2");
        var notFound = entry.NotFound(out var problem);

        // Assert
        Assert.True(notFound);
        Assert.NotNull(problem);
        Assert.Equal("The record for 'City' was not found", problem.Detail);
    }

    [Fact]
    public async Task TryFindByAsync_NonEqualityOperators_DegradeToGenericMessage()
    {
        // Arrange
        using var fixture = new CityFixture();

        // Act: "with Name 'Blumenau' was not found" would assert the opposite of what was queried.
        var byNotEqual = await fixture.Db.TryFindByAsync<City>(c => c.Name != "Blumenau" && c.StateId == 0);
        var byNotEqualNotFound = byNotEqual.NotFound(out var byNotEqualProblem);

        // Act: same for relational operators.
        var byGreaterThan = await fixture.Db.TryFindByAsync<City>(c => c.Id > 10_000);
        var byGreaterThanNotFound = byGreaterThan.NotFound(out var byGreaterThanProblem);

        // Assert
        Assert.True(byNotEqualNotFound);
        Assert.NotNull(byNotEqualProblem);
        Assert.Equal("The record for 'City' was not found", byNotEqualProblem.Detail);

        Assert.True(byGreaterThanNotFound);
        Assert.NotNull(byGreaterThanProblem);
        Assert.Equal("The record for 'City' was not found", byGreaterThanProblem.Detail);
    }

    [Fact]
    public async Task TryFindByAsync_CapturedPropertyOnValueSide_ProducesRichMessage_ButReadsGetterAgain()
    {
        // Arrange
        using var fixture = new CityFixture();
        var request = new SearchRequest("Nonexistent");

        // Act
        var entry = await fixture.Db.TryFindByAsync<City>(c => c.Name == request.Name);
        var notFound = entry.NotFound(out var problem);

        // Assert: reading captured properties is what makes `request.Name` yield a rich message;
        // the documented trade-off is that the getter runs once more than the query itself needed.
        Assert.True(notFound);
        Assert.NotNull(problem);
        Assert.Equal("The record of 'City' with Name 'Nonexistent' was not found", problem.Detail);
        Assert.True(request.NameReads > 1, $"expected the getter to be read again, got {request.NameReads}");
    }

    [Fact]
    public async Task TryFindByAsync_AmbiguousOrDeepMemberComparison_DegradesToGenericMessage()
    {
        // Arrange
        using var fixture = new CityFixture();

        // Act: both sides are direct members of the same parameter -> ambiguous, no criterion extracted.
        var byBothMembers = await fixture.Db.TryFindByAsync<State>(s => s.Name == s.Code);
        var byBothMembersNotFound = byBothMembers.NotFound(out var byBothMembersProblem);

        // Act: right side is a two-level deep chain, not a direct member of the parameter.
        var byDeepChain = await fixture.Db.TryFindByAsync<City>(c => c.State.Name == "Nonexistent");
        var byDeepChainNotFound = byDeepChain.NotFound(out var byDeepChainProblem);

        // Assert
        Assert.True(byBothMembersNotFound);
        Assert.NotNull(byBothMembersProblem);
        Assert.Equal("The record for 'State' was not found", byBothMembersProblem.Detail);

        Assert.True(byDeepChainNotFound);
        Assert.NotNull(byDeepChainProblem);
        Assert.Equal("The record for 'City' was not found", byDeepChainProblem.Detail);
    }

    [Fact]
    public async Task TryFindByAsync_MethodCallOnValueSide_DegradesToGenericMessage_WithoutReinvoking()
    {
        // Arrange
        using var fixture = new CityFixture();
        var callCount = 0;

        // a Func<> variable, not a local function: local functions can't appear in expression trees.
        Func<string, string> track = value =>
        {
            callCount++;
            return value;
        };

        // Act
        var entry = await fixture.Db.TryFindByAsync<City>(c => c.Name == track("Nonexistent"));
        var notFound = entry.NotFound(out var problem);

        // Assert
        Assert.True(notFound);
        Assert.NotNull(problem);
        Assert.Equal("The record for 'City' was not found", problem.Detail);
        Assert.Equal(1, callCount);
    }

    /// <summary>A captured filter object whose property getter counts its reads.</summary>
    private sealed class SearchRequest(string name)
    {
        public int NameReads { get; private set; }

        public string Name
        {
            get
            {
                NameReads++;
                return name;
            }
        }
    }
}
