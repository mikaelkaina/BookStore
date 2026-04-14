using BookStore.Domain.Common;
using BookStore.Domain.Enums;
using BookStore.Domain.ValueObjets;

namespace BookStore.Domain.Entities;

public sealed class Customer : Entity
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public Email Email { get; private set; }
    public string? Phone { get; private set; }
    public string? Document { get; private set; }
    public DateOnly? BirthDate { get; private set; }
    public CustomerRole Role { get; private set; }
    public bool IsActive { get; private set; }

    private readonly List<Address> _addresses = [];
    public IReadOnlyCollection<Address> Addresses => _addresses.AsReadOnly();

    private readonly List<Order> _oders = [];
    public IReadOnlyCollection<Order> Orders => _oders.AsReadOnly();

    public string FullNmae => $"{FirstName} {LastName}";

    private Customer() { }

    public Customer(string firstName, string lastName, Email email, string? phone, string? document, DateOnly? birthDate)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        Document = document;
        BirthDate = birthDate;
        Role = CustomerRole.Customer;
        IsActive = true;
    }

    public static Result<Customer> Create(string firstName, string lastName, string email, string? phone = null,
        string? document = null, DateOnly? birthDate =null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return Result.Failure<Customer>(Error.Validation(nameof(FirstName),"First name is required."));

        if (string.IsNullOrWhiteSpace(lastName))
            return Result.Failure<Customer>(Error.Validation(nameof(LastName), "Last name is required."));

        var emailResult = Email.Create(email);
        if (emailResult.IsFailure) 
            return Result.Failure<Customer>(emailResult.Error);
        
        if(document is not null & !IsValidCpf(document))
            return Result.Failure<Customer>(Error.Validation(nameof(Document), "CPF is invalid."));

        return Result.Success(new Customer(
            firstName.Trim(), lastName.Trim(), emailResult.Value, phone, document, birthDate));
    }

    public Result AddAddress(Address address) { _addresses.Add(address); SetUpdatedAt(); return Result.Success(); }

    public Result UpdateProfile(string firstName, string lastName, string? phone)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return Result.Failure(Error.Validation(nameof(FirstName), "First name is required."));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Phone = phone;
        SetUpdatedAt();
        return Result.Success();
    }

    public void PromoteToAdmin() { Role = CustomerRole.Admin; SetUpdatedAt(); }
    public void Deactivate() { IsActive = false; SetUpdatedAt(); }

    private static bool IsValidCpf(string cpf)
    {
        var clean = cpf.Replace(".", "").Replace("-", "");
        if (clean.Length != 11 || !clean.All(char.IsDigit)) return false;
        if (clean.Distinct().Count() == 1) return false;

        var sum = 0;
        for (var i = 0; i < 9; i++) sum += (clean[i] - '0') * (10 - i);
        var r = sum % 11;
        var d1 = r < 2 ? 0 : 11 - r;

        sum = 0;
        for (var i = 0; i < 10; i++) sum += (clean[i] - '0') * (11 - i);
        r = sum % 11;
        var d2 = r < 2 ? 0 : 11 - r;

        return clean[9] - '0' == d1 && clean[10] - '0' == d2;
    }

}

public static class CutomerErros
{
    public static Error NotFound(Guid id) => Error.NotFound(nameof(Customer), id);
    public static Error EmailAlreadyExixts(string email) =>
        new("Customer.Email.AlreadyExists", $"A customer with the email '{email}' already exists.");
}
