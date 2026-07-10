using Microsoft.EntityFrameworkCore;
using RoyalCode.SmartProblems;

namespace RoyalCode.SmartProblems.Tests.EntityFramework;

public class EFOperationsTests
{
    private static TestDbContext CreateDbContext()
    {
        var db = new TestDbContext();
        db.InitializeDatabase();
        return db;
    }

    [Fact]
    public void AddTo_And_SaveChanges_Must_InsertEntity_WhenResultIsSuccess()
    {
        // Arrange
        using var db = CreateDbContext();
        Result<TestEntity> result = new TestEntity { Name = "Created" };

        // Act
        var saved = result
            .AddTo(db)
            .SaveChanges(db);
        var hasValue = saved.HasValue(out var entity);

        // Assert
        Assert.True(hasValue);
        Assert.NotNull(entity);
        Assert.NotEqual(0, entity.Id);
        Assert.Equal("Created", db.TestEntities.Single(e => e.Id == entity.Id).Name);
    }

    [Fact]
    public void AddTo_Must_NotTrackEntity_WhenResultHasProblems()
    {
        // Arrange
        using var db = CreateDbContext();
        Result<TestEntity> result = Problems.InvalidParameter("Invalid entity").AsResult<TestEntity>();

        // Act
        var added = result.AddTo(db);
        var hasProblems = added.HasProblems(out var problems);

        // Assert
        Assert.True(hasProblems);
        Assert.NotNull(problems);
        Assert.Empty(db.ChangeTracker.Entries<TestEntity>());
    }

    [Fact]
    public async Task AddToAsync_UsingResult_Must_AddEntity_WhenResultIsSuccess()
    {
        // Arrange
        using var db = CreateDbContext();
        Result<TestEntity> result = new TestEntity { Name = "Async Created" };

        // Act
        var added = await result.AddToAsync(db);
        var hasValue = added.HasValue(out var entity);

        // Assert
        Assert.True(hasValue);
        Assert.NotNull(entity);
        Assert.Equal(EntityState.Added, db.Entry(entity).State);
    }

    [Fact]
    public async Task AddToAsync_UsingTaskResult_Must_AddEntity_WhenResultIsSuccess()
    {
        // Arrange
        using var db = CreateDbContext();
        Task<Result<TestEntity>> result = Task.FromResult<Result<TestEntity>>(
            new TestEntity { Name = "Task Created" });

        // Act
        var added = await result.AddToAsync(db);
        var hasValue = added.HasValue(out var entity);

        // Assert
        Assert.True(hasValue);
        Assert.NotNull(entity);
        Assert.Equal(EntityState.Added, db.Entry(entity).State);
    }

    [Fact]
    public async Task AddToAsync_UsingValueTaskResult_Must_AddEntity_WhenResultIsSuccess()
    {
        // Arrange
        using var db = CreateDbContext();
        var result = ValueTask.FromResult<Result<TestEntity>>(
            new TestEntity { Name = "ValueTask Created" });

        // Act
        var added = await result.AddToAsync(db);
        var hasValue = added.HasValue(out var entity);

        // Assert
        Assert.True(hasValue);
        Assert.NotNull(entity);
        Assert.Equal(EntityState.Added, db.Entry(entity).State);
    }

    [Fact]
    public void SaveChanges_Must_NotPersistPendingChanges_WhenResultHasProblems()
    {
        // Arrange
        using var db = CreateDbContext();
        var entity = new TestEntity { Name = "Blocked" };
        db.TestEntities.Add(entity);
        Result result = Problems.InvalidParameter("Invalid state").AsResult();

        // Act
        var saved = result.SaveChanges(db);
        var hasProblems = saved.HasProblems(out var problems);

        // Assert
        Assert.True(hasProblems);
        Assert.NotNull(problems);
        Assert.Equal(EntityState.Added, db.Entry(entity).State);
        Assert.False(db.TestEntities.Any(e => e.Name == "Blocked"));
    }

    [Fact]
    public async Task SaveChangesAsync_UsingResult_Must_PersistPendingChanges_WhenResultIsSuccess()
    {
        // Arrange
        using var db = CreateDbContext();
        db.TestEntities.Add(new TestEntity { Name = "Async Saved" });
        var result = Result.Ok();

        // Act
        var saved = await result.SaveChangesAsync(db);

        // Assert
        Assert.True(saved.IsSuccess);
        Assert.True(await db.TestEntities.AnyAsync(e => e.Name == "Async Saved"));
    }

    [Fact]
    public async Task SaveChangesAsync_UsingTaskResult_Must_PersistPendingChanges_WhenResultIsSuccess()
    {
        // Arrange
        using var db = CreateDbContext();
        db.TestEntities.Add(new TestEntity { Name = "Task Saved" });
        Task<Result> result = Task.FromResult(Result.Ok());

        // Act
        var saved = await result.SaveChangesAsync(db);

        // Assert
        Assert.True(saved.IsSuccess);
        Assert.True(await db.TestEntities.AnyAsync(e => e.Name == "Task Saved"));
    }

    [Fact]
    public async Task SaveChangesAsync_UsingResultOfEntity_Must_PersistPendingChanges_WhenResultIsSuccess()
    {
        // Arrange
        using var db = CreateDbContext();
        var entity = new TestEntity { Name = "Entity Saved" };
        db.TestEntities.Add(entity);
        Result<TestEntity> result = entity;

        // Act
        var saved = await result.SaveChangesAsync(db);
        var hasValue = saved.HasValue(out var savedEntity);

        // Assert
        Assert.True(hasValue);
        Assert.Same(entity, savedEntity);
        Assert.True(await db.TestEntities.AnyAsync(e => e.Name == "Entity Saved"));
    }

    [Fact]
    public async Task RemoveFromAsync_And_SaveChangesAsync_Must_RemoveEntity_WhenFindResultIsFound()
    {
        // Arrange
        using var db = CreateDbContext();
        db.TestEntities.Add(new TestEntity { Name = "Remove Me" });
        db.SaveChanges();

        // Act
        var removed = await db.TestEntities
            .TryFindByAsync(e => e.Name == "Remove Me")
            .RemoveFromAsync(db, CancellationToken.None)
            .SaveChangesAsync(db);
        var hasValue = removed.HasValue(out var entity);

        // Assert
        Assert.True(hasValue);
        Assert.NotNull(entity);
        Assert.Equal("Remove Me", entity.Name);
        Assert.False(await db.TestEntities.AnyAsync(e => e.Name == "Remove Me"));
    }

    [Fact]
    public async Task RemoveFromAsync_Must_ReturnProblem_And_NotRemoveEntity_WhenFindResultIsNotFound()
    {
        // Arrange
        using var db = CreateDbContext();
        db.TestEntities.Add(new TestEntity { Name = "Keep Me" });
        db.SaveChanges();

        // Act
        var removed = await db.TestEntities
            .TryFindByAsync(e => e.Name == "Missing")
            .RemoveFromAsync(db, CancellationToken.None);
        var hasProblems = removed.HasProblems(out var problems);

        // Assert
        Assert.True(hasProblems);
        Assert.NotNull(problems);
        Assert.True(await db.TestEntities.AnyAsync(e => e.Name == "Keep Me"));
    }
}
