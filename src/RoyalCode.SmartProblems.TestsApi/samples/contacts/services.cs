using Microsoft.EntityFrameworkCore;
using RoyalCode.SmartProblems.Entities;

namespace RoyalCode.SmartProblems.TestsApi.Apis;

public sealed class ContactsService(ContactsDbContext db)
{
	public async Task<Result<Contact>> CreateAsync(CreateContactRequest create, CancellationToken ct = default)
	{
		if (create.HasProblems(out var problems))
			return problems;

		var normalizedEmail = create.Email.NormalizeEmail();

		var exists = await db.Contacts
			.AnyAsync(c => c.Email == normalizedEmail, ct);

		if (exists)
		{
			return Problems.InvalidState("A contact with this e-mail already exists.")
				.With("Email", normalizedEmail);
		}

		var contact = new Contact(create.Name, normalizedEmail, create.Phone);

		db.Contacts.Add(contact);
		await db.SaveChangesAsync(ct);

		return contact;
	}

	public async Task<Result<Contact>> GetByIdAsync(int id, CancellationToken ct = default)
	{
		return await FindByIdAsync(id, ct);
	}

	public async Task<Result<Contact>> UpdateAsync(int id, UpdateContactRequest update, CancellationToken ct = default)
	{
		if (update.HasProblems(out var problems))
			return problems;

		var found = await FindByIdAsync(id, ct);

		if (found.HasProblems(out var findProblems))
			return findProblems;

		found.EnsureHasValue(out var contact);

		if (update.Email is not null)
		{
			var normalizedEmail = update.Email.NormalizeEmail();

			if (!string.Equals(contact.Email, normalizedEmail, StringComparison.OrdinalIgnoreCase))
			{
				var exists = await db.Contacts
					.AnyAsync(c => c.Id != contact.Id && c.Email == normalizedEmail, ct);

				if (exists)
				{
					return Problems.InvalidState("A contact with this e-mail already exists.")
						.With("Email", normalizedEmail)
						.With("Id", id);
				}

				contact.ChangeEmail(normalizedEmail);
			}
		}

		if (update.Name is not null)
			contact.ChangeName(update.Name);

		if (update.ClearPhone)
			contact.ChangePhone(null);
		else if (update.Phone is not null)
			contact.ChangePhone(update.Phone);

		contact.Touch();
		await db.SaveChangesAsync(ct);

		return contact;
	}

	public async Task<Result> DeleteAsync(int id, CancellationToken ct = default)
	{
		var found = await FindByIdAsync(id, ct);

		if (found.HasProblems(out var findProblems))
			return findProblems;

		found.EnsureHasValue(out var contact);

		db.Contacts.Remove(contact);
		await db.SaveChangesAsync(ct);

		return Result.Ok();
	}

	private async Task<Result<Contact>> FindByIdAsync(int id, CancellationToken ct)
	{
		if (id <= 0)
		{
			return Problems.InvalidParameter("Id must be greater than zero.", nameof(id))
				.With("Id", id);
		}

		Id<Contact, int> contactId = id;
		FindResult<Contact, int> find = await db.TryFindAsync(contactId, ct);

		return find.ToResult();
	}
}
