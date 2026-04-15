using Bogus;
using BookStore.Domain.ValueObjects;
using FluentAssertions;

namespace BookStore.UnitTests.ValueObjects;

public class EmailTests
{
    private readonly Faker _faker = new("pt_BR");

    [Fact]
    public void Create_WithValidEmail_ShouldSucceed()
    {
        var result = Email.Create("usuario@exemplo.com.br");

        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be("usuario@exemplo.com.br");
    }

    [Theory]
    [InlineData("EMAIL@EXEMPLO.COM")]
    [InlineData("  email@teste.com  ")]
    public void Create_ShouldNormalizeToLowerAndTrim(string raw)
    {
        var result = Email.Create(raw);

        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(raw.Trim().ToLowerInvariant());
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyOrNull_ShouldFail(string? email)
    {
        var result = Email.Create(email!);

        result.IsFailure.Should().BeTrue();
    }

    [Theory]
    [InlineData("semArroba")]
    [InlineData("@semlocal.com")]
    [InlineData("sem@dominio")]
    [InlineData("duplo@@arroba.com")]
    public void Create_WithInvalidFormat_ShouldFail(string email)
    {
        var result = Email.Create(email);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Email");
    }

    [Fact]
    public void Create_WithFakerEmail_ShouldSucceed()
    {
        for (var i = 0; i < 20; i++)
        {
            var email = _faker.Internet.Email();
            var result = Email.Create(email);
            result.IsSuccess.Should().BeTrue(because: $"'{email}' deve ser um email válido");
        }
    }

    [Fact]
    public void Equality_SameValue_ShouldBeEqual()
    {
        var a = Email.Create("teste@email.com").Value;
        var b = Email.Create("teste@email.com").Value;

        a.Should().Be(b);
    }

    [Fact]
    public void ToString_ShouldReturnEmailValue()
    {
        var email = Email.Create("user@test.com").Value;
        email.ToString().Should().Be("user@test.com");
    }
}
