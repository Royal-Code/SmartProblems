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

public class Country
{
    private List<State> _states = [];

    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public virtual IReadOnlyList<State> States => _states;
    protected Country() { }
    public Country(string name, string code)
    {
        Name = name;
        Code = code;
    }
    public void AddState(State state)
    {
        _states.Add(state);
    }
    public void RemoveState(State state)
    {
        _states.Remove(state);
    }
    public State? GetStateById(int stateId)
    {
        return _states.FirstOrDefault(s => s.Id == stateId);
    }
}

public class State
{
    private List<City> _cities = [];

    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public int CountryId { get; private set; }
    public virtual Country Country { get; private set; } = null!;
    public virtual IReadOnlyList<City> Cities => _cities;
    protected State() { }
    public State(string name, string code, int countryId)
    {
        Name = name;
        Code = code;
        CountryId = countryId;
    }
    public void AddCity(City city)
    {
        _cities.Add(city);
    }
    public void RemoveCity(City city)
    {
        _cities.Remove(city);
    }
    public City? GetCityById(int cityId)
    {
        return _cities.FirstOrDefault(c => c.Id == cityId);
    }
}

public class City
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int StateId { get; private set; }
    public virtual State State { get; private set; } = null!;
    protected City() { }
    public City(string name, int stateId)
    {
        Name = name;
        StateId = stateId;
    }
}