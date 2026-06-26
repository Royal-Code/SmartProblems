namespace RoyalCode.SmartProblems.TestsApi.Apis;

public static class ContactsExtensions
{
	public static string NormalizeName(this string name)
	{
		ArgumentNullException.ThrowIfNull(name);
		return name.Trim();
	}

	public static string NormalizeEmail(this string email)
	{
		ArgumentNullException.ThrowIfNull(email);
		return email.Trim().ToLowerInvariant();
	}

	public static string? NormalizePhone(this string? phone)
	{
		if (string.IsNullOrWhiteSpace(phone))
			return null;

		return phone.Trim();
	}

	public static bool IsSimpleEmail(this string email)
	{
		ArgumentNullException.ThrowIfNull(email);
		var at = email.IndexOf('@');
		return at > 0 && at < email.Length - 1;
	}

	public static ContactDetails ToContactDetails(this Contact contact)
	{
		ArgumentNullException.ThrowIfNull(contact);

		return new ContactDetails
		{
			Id = contact.Id,
			Name = contact.Name,
			Email = contact.Email,
			Phone = contact.Phone,
			CreatedAt = contact.CreatedAt,
			UpdatedAt = contact.UpdatedAt
		};
	}
}
