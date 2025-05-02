using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RoyalCode.SmartProblems.Entities;
using System.ComponentModel;

namespace RoyalCode.SmartProblems.Tests.EntityFramework;

public class TryFindTests
{
    private static TestDbContext CreateDbContext()
    {
        var db = new TestDbContext();
        db.InitializeDatabase();
        db.TestEntities.Add(new TestEntity { Id = 1, Name = "Test1" });
        db.TestEntities.Add(new TestEntity { Id = 2, Name = "Test2" });
        db.TestEntities.Add(new TestEntity { Id = 3, Name = "Test3" });
        db.SaveChanges();

        return db;
    }

    [Fact]
    public async Task TryFindAsync_UsingDb_Must_ReturnEntity()
    {
        // Arrange
        using var db = CreateDbContext();

        // Act
        Id<TestEntity, int> id = 2;
        var entry = await db.TryFindAsync(id);
        var notFound = entry.NotFound(out var problems);

        // Assert
        Assert.False(notFound);
        Assert.Null(problems);

        Assert.NotNull(entry.Entity);
        Assert.Equal(id, entry.Entity.Id);
        Assert.Equal("Test2", entry.Entity.Name);
    }

    [Fact]
    public async Task TryFindAsync_UsingDb_Must_NotFound()
    {
        // Arrange
        using var db = CreateDbContext();

        // Act
        Id<TestEntity, int> id = 4;
        var entry = await db.TryFindAsync(id);
        var notFound = entry.NotFound(out var problem);

        // Assert
        Assert.True(notFound);
        Assert.NotNull(problem);
        Assert.Null(entry.Entity);

        Assert.Equal("The record of 'The Entity for Tests' with id '4' was not found", problem.Detail);
        Assert.NotNull(problem.Extensions);
        
        problem.Extensions.TryGetValue("id", out var idProblem);
        Assert.NotNull(idProblem);
        Assert.Equal(4, (int)idProblem);
        
        problem.Extensions.TryGetValue("entity", out var entityProblem);
        Assert.NotNull(entityProblem);
        Assert.Equal("TestEntity", (string)entityProblem);
    }

    [Fact]
    public async Task TryFindAsync_UsingSet_Must_ReturnEntity()
    {
        // Arrange
        using var db = CreateDbContext();

        // Act
        Id<TestEntity, int> id = 2;
        var entry = await db.TestEntities.TryFindAsync(id);
        var notFound = entry.NotFound(out var problems);

        // Assert
        Assert.False(notFound);
        Assert.Null(problems);

        Assert.NotNull(entry.Entity);
        Assert.Equal(id, entry.Entity.Id);
        Assert.Equal("Test2", entry.Entity.Name);
    }

    [Fact]
    public async Task TryFindAsync_UsingSet_Must_NotFound()
    {
        // Arrange
        using var db = CreateDbContext();

        // Act
        Id<TestEntity, int> id = 4;
        var entry = await db.TestEntities.TryFindAsync(id);
        var notFound = entry.NotFound(out var problem);
        
        // Assert
        Assert.True(notFound);
        Assert.NotNull(problem);
        Assert.Null(entry.Entity);

        Assert.Equal("The record of 'The Entity for Tests' with id '4' was not found", problem.Detail);

        

        Assert.NotNull(problem.Extensions);

        problem.Extensions.TryGetValue("id", out var idProblem);
        Assert.NotNull(idProblem);
        Assert.Equal(4, (int)idProblem);

        problem.Extensions.TryGetValue("entity", out var entityProblem);
        Assert.NotNull(entityProblem);
        Assert.Equal("TestEntity", (string)entityProblem);
    }

    [Fact]
    public async Task TryFindByAsync_UsingDb_Must_ReturnEntity()
    {
        // Arrange
        using var db = CreateDbContext();

        // Act
        var entry = await db.TryFindByAsync<TestEntity>(e => e.Name == "Test2");
        var notFound = entry.NotFound(out var problems);

        // Assert
        Assert.False(notFound);
        Assert.Null(problems);

        Assert.NotNull(entry.Entity);
        Assert.Equal("Test2", entry.Entity.Name);
    }

    [Fact]
    public async Task TryFindByAsync_UsingDb_Must_NotFound()
    {
        // Arrange
        using var db = CreateDbContext();

        // Act
        var entry = await db.TryFindByAsync<TestEntity>(e => e.Name == "Test4");
        var notFound = entry.NotFound(out var problem);

        // Assert
        Assert.True(notFound);
        Assert.NotNull(problem);
        Assert.Null(entry.Entity);

        Assert.NotNull(problem.Extensions);

        Assert.Equal("The record of 'The Entity for Tests' with Name 'Test4' was not found", problem.Detail);

        problem.Extensions.TryGetValue("Name", out var nameProblem);
        Assert.NotNull(nameProblem);
        Assert.Equal("Test4", (string)nameProblem);

        problem.Extensions.TryGetValue("entity", out var entityProblem);
        Assert.NotNull(entityProblem);
        Assert.Equal("TestEntity", (string)entityProblem);
    }

    [Fact]
    public async Task TryFindByAsync_UsingDb_AndVariableFilter_Must_NotFound()
    {
        // Arrange
        using var db = CreateDbContext();

        string filter = "Test4";

        // Act
        var entry = await db.TryFindByAsync<TestEntity>(e => e.Name == filter);
        var notFound = entry.NotFound(out var problem);

        // Assert
        Assert.True(notFound);
        Assert.NotNull(problem);
        Assert.Null(entry.Entity);

        Assert.NotNull(problem.Extensions);

        Assert.Equal("The record of 'The Entity for Tests' with Name 'Test4' was not found", problem.Detail);

        problem.Extensions.TryGetValue("Name", out var nameProblem);
        Assert.NotNull(nameProblem);
        Assert.Equal("Test4", (string)nameProblem);

        problem.Extensions.TryGetValue("entity", out var entityProblem);
        Assert.NotNull(entityProblem);
        Assert.Equal("TestEntity", (string)entityProblem);
    }

    [Fact]
    public async Task TryFindByAsync_UsingDb_AndVariableClosureFilter_Must_NotFound()
    {
        // Arrange
        using var db = CreateDbContext();

        var filter = new Tuple<int, string>(4, "Test4");

        // Act
        var entry = await db.TryFindByAsync<TestEntity>(e => e.Name == filter.Item2);
        var notFound = entry.NotFound(out var problem);

        // Assert
        Assert.True(notFound);
        Assert.NotNull(problem);
        Assert.Null(entry.Entity);

        Assert.NotNull(problem.Extensions);

        Assert.Equal("The record of 'The Entity for Tests' with Name 'Test4' was not found", problem.Detail);

        problem.Extensions.TryGetValue("Name", out var nameProblem);
        Assert.NotNull(nameProblem);
        Assert.Equal("Test4", (string)nameProblem);

        problem.Extensions.TryGetValue("entity", out var entityProblem);
        Assert.NotNull(entityProblem);
        Assert.Equal("TestEntity", (string)entityProblem);
    }

    [Fact]
    public async Task TryFindByAsync_UsingSet_Must_ReturnEntity()
    {
        // Arrange
        using var db = CreateDbContext();

        // Act
        var entry = await db.TestEntities.TryFindByAsync(e => e.Name == "Test2");
        var notFound = entry.NotFound(out var problems);

        // Assert
        Assert.False(notFound);
        Assert.Null(problems);
        Assert.NotNull(entry.Entity);
        Assert.Equal("Test2", entry.Entity.Name);
    }

    [Fact]
    public async Task TryFindByAsync_UsingSet_Must_NotFound()
    {
        // Arrange
        using var db = CreateDbContext();

        // Act
        var entry = await db.TestEntities.TryFindByAsync(e => e.Name == "Test4");
        var notFound = entry.NotFound(out var problem);

        // Assert
        Assert.True(notFound);
        Assert.NotNull(problem);
        Assert.Null(entry.Entity);

        Assert.NotNull(problem.Extensions);

        Assert.Equal("The record of 'The Entity for Tests' with Name 'Test4' was not found", problem.Detail);

        problem.Extensions.TryGetValue("Name", out var nameProblem);
        Assert.NotNull(nameProblem);
        Assert.Equal("Test4", (string)nameProblem);

        problem.Extensions.TryGetValue("entity", out var entityProblem);
        Assert.NotNull(entityProblem);
        Assert.Equal("TestEntity", (string)entityProblem);
    }

    [Fact]
    public async Task TryFindByAsync_UsingSet_AndVariableFilter_Must_NotFound()
    {
        // Arrange
        using var db = CreateDbContext();

        string filter = "Test4";

        // Act
        var entry = await db.TestEntities.TryFindByAsync(e => e.Name == filter);
        var notFound = entry.NotFound(out var problem);

        // Assert
        Assert.True(notFound);
        Assert.NotNull(problem);
        Assert.Null(entry.Entity);

        Assert.NotNull(problem.Extensions);

        Assert.Equal("The record of 'The Entity for Tests' with Name 'Test4' was not found", problem.Detail);

        problem.Extensions.TryGetValue("Name", out var nameProblem);
        Assert.NotNull(nameProblem);
        Assert.Equal("Test4", (string)nameProblem);

        problem.Extensions.TryGetValue("entity", out var entityProblem);
        Assert.NotNull(entityProblem);
        Assert.Equal("TestEntity", (string)entityProblem);
    }

    [Fact]
    public async Task TryFindByAsync_UsingSet_AndVariableClosureFilter_Must_NotFound()
    {
        // Arrange
        using var db = CreateDbContext();

        var filter = new Tuple<int, string>(4, "Test4");

        // Act
        var entry = await db.TestEntities.TryFindByAsync(e => e.Name == filter.Item2);
        var notFound = entry.NotFound(out var problem);

        // Assert
        Assert.True(notFound);
        Assert.NotNull(problem);
        Assert.Null(entry.Entity);

        Assert.NotNull(problem.Extensions);

        Assert.Equal("The record of 'The Entity for Tests' with Name 'Test4' was not found", problem.Detail);

        problem.Extensions.TryGetValue("Name", out var nameProblem);
        Assert.NotNull(nameProblem);
        Assert.Equal("Test4", (string)nameProblem);

        problem.Extensions.TryGetValue("entity", out var entityProblem);
        Assert.NotNull(entityProblem);
        Assert.Equal("TestEntity", (string)entityProblem);
    }

    [Fact]
    public async Task TryFindByAsync_UsingDbAndNames_Must_ReturnEntity()
    {
        // Arrange
        using var db = CreateDbContext();

        // Act
        var entry = await db.TryFindByAsync<TestEntity>(e => e.Name == "Test2", "Name", "name", "Test2");
        var notFound = entry.NotFound(out var problem);

        // Assert
        Assert.False(notFound);
        Assert.Null(problem);

        Assert.NotNull(entry.Entity);
        Assert.Equal("Test2", entry.Entity.Name);
    }

    [Fact]
    public async Task TryFindByAsync_UsingDbAndNames_Must_NotFound()
    {
        // Arrange
        using var db = CreateDbContext();

        // Act
        var entry = await db.TryFindByAsync<TestEntity>(e => e.Name == "Test4", "Name", "name", "Test4");
        var notFound = entry.NotFound(out var problem);

        // Assert
        Assert.True(notFound);
        Assert.NotNull(problem);
        Assert.Null(entry.Entity);

        Assert.NotNull(problem.Extensions);

        Assert.Equal("The record of 'The Entity for Tests' with Name 'Test4' was not found", problem.Detail);

        problem.Extensions.TryGetValue("name", out var nameProblem);
        Assert.NotNull(nameProblem);
        Assert.Equal("Test4", (string)nameProblem);

        problem.Extensions.TryGetValue("entity", out var entityProblem);
        Assert.NotNull(entityProblem);
        Assert.Equal("TestEntity", (string)entityProblem);
    }

    [Fact]
    public async Task TryFindByAsync_UsingSetAndNames_Must_ReturnEntity()
    {
        // Arrange
        using var db = CreateDbContext();

        // Act
        var entry = await db.TestEntities.TryFindByAsync(e => e.Name == "Test2", "Name", "name", "Test2");
        var notFound = entry.NotFound(out var problems);

        // Assert
        Assert.False(notFound);
        Assert.Null(problems);
        Assert.NotNull(entry.Entity);
        Assert.Equal("Test2", entry.Entity.Name);
    }

    [Fact]
    public async Task TryFindByAsync_UsingSetAndNames_Must_NotFound()
    {
        // Arrange
        using var db = CreateDbContext();

        // Act
        var entry = await db.TestEntities.TryFindByAsync(e => e.Name == "Test4", "Name", "name", "Test4");
        var notFound = entry.NotFound(out var problem);

        // Assert
        Assert.True(notFound);
        Assert.NotNull(problem);
        Assert.Null(entry.Entity);

        Assert.NotNull(problem.Extensions);

        Assert.Equal("The record of 'The Entity for Tests' with Name 'Test4' was not found", problem.Detail);

        problem.Extensions.TryGetValue("name", out var nameProblem);
        Assert.NotNull(nameProblem);
        Assert.Equal("Test4", (string)nameProblem);

        problem.Extensions.TryGetValue("entity", out var entityProblem);
        Assert.NotNull(entityProblem);
        Assert.Equal("TestEntity", (string)entityProblem);
    }

    [Fact]
    public async Task TryFindByAsync_PropertySelector_UsingDb_Must_ReturnEntity()
    {
        // Arrange
        using var db = CreateDbContext();

        // Act
        var entry = await db.TryFindByAsync<TestEntity, string>(e => e.Name, "Test2");
        var notFound = entry.NotFound(out var problems);
        
        // Assert
        Assert.False(notFound);
        Assert.Null(problems);
        Assert.NotNull(entry.Entity);
        Assert.Equal("Test2", entry.Entity.Name);
    }

    [Fact]
    public async Task TryFindByAsync_PropertySelector_UsingDb_Must_NotFound()
    {
        // Arrange
        using var db = CreateDbContext();

        // Act
        var entry = await db.TryFindByAsync<TestEntity, string>(e => e.Name, "Test4");
        var notFound = entry.NotFound(out var problem);

        // Assert
        Assert.True(notFound);
        Assert.NotNull(problem);
        Assert.Null(entry.Entity);

        Assert.NotNull(problem.Extensions);

        Assert.Equal("The record of 'The Entity for Tests' with Name 'Test4' was not found", problem.Detail);

        problem.Extensions.TryGetValue("Name", out var nameProblem);
        Assert.NotNull(nameProblem);
        Assert.Equal("Test4", (string)nameProblem);

        problem.Extensions.TryGetValue("entity", out var entityProblem);
        Assert.NotNull(entityProblem);
        Assert.Equal("TestEntity", (string)entityProblem);
    }

    [Fact]
    public async Task TryFindByAsync_PropertySelector_UsingSet_Must_ReturnEntity()
    {
        // Arrange
        using var db = CreateDbContext();

        // Act
        var entry = await db.TestEntities.TryFindByAsync(e => e.Name, "Test2");
        var notFound = entry.NotFound(out var problems);
        
        // Assert
        Assert.False(notFound);
        Assert.Null(problems);
        Assert.NotNull(entry.Entity);
        Assert.Equal("Test2", entry.Entity.Name);
    }

    [Fact]
    public async Task TryFindByAsync_PropertySelector_UsingSet_Must_NotFound()
    {
        // Arrange
        using var db = CreateDbContext();

        // Act
        var entry = await db.TestEntities.TryFindByAsync(e => e.Name, "Test4");
        var notFound = entry.NotFound(out var problem);

        // Assert
        Assert.True(notFound);
        Assert.NotNull(problem);
        Assert.Null(entry.Entity);

        Assert.NotNull(problem.Extensions);

        Assert.Equal("The record of 'The Entity for Tests' with Name 'Test4' was not found", problem.Detail);

        problem.Extensions.TryGetValue("Name", out var nameProblem);
        Assert.NotNull(nameProblem);
        Assert.Equal("Test4", (string)nameProblem);

        problem.Extensions.TryGetValue("entity", out var entityProblem);
        Assert.NotNull(entityProblem);
        Assert.Equal("TestEntity", (string)entityProblem);
    }
}

[DisplayName("The Entity for Tests")]
internal class TestEntity
{
    public int Id { get; set; }

    [DisplayName("Name")]
    public string Name { get; set; }
}

internal class TestDbContext : DbContext
{
    private readonly SqliteConnection connection;

    public TestDbContext()
    {
        connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite(connection);
        }

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var e = modelBuilder.Entity<TestEntity>();
        e.ToTable("TestEntities");
        e.HasKey(e => e.Id);
        e.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
        e.Property(e => e.Name).HasColumnName("name").IsRequired().HasMaxLength(100);

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<TestEntity> TestEntities { get; set; }

    public void InitializeDatabase()
    {
        Database.EnsureCreated(); // Garante que as tabelas sejam criadas
    }

    public override void Dispose()
    {
        base.Dispose();
        connection.Dispose(); // Fecha a conexão ao liberar o contexto
    }
}