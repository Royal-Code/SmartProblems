using RoyalCode.SmartProblems.HttpResults;

namespace RoyalCode.SmartProblems.TestsApi.Apis;

public static class ContactsApi
{
	public static IServiceCollection AddContactsSample(this IServiceCollection services)
	{
		services.AddContactsPersistence();
		services.AddScoped<ContactsService>();
		return services;
	}

	public static IEndpointRouteBuilder MapContactsApi(this IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("/api/contacts").WithTags("Contacts API");

		group.MapPost("/", CreateContact);
		group.MapGet("/{id:int}", GetContactById);
		group.MapPatch("/{id:int}", UpdateContact);
		group.MapDelete("/{id:int}", DeleteContact);

		return app;
	}

	private static async Task<CreatedMatch<ContactDetails>> CreateContact(
		CreateContactRequest create,
		ContactsService service,
		CancellationToken ct)
	{
		return (await service.CreateAsync(create, ct))
			.Map(contact => contact.ToContactDetails())
			.CreatedMatch(contact => $"/api/contacts/{contact.Id}");
	}

	private static async Task<OkMatch<ContactDetails>> GetContactById(
		int id,
		ContactsService service,
		CancellationToken ct)
	{
		return (await service.GetByIdAsync(id, ct))
			.Map(contact => contact.ToContactDetails());
	}

	private static async Task<OkMatch<ContactDetails>> UpdateContact(
		int id,
		UpdateContactRequest update,
		ContactsService service,
		CancellationToken ct)
	{
		return (await service.UpdateAsync(id, update, ct))
			.Map(contact => contact.ToContactDetails());
	}

	private static async Task<NoContentMatch> DeleteContact(
		int id,
		ContactsService service,
		CancellationToken ct)
	{
		return await service.DeleteAsync(id, ct);
	}
}
