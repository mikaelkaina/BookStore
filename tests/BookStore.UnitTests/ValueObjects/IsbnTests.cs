using BookStore.Domain.ValueObjects;
using FluentAssertions;

namespace BookStore.UnitTests.ValueObjects;

public class IsbnTests
{
    [Theory]
    [InlineData("9780306406157")]
    [InlineData("978-0-306-40615-7")]
    [InlineData("9780140449136")]
    public void Create_WithValidIsbn13_ShouldSucceed(string isbn)
    {
        var result = Isbn.Create(isbn);

        result.IsSuccess.Should().BeTrue(because: $"'{isbn}' é um ISBN-13 válido");
        result.Value.Value.Should().NotContain("-");
    }

    [Theory]
    [InlineData("0306406152")]
    [InlineData("0-306-40615-2")]
    public void Create_WithValidIsbn10_ShouldSucceed(string isbn)
    {
        var result = Isbn.Create(isbn);

        result.IsSuccess.Should().BeTrue(because: $"'{isbn}' é um ISBN-10 válido");
    }

    [Theory]
    [InlineData("9780306406158")] // checksum errado
    [InlineData("1234567890123")] // inválido
    [InlineData("123")]           // muito curto
    [InlineData("")]
    [InlineData(null)]
    public void Create_WithInvalidIsbn_ShouldFail(string? isbn)
    {
        var result = Isbn.Create(isbn!);

        result.IsFailure.Should().BeTrue(because: $"'{isbn}' não é um ISBN válido");
    }

    [Fact]
    public void Create_ShouldStripHyphens()
    {
        var result = Isbn.Create("978-0-306-40615-7");

        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be("9780306406157");
    }

    [Fact]
    public void Equality_SameIsbn_ShouldBeEqual()
    {
        var a = Isbn.Create("9780306406157").Value;
        var b = Isbn.Create("9780306406157").Value;

        a.Should().Be(b);
    }
}
