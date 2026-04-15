using Bogus;
using BookStore.Domain.ValueObjects;
using FluentAssertions;

namespace BookStore.UnitTests.ValueObjects;

public class AddressTests
{
    private readonly Faker _faker = new("pt_BR");

    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var result = Address.Create(
            "Rua das Flores", "100", "Apto 12",
            "Centro", "São Paulo", "SP", "01310100");

        result.IsSuccess.Should().BeTrue();
        result.Value.City.Should().Be("São Paulo");
        result.Value.State.Should().Be("SP");
    }

    [Fact]
    public void Create_ShouldNormalizeStateToUppercase()
    {
        var result = Address.Create("Rua A", "1", null, "Bairro", "Cidade", "sp", "01310100");

        result.IsSuccess.Should().BeTrue();
        result.Value.State.Should().Be("SP");
    }

    [Fact]
    public void Create_ShouldStripZipCodeFormatting()
    {
        var result = Address.Create("Rua A", "1", null, "Bairro", "Cidade", "SP", "01310-100");

        result.IsSuccess.Should().BeTrue();
        result.Value.ZipCode.Should().Be("01310100");
    }

    [Fact]
    public void FormattedZipCode_ShouldReturnWithHyphen()
    {
        var result = Address.Create("Rua A", "1", null, "Bairro", "Cidade", "SP", "01310100");

        result.Value.FormattedZipCode.Should().Be("01310-100");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyStreet_ShouldFail(string street)
    {
        var result = Address.Create(street, "1", null, "Bairro", "Cidade", "SP", "01310100");
        result.IsFailure.Should().BeTrue();
    }

    [Theory]
    [InlineData("S")]    
    [InlineData("SPP")] 
    public void Create_WithInvalidState_ShouldFail(string state)
    {
        var result = Address.Create("Rua A", "1", null, "Bairro", "Cidade", state, "01310100");
        result.IsFailure.Should().BeTrue();
    }

    [Theory]
    [InlineData("0131010")]
    [InlineData("013101000")]  
    [InlineData("ABCDEFGH")]   
    public void Create_WithInvalidZipCode_ShouldFail(string zip)
    {
        var result = Address.Create("Rua A", "1", null, "Bairro", "Cidade", "SP", zip);
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Equality_SameAddress_ShouldBeEqual()
    {
        var a = Address.Create("Rua A", "1", null, "Bairro", "Cidade", "SP", "01310100").Value;
        var b = Address.Create("Rua A", "1", null, "Bairro", "Cidade", "SP", "01310100").Value;

        a.Should().Be(b);
    }

    [Fact]
    public void Equality_DifferentComplement_ShouldNotBeEqual()
    {
        var a = Address.Create("Rua A", "1", "Apto 1", "Bairro", "Cidade", "SP", "01310100").Value;
        var b = Address.Create("Rua A", "1", "Apto 2", "Bairro", "Cidade", "SP", "01310100").Value;

        a.Should().NotBe(b);
    }
}
