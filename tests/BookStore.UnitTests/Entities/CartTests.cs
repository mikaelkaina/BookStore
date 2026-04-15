using BookStore.Domain.Entities;
using BookStore.UnitTests.Builders;
using FluentAssertions;

namespace BookStore.UnitTests.Entities;

public class CartTests
{
    [Fact]
    public void CreateForCustomer_ShouldSetCustomerId()
    {
        var customerId = Guid.NewGuid();
        var cart = new CartBuilder().WithCustomerId(customerId).Build();

        cart.CustomerId.Should().Be(customerId);
        cart.SessionId.Should().BeNull();
        cart.IsCheckedOut.Should().BeFalse();
    }

    [Fact]
    public void CreateForGuest_ShouldSetSessionId()
    {
        var sessionId = "sess-abc-123";
        var cart = new CartBuilder().ForGuest(sessionId).Build();

        cart.SessionId.Should().Be(sessionId);
        cart.CustomerId.Should().BeNull();
    }

    [Fact]
    public void AddItem_WithValidBook_ShouldSucceed()
    {
        var cart = new CartBuilder().Build();
        var book = new BookBuilder().WithPrice(40m).WithStock(10).Build();

        var result = cart.AddItem(book, 2);

        result.IsSuccess.Should().BeTrue();
        cart.Items.Should().HaveCount(1);
        cart.TotalItems.Should().Be(2);
        cart.Total.Amount.Should().Be(80m);
    }

    [Fact]
    public void AddItem_SameBook_ShouldAccumulateQuantity()
    {
        var cart = new CartBuilder().Build();
        var book = new BookBuilder().WithStock(10).Build();

        cart.AddItem(book, 2);
        cart.AddItem(book, 3);

        cart.Items.Should().HaveCount(1);
        cart.Items.First().Quantity.Should().Be(5);
    }

    [Fact]
    public void AddItem_WithInsufficientStock_ShouldFail()
    {
        var cart = new CartBuilder().Build();
        var book = new BookBuilder().WithStock(1).Build();

        var result = cart.AddItem(book, 5);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Book.InsufficientStock");
    }

    [Fact]
    public void AddItem_InactiveBook_ShouldFail()
    {
        var cart = new CartBuilder().Build();
        var book = new BookBuilder().Build();
        book.Deactivate();

        var result = cart.AddItem(book, 1);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Cart.BookNotAvailable");
    }

    [Fact]
    public void RemoveItem_ExistingItem_ShouldSucceed()
    {
        var cart = new CartBuilder().Build();
        var book = new BookBuilder().WithStock(5).Build();
        cart.AddItem(book, 2);

        var result = cart.RemoveItem(book.Id);

        result.IsSuccess.Should().BeTrue();
        cart.Items.Should().BeEmpty();
    }

    [Fact]
    public void RemoveItem_NonExistingItem_ShouldFail()
    {
        var cart = new CartBuilder().Build();

        var result = cart.RemoveItem(Guid.NewGuid());

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void UpdateItemQuantity_ToZero_ShouldRemoveItem()
    {
        var cart = new CartBuilder().Build();
        var book = new BookBuilder().WithStock(10).Build();
        cart.AddItem(book, 3);

        var result = cart.UpdateItemQuantity(book.Id, 0, 10);

        result.IsSuccess.Should().BeTrue();
        cart.Items.Should().BeEmpty();
    }

    [Fact]
    public void Clear_ShouldRemoveAllItems()
    {
        var cart = new CartBuilder().Build();
        var book1 = new BookBuilder().WithStock(5).Build();
        var book2 = new BookBuilder().WithStock(5).Build();
        cart.AddItem(book1, 1);
        cart.AddItem(book2, 1);

        cart.Clear();

        cart.Items.Should().BeEmpty();
        cart.Total.Amount.Should().Be(0);
    }

    [Fact]
    public void Checkout_WithItems_ShouldSucceed()
    {
        var cart = new CartBuilder().Build();
        var book = new BookBuilder().WithStock(5).Build();
        cart.AddItem(book, 1);

        var result = cart.Checkout();

        result.IsSuccess.Should().BeTrue();
        cart.IsCheckedOut.Should().BeTrue();
    }

    [Fact]
    public void Checkout_EmptyCart_ShouldFail()
    {
        var cart = new CartBuilder().Build();

        var result = cart.Checkout();

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Cart.Empty");
    }

    [Fact]
    public void Checkout_AlreadyCheckedOut_ShouldFail()
    {
        var cart = new CartBuilder().Build();
        var book = new BookBuilder().WithStock(5).Build();
        cart.AddItem(book, 1);
        cart.Checkout();

        var result = cart.Checkout();

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Cart.AlreadyCheckedOut");
    }

    [Fact]
    public void AddItem_ToCheckedOutCart_ShouldFail()
    {
        var cart = new CartBuilder().Build();
        var book = new BookBuilder().WithStock(10).Build();
        cart.AddItem(book, 1);
        cart.Checkout();

        var newBook = new BookBuilder().WithStock(5).Build();
        var result = cart.AddItem(newBook, 1);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Cart.AlreadyCheckedOut");
    }

    [Fact]
    public void AssignToCustomer_ShouldSetCustomerId()
    {
        var cart = Cart.CreateForGuest("session-123");
        var customerId = Guid.NewGuid();

        cart.AssignToCustomer(customerId);

        cart.CustomerId.Should().Be(customerId);
    }

    [Fact]
    public void Total_ShouldReflectAllItems()
    {
        var cart = new CartBuilder().Build();
        var book1 = new BookBuilder().WithPrice(50m).WithStock(10).Build();
        var book2 = new BookBuilder().WithPrice(30m).WithStock(10).Build();

        cart.AddItem(book1, 2); // 100
        cart.AddItem(book2, 3); // 90

        cart.Total.Amount.Should().Be(190m);
    }
}
