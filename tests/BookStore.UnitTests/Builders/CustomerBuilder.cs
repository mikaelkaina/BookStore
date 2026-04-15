using Bogus;
using BookStore.Domain.Entities;
using FluentAssertions;

public class CustomerBuilder
{
    private readonly Faker _faker = new("pt_BR");

    private string _firstName;
    private string _lastName;
    private string _email;
    private string? _phone;
    private string _document;
    private DateOnly? _birthDate;

    public CustomerBuilder()
    {
        _firstName = _faker.Name.FirstName();
        _lastName = _faker.Name.LastName();
        _email = _faker.Internet.Email();
        _phone = _faker.Phone.PhoneNumber("(##) #####-####");
        _document = "12345678909";
        _birthDate = DateOnly.FromDateTime(_faker.Date.Past(30, DateTime.Now.AddYears(-18)));
    }

    public CustomerBuilder WithFirstName(string firstName) { _firstName = firstName; return this; }
    public CustomerBuilder WithLastName(string lastName) { _lastName = lastName; return this; }
    public CustomerBuilder WithEmail(string email) { _email = email; return this; }
    public CustomerBuilder WithPhone(string phone) { _phone = phone; return this; }
    public CustomerBuilder WithBirthDate(DateOnly birthDate) { _birthDate = birthDate; return this; }

    public CustomerBuilder WithDocument(string document)
    {
        _document = document ?? "12345678909";
        return this;
    }

    public Customer Build()
    {
        var result = Customer.Create(
            _firstName,
            _lastName,
            _email,
            _phone,
            _document,
            _birthDate
        );

        result.IsSuccess.Should().BeTrue("CustomerBuilder deve sempre gerar um cliente válido");

        return result.Value;
    }
}