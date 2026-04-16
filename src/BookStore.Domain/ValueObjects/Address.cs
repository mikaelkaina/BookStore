using BookStore.Domain.Common;

namespace BookStore.Domain.ValueObjects;

public sealed class Address : ValueObject
{
    public string Street { get; }
    public string Number { get; }
    public string? Complement { get; }
    public string Neighborhood { get; }
    public string City { get; } 
    public string State { get; }
    public string ZipCode { get; }
    public string Country { get; }

    private Address(string street, string number, string? complement, string neighborhood, string city, string state, string zipCode, string country)
    {
        Street = street;
        Number = number;
        Complement = complement;
        Neighborhood = neighborhood;
        City = city;
        State = state;
        ZipCode = zipCode;
        Country = country;
    }

    public static Result<Address> Create(string street, string number, string? complement, string neighborhood, string city, string state, string zipCode, string country = "Brazil")
    {
        if (string.IsNullOrWhiteSpace(street))
            return Result.Failure<Address>(Error.Validation(nameof(Street),"Street is required."));

        if (string.IsNullOrWhiteSpace(number))
            return Result.Failure<Address>(Error.Validation(nameof(Number), "Number is required."));

        if (string.IsNullOrWhiteSpace(neighborhood))
            return Result.Failure<Address>(Error.Validation(nameof(Neighborhood), "Neighborhood is required."));

        if (string.IsNullOrWhiteSpace(city))
            return Result.Failure<Address>(Error.Validation(nameof(City), "City is required."));

        if (string.IsNullOrWhiteSpace(state) || state.Length != 2)
            return Result.Failure<Address>(Error.Validation(nameof(State), "State is required."));

        var cleanZip = zipCode?.Replace("-", "").Replace(".", "");
        if (string.IsNullOrWhiteSpace(cleanZip) || cleanZip.Length != 8 || !cleanZip.All(char.IsDigit))
            return Result.Failure<Address>(Error.Validation(nameof(ZipCode), "ZipCode must be 8 digits."));

        return Result.Success(new Address(
            street, number, complement, neighborhood,
            city, state.ToUpperInvariant(), cleanZip, country
        ));
    }

    public string FormattedZipCode => $"{ZipCode[..5]}-{ZipCode[5..]}";

    public override string ToString() =>
        $"{Street}, {Number}{(Complement is not null ? $", {Complement}" : "")}, {Neighborhood}, {City}/{State}, {FormattedZipCode}";

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return Number;
        yield return Complement ?? string.Empty;
        yield return Neighborhood;
        yield return City;
        yield return State;
        yield return ZipCode;
        yield return Country;
    }
}
