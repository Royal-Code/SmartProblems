namespace RoyalCode.SmartProblems.TestsApi.Apis;

public sealed class Contact
{
	public int Id { get; private set; }

	public string Name { get; private set; } = string.Empty;

	public string Email { get; private set; } = string.Empty;

	public string? Phone { get; private set; }

	public DateTimeOffset CreatedAt { get; private set; }

	public DateTimeOffset? UpdatedAt { get; private set; }

	private Contact() { }

	public Contact(string name, string email, string? phone)
	{
		Name = name.NormalizeName();
		Email = email.NormalizeEmail();
		Phone = phone.NormalizePhone();
		CreatedAt = DateTimeOffset.UtcNow;
	}

	public void ChangeName(string name)
	{
		Name = name.NormalizeName();
	}

	public void ChangeEmail(string email)
	{
		Email = email.NormalizeEmail();
	}

	public void ChangePhone(string? phone)
	{
		Phone = phone.NormalizePhone();
	}

	public void Touch()
	{
		UpdatedAt = DateTimeOffset.UtcNow;
	}
}
