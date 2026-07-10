using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace RoyalCode.SmartProblems.TestsApi.Apis;

public sealed class ContactsDbContext(DbContextOptions<ContactsDbContext> options) : DbContext(options)
{
	public DbSet<Contact> Contacts => Set<Contact>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Contact>(builder =>
		{
			builder.ToTable("contacts");

			builder.HasKey(c => c.Id);
			builder.Property(c => c.Id).ValueGeneratedOnAdd();

			builder.Property(c => c.Name)
				.HasMaxLength(160)
				.IsRequired();

			builder.Property(c => c.Email)
				.HasMaxLength(320)
				.IsRequired();

			builder.Property(c => c.Phone)
				.HasMaxLength(30);

			builder.Property(c => c.CreatedAt)
				.IsRequired();

			builder.HasIndex(c => c.Email)
				.IsUnique();
		});

		modelBuilder.Entity<Country>(builder =>
		{
			builder.ToTable("countries");

			builder.HasKey(c => c.Id);
			builder.Property(c => c.Id).ValueGeneratedOnAdd();

			builder.Property(c => c.Name)
				.HasMaxLength(100)
				.IsRequired();

			builder.Property(c => c.Code)
				.HasMaxLength(100)
				.IsRequired();
		});

		modelBuilder.Entity<Country>(builder =>
		{
			builder.ToTable("countries");

			builder.HasKey(c => c.Id);
			builder.Property(c => c.Id).ValueGeneratedOnAdd();

			builder.HasMany(c => c.States)
				.WithOne(s => s.Country)
				.HasForeignKey(s => s.CountryId)
				.IsRequired();
		});

		modelBuilder.Entity<State>(builder =>
		{
			builder.ToTable("states");

			builder.HasKey(s => s.Id);
			builder.Property(s => s.Id).ValueGeneratedOnAdd();

			builder.Property(s => s.Name)
				.HasMaxLength(100)
				.IsRequired();

			builder.Property(s => s.Code)
				.HasMaxLength(100)
				.IsRequired();

			builder.HasMany(s => s.Cities)
				.WithOne(c => c.State)
				.HasForeignKey(c => c.StateId)
				.IsRequired();
		});

		modelBuilder.Entity<City>(builder =>
		{
			builder.ToTable("cities");

			builder.HasKey(c => c.Id);
			builder.Property(c => c.Id).ValueGeneratedOnAdd();

			builder.Property(c => c.Name)
				.HasMaxLength(100)
				.IsRequired();
		});
	}
}

public static class ContactsEfCoreConfiguration
{
	private const string ConnectionString = "Data Source=:memory:;Cache=Shared";

	public static IServiceCollection AddContactsPersistence(this IServiceCollection services)
	{
		services.AddSingleton(_ =>
		{
			var connection = new SqliteConnection(ConnectionString);
			connection.Open();
			return connection;
		});

		services.AddDbContext<ContactsDbContext>((provider, options) =>
		{
			var connection = provider.GetRequiredService<SqliteConnection>();
			options.UseSqlite(connection);
		});

		return services;
	}

	public static WebApplication EnsureContactsDatabase(this WebApplication app)
	{
		using var scope = app.Services.CreateScope();
		var db = scope.ServiceProvider.GetRequiredService<ContactsDbContext>();
		db.Database.EnsureCreated();
		return app;
	}
}
