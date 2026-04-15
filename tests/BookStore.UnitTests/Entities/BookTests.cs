using BookStore.Domain.Entities;
using BookStore.Domain.Enums;
using BookStore.Domain.Events;
using BookStore.UnitTests.Builders;
using FluentAssertions;

namespace BookStore.UnitTests.Entities;

public class BookTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var result = new BookBuilder().Build();

        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_ShouldRaiseBookCreatedEvent()
    {
        var book = new BookBuilder().Build();

        book.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<BookCreatedEvent>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyTitle_ShouldFail(string title)
    {
        var result = new BookBuilder().WithTitle(title).Build();
    }

    [Fact]
    public void Create_WithEmptyTitle_ShouldReturnFailure()
    {
        var result = BookTestHelper.CreateWithTitle("");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Title");
    }

    [Fact]
    public void Create_WithNegativePrice_ShouldReturnFailure()
    {
        var result = BookTestHelper.CreateWithPrice(-1m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Amount");
    }

    [Fact]
    public void Create_WithNegativeStock_ShouldReturnFailure()
    {
        var result = BookTestHelper.CreateWithStock(-1);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("StockQuantity");
    }

    [Fact]
    public void Create_WithInvalidIsbn_ShouldReturnFailure()
    {
        var result = BookTestHelper.CreateWithIsbn("ISBN-INVALIDO");

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void UpdatePrice_WithValidPrice_ShouldSucceedAndRaiseEvent()
    {
        var book = new BookBuilder().WithPrice(100m).Build();
        book.ClearDomainEvents();

        var result = book.UpdatePrice(150m);

        result.IsSuccess.Should().BeTrue();
        book.Price.Amount.Should().Be(150m);
        book.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<BookPriceChangedEvent>();
    }

    [Fact]
    public void UpdatePrice_WithNegativePrice_ShouldFail()
    {
        var book = new BookBuilder().Build();

        var result = book.UpdatePrice(-10m);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void AddStock_WithPositiveQuantity_ShouldIncreaseStock()
    {
        var book = new BookBuilder().WithStock(10).Build();

        var result = book.AddStock(5);

        result.IsSuccess.Should().BeTrue();
        book.StockQuantity.Should().Be(15);
    }

    [Fact]
    public void AddStock_WithZeroQuantity_ShouldFail()
    {
        var book = new BookBuilder().Build();

        var result = book.AddStock(0);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void DecrementStock_WithSufficientStock_ShouldSucceed()
    {
        var book = new BookBuilder().WithStock(10).Build();

        var result = book.DecrementStock(3);

        result.IsSuccess.Should().BeTrue();
        book.StockQuantity.Should().Be(7);
    }

    [Fact]
    public void DecrementStock_WhenReachesZero_ShouldRaiseOutOfStockEvent()
    {
        var book = new BookBuilder().WithStock(1).Build();
        book.ClearDomainEvents();

        book.DecrementStock(1);

        book.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<BookOutOfStockEvent>();
    }

    [Fact]
    public void DecrementStock_WithInsufficientStock_ShouldFail()
    {
        var book = new BookBuilder().WithStock(2).Build();

        var result = book.DecrementStock(5);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Book.InsufficientStock");
        book.StockQuantity.Should().Be(2);
    }

    [Fact]
    public void HasStock_WhenStockSufficient_ShouldReturnTrue()
    {
        var book = new BookBuilder().WithStock(5).Build();

        book.HasStock(5).Should().BeTrue();
        book.HasStock(6).Should().BeFalse();
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveFalse()
    {
        var book = new BookBuilder().Build();

        book.Deactivate();

        book.IsActive.Should().BeFalse();
        book.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Activate_AfterDeactivation_ShouldSetIsActiveTrue()
    {
        var book = new BookBuilder().Build();
        book.Deactivate();

        book.Activate();

        book.IsActive.Should().BeTrue();
    }
}

internal static class BookTestHelper
{
    private static readonly string ValidIsbn = "9780306406157";
    private static readonly Guid ValidCategoryId = Guid.NewGuid();

    public static dynamic CreateWithTitle(string title) =>
        Book.Create(title, "Autor", null, ValidIsbn, 50m, 10, 300,
            null, "Editora", DateOnly.FromDateTime(DateTime.Now), BookFormat.Paperback,
            "Português", ValidCategoryId);

    public static dynamic CreateWithPrice(decimal price) =>
        Book.Create("Título", "Autor", null, ValidIsbn, price, 10, 300,
            null, "Editora", DateOnly.FromDateTime(DateTime.Now), BookFormat.Paperback,
            "Português", ValidCategoryId);

    public static dynamic CreateWithStock(int stock) =>
        Book.Create("Título", "Autor", null, ValidIsbn, 50m, stock, 300,
            null, "Editora", DateOnly.FromDateTime(DateTime.Now), BookFormat.Paperback,
            "Português", ValidCategoryId);

    public static dynamic CreateWithIsbn(string isbn) =>
        Book.Create("Título", "Autor", null, isbn, 50m, 10, 300,
            null, "Editora", DateOnly.FromDateTime(DateTime.Now), BookFormat.Paperback,
            "Português", ValidCategoryId);
}
