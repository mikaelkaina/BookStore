using BookStore.Domain.Entities;
using BookStore.Domain.Enums;
using BookStore.Domain.ValueObjects;
using FluentAssertions;

namespace BookStore.UnitTests.Entities;

public class CustomerTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var customer = new CustomerBuilder().Build();

        customer.Should().NotBeNull();
        customer.IsActive.Should().BeTrue();
        customer.Role.Should().Be(CustomerRole.Customer);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyFirstName_ShouldFail(string firstName)
    {
        var result = Customer.Create(firstName, "Silva", "email@teste.com", document: "12345678909");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("FirstName");
    }

    [Fact]
    public void Create_WithInvalidEmail_ShouldFail()
    {
        var result = Customer.Create("João", "Silva", "email-invalido", document: "12345678909");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Email");
    }

    [Fact]
    public void Create_WithInvalidCpf_ShouldFail()
    {
        var result = Customer.Create("João", "Silva", "joao@email.com",
            document: "11111111111");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Cpf");
    }

    [Fact]
    public void Create_WithNullCpf_ShouldFail()
    {
        var result = Customer.Create("João", "Silva", "email@email.com", document: null);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Cpf");
    }

    [Fact]
    public void AddAddress_ShouldIncreaseAddressCount()
    {
        var customer = new CustomerBuilder().Build();

        var addressResult = Address.Create(
            "Rua das Flores", "42", null,
            "Centro", "Recife", "PE", "50010000");

        addressResult.IsSuccess.Should().BeTrue();

        customer.AddAddress(addressResult.Value);

        customer.Addresses.Should().HaveCount(1);
        customer.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void UpdateProfile_WithValidData_ShouldSucceed()
    {
        var customer = new CustomerBuilder().Build();

        var result = customer.UpdateProfile("Carlos", "Mendes", "(81) 99999-9999");

        result.IsSuccess.Should().BeTrue();
        customer.FirstName.Should().Be("Carlos");
        customer.LastName.Should().Be("Mendes");
        customer.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void UpdateProfile_WithEmptyFirstName_ShouldFail()
    {
        var customer = new CustomerBuilder().Build();

        var result = customer.UpdateProfile("", "Silva", null);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void PromoteToAdmin_ShouldChangeRole()
    {
        var customer = new CustomerBuilder().Build();

        customer.PromoteToAdmin();

        customer.Role.Should().Be(CustomerRole.Admin);
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveFalse()
    {
        var customer = new CustomerBuilder().Build();

        customer.Deactivate();

        customer.IsActive.Should().BeFalse();
        customer.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void FullName_ShouldCombineFirstAndLastName()
    {
        var result = Customer.Create(
            "Maria",
            "Oliveira",
            "maria@email.com",
            document: "12345678909");

        result.IsSuccess.Should().BeTrue();
        result.Value.FullName.Should().Be("Maria Oliveira");
    }

    [Fact]
    public void FullName_ShouldTrimNames()
    {
        var result = Customer.Create(
            "  Maria  ",
            "  Oliveira  ",
            "email@email.com",
            document: "12345678909");

        result.IsSuccess.Should().BeTrue();
        result.Value.FullName.Should().Be("Maria Oliveira");
    }
}