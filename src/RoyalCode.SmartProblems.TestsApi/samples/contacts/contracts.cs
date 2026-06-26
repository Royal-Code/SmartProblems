using System.Diagnostics.CodeAnalysis;

namespace RoyalCode.SmartProblems.TestsApi.Apis;

public sealed class CreateContactRequest
{
	public required string Name { get; set; }

	public required string Email { get; set; }

	public string? Phone { get; set; }

	public bool HasProblems([NotNullWhen(true)] out Problems? problems)
	{
		Problems errors = [];

		if (string.IsNullOrWhiteSpace(Name))
			errors += Problems.InvalidParameter("Name is required.", nameof(Name));
		else if (Name.NormalizeName().Length > 160)
			errors += Problems.InvalidParameter("Name cannot be longer than 160 characters.", nameof(Name));

		if (string.IsNullOrWhiteSpace(Email))
		{
			errors += Problems.InvalidParameter("E-mail is required.", nameof(Email));
		}
		else
		{
			var email = Email.NormalizeEmail();
			if (!email.IsSimpleEmail())
				errors += Problems.InvalidParameter("E-mail format is invalid.", nameof(Email));
			else if (email.Length > 320)
				errors += Problems.InvalidParameter("E-mail cannot be longer than 320 characters.", nameof(Email));
		}

		var normalizedPhone = Phone.NormalizePhone();
		if (normalizedPhone is not null && normalizedPhone.Length > 30)
			errors += Problems.InvalidParameter("Phone cannot be longer than 30 characters.", nameof(Phone));

		if (errors.Count > 0)
		{
			problems = errors;
			return true;
		}

		problems = null;
		return false;
	}
}

public sealed class UpdateContactRequest
{
	public string? Name { get; set; }

	public string? Email { get; set; }

	public string? Phone { get; set; }

	public bool ClearPhone { get; set; }

	public bool HasProblems([NotNullWhen(true)] out Problems? problems)
	{
		Problems errors = [];

		if (Name is null && Email is null && Phone is null && !ClearPhone)
			errors += Problems.InvalidParameter("At least one field must be sent for update.", "request");

		if (Name is not null)
		{
			var normalizedName = Name.NormalizeName();
			if (normalizedName.Length == 0)
				errors += Problems.InvalidParameter("Name cannot be empty.", nameof(Name));
			else if (normalizedName.Length > 160)
				errors += Problems.InvalidParameter("Name cannot be longer than 160 characters.", nameof(Name));
		}

		if (Email is not null)
		{
			var email = Email.NormalizeEmail();
			if (!email.IsSimpleEmail())
				errors += Problems.InvalidParameter("E-mail format is invalid.", nameof(Email));
			else if (email.Length > 320)
				errors += Problems.InvalidParameter("E-mail cannot be longer than 320 characters.", nameof(Email));
		}

		var normalizedPhone = Phone.NormalizePhone();
		if (normalizedPhone is not null && normalizedPhone.Length > 30)
			errors += Problems.InvalidParameter("Phone cannot be longer than 30 characters.", nameof(Phone));

		if (Phone is not null && ClearPhone)
			errors += Problems.InvalidParameter("Use either Phone or ClearPhone.", nameof(ClearPhone));

		if (errors.Count > 0)
		{
			problems = errors;
			return true;
		}

		problems = null;
		return false;
	}
}

public sealed class ContactDetails
{
	public int Id { get; set; }

	public string Name { get; set; } = string.Empty;

	public string Email { get; set; } = string.Empty;

	public string? Phone { get; set; }

	public DateTimeOffset CreatedAt { get; set; }

	public DateTimeOffset? UpdatedAt { get; set; }
}
