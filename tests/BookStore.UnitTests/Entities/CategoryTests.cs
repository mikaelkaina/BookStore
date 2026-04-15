using BookStore.Domain.Entities;
using FluentAssertions;

namespace BookStore.UnitTests.Entities;

public class CategoryTests
{
    [Fact]
    public void Create_WithValidName_ShouldSucceed()
    {
        var result = Category.Create("Ficção Científica", "Livros de FC");

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Ficção Científica");
        result.Value.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_ShouldGenerateSlugFromName()
    {
        var result = Category.Create("Ficção Científica");

        result.Value.Slug.Should().Be("ficcao-cientifica");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyName_ShouldFail(string name)
    {
        var result = Category.Create(name);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Name");
    }

    [Fact]
    public void Create_WithNameOver100Chars_ShouldFail()
    {
        var longName = new string('A', 101);
        var result = Category.Create(longName);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Update_WithValidName_ShouldChangeNameAndSlug()
    {
        var category = Category.Create("Aventura").Value;

        var result = category.Update("Romance", "Livros de Romance");

        result.IsSuccess.Should().BeTrue();
        category.Name.Should().Be("Romance");
        category.Slug.Should().Be("romance");
        category.Description.Should().Be("Livros de Romance");
        category.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveFalse()
    {
        var category = Category.Create("Terror").Value;

        category.Deactivate();

        category.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Activate_AfterDeactivation_ShouldRestoreActive()
    {
        var category = Category.Create("Terror").Value;
        category.Deactivate();

        category.Activate();

        category.IsActive.Should().BeTrue();
    }

    [Theory]
    [InlineData("Ação", "acao")]
    [InlineData("Não-Ficção", "nao-ficcao")]
    [InlineData("Ciência e Tecnologia", "ciencia-e-tecnologia")]
    public void Create_ShouldGenerateCorrectSlugWithSpecialChars(string name, string expectedSlug)
    {
        var result = Category.Create(name);

        result.IsSuccess.Should().BeTrue();
        result.Value.Slug.Should().Be(expectedSlug);
    }
}
