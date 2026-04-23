using BookStore.Domain.Common;
using BookStore.Domain.Enums;
using BookStore.Domain.ValueObjects;

namespace BookStore.Domain.Entities;

public sealed class Customer : Entity
{
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public string? Phone { get; private set; }
    public Cpf Document { get; private set; } = null!;
    public DateOnly? BirthDate { get; private set; }
    public CustomerRole Role { get; private set; }
    public bool IsActive { get; private set; }

    private readonly List<Address> _addresses = [];
    public IReadOnlyCollection<Address> Addresses => _addresses.AsReadOnly();

    private readonly List<Order> _orders = [];
    public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();

    public string FullName => $"{FirstName} {LastName}";

    private Customer() { }

    private Customer(string firstName, string lastName, Email email, string? phone, Cpf document, DateOnly? birthDate)
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

    public static Result<Customer> Create(string firstName, string lastName, string email,
    string? phone = null, string? document = null, DateOnly? birthDate = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return Result.Failure<Customer>(
                Error.Validation(nameof(FirstName), "First name is required."));

        if (string.IsNullOrWhiteSpace(lastName))
            return Result.Failure<Customer>(
                Error.Validation(nameof(LastName), "Last name is required."));

        if (string.IsNullOrWhiteSpace(document))
            return Result.Failure<Customer>(
                Error.Validation(nameof(Document), "CPF is required."));

        var emailResult = Email.Create(email);
        if (emailResult.IsFailure)
            return Result.Failure<Customer>(emailResult.Error);

        var cpfResult = Cpf.Create(document);
        if (cpfResult.IsFailure)
            return Result.Failure<Customer>(cpfResult.Error);

        return Result.Success(new Customer(
            firstName.Trim(),
            lastName.Trim(),
            emailResult.Value,
            phone,
            cpfResult.Value,
            birthDate
        ));
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
}

public static class CustomerErrors
{
    public static Error NotFound(Guid id) => Error.NotFound(nameof(Customer), id);
    public static Error EmailAlreadyExixts(string email) =>
        new("Customer.Email.AlreadyExists", $"A customer with the email '{email}' already exists.");
}
