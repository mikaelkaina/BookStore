using BookStore.Domain.Entities;
using BookStore.Domain.Enums;
using BookStore.Domain.Events;
using BookStore.Domain.ValueObjects;
using BookStore.UnitTests.Builders;
using FluentAssertions;

namespace BookStore.UnitTests.Entities;

public class OrderTests
{
    private Order CreateOrderWithItem(int bookStock = 10, int quantity = 2)
    {
        var order = new OrderBuilder().Build();
        var book = new BookBuilder().WithStock(bookStock).Build();
        order.AddItem(book, quantity);
        order.ClearDomainEvents();
        return order;
    }

    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var result = new OrderBuilder().Build();

        result.Should().NotBeNull();
        result.Status.Should().Be(OrderStatus.Pending);
        result.OrderNumber.Should().StartWith("ORD-");
    }

    [Fact]
    public void Create_ShouldRaiseOrderCreatedEvent()
    {
        var order = new OrderBuilder().Build();

        order.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<OrderCreatedEvent>();
    }

    [Fact]
    public void Create_WithEmptyCustomerId_ShouldFail()
    {
        var address = OrderBuilder.BuildDefaultAddress();
        var result = Order.Create(Guid.Empty, address);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void AddItem_WithSufficientStock_ShouldSucceed()
    {
        var order = new OrderBuilder().Build();
        var book = new BookBuilder().WithStock(10).WithPrice(50m).Build();

        var result = order.AddItem(book, 2);

        result.IsSuccess.Should().BeTrue();
        order.Items.Should().HaveCount(1);
        order.Items.First().Quantity.Should().Be(2);
        order.SubTotal.Amount.Should().Be(100m);
    }

    [Fact]
    public void AddItem_SameBookTwice_ShouldAccumulateQuantity()
    {
        var order = new OrderBuilder().Build();
        var book = new BookBuilder().WithStock(10).Build();

        order.AddItem(book, 2);
        order.AddItem(book, 3);

        order.Items.Should().HaveCount(1);
        order.Items.First().Quantity.Should().Be(5);
    }

    [Fact]
    public void AddItem_WithInsufficientStock_ShouldFail()
    {
        var order = new OrderBuilder().Build();
        var book = new BookBuilder().WithStock(1).Build();

        var result = order.AddItem(book, 5);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Book.InsufficientStock");
    }

    [Fact]
    public void RemoveItem_ExistingBook_ShouldSucceed()
    {
        var order = new OrderBuilder().Build();
        var book = new BookBuilder().WithStock(10).Build();
        order.AddItem(book, 2);

        var result = order.RemoveItem(book.Id);

        result.IsSuccess.Should().BeTrue();
        order.Items.Should().BeEmpty();
    }

    [Fact]
    public void AddItem_AfterPaymentConfirmed_ShouldFail()
    {
        var order = CreateOrderWithItem();
        order.ConfirmPayment();

        var newBook = new BookBuilder().WithStock(10).Build();
        var result = order.AddItem(newBook, 1);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Order.CannotModify");
    }

    [Fact]
    public void ConfirmPayment_WithItems_ShouldChangeStatus()
    {
        var order = CreateOrderWithItem();

        var result = order.ConfirmPayment();

        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.PaymentConfirmed);
        order.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<OrderPaymentConfirmedEvent>();
    }

    [Fact]
    public void ConfirmPayment_WithNoItems_ShouldFail()
    {
        var order = new OrderBuilder().Build();

        var result = order.ConfirmPayment();

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Order.Empty");
    }

    [Fact]
    public void FullOrderLifecycle_ShouldTransitionCorrectly()
    {
        var order = CreateOrderWithItem();

        order.ConfirmPayment().IsSuccess.Should().BeTrue();
        order.StartProcessing().IsSuccess.Should().BeTrue();
        order.Ship().IsSuccess.Should().BeTrue();
        order.Deliver().IsSuccess.Should().BeTrue();

        order.Status.Should().Be(OrderStatus.Delivered);
        order.ShippedAt.Should().NotBeNull();
        order.DeliveredAt.Should().NotBeNull();
    }

    [Fact]
    public void Ship_ShouldRaiseOrderShippedEvent()
    {
        var order = CreateOrderWithItem();
        order.ConfirmPayment();
        order.StartProcessing();
        order.ClearDomainEvents();

        order.Ship();

        order.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<OrderShippedEvent>();
    }

    [Fact]
    public void Cancel_PendingOrder_ShouldSucceed()
    {
        var order = CreateOrderWithItem();

        var result = order.Cancel("Desistência do cliente");

        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Cancelled);
        order.CancelledAt.Should().NotBeNull();
        order.DomainEvents.OfType<OrderCancelledEvent>().Should().HaveCount(1);
    }

    [Fact]
    public void Cancel_DeliveredOrder_ShouldFail()
    {
        var order = CreateOrderWithItem();
        order.ConfirmPayment();
        order.StartProcessing();
        order.Ship();
        order.Deliver();

        var result = order.Cancel();

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Order.InvalidTransition");
    }

    [Fact]
    public void ApplyDiscount_LessThanSubtotal_ShouldSucceed()
    {
        var order = CreateOrderWithItem();
        var discount = Money.Create(10m).Value;

        var result = order.ApplyDiscount(discount);

        result.IsSuccess.Should().BeTrue();
        order.Total.Amount.Should().Be(order.SubTotal.Amount - 10m);
    }

    [Fact]
    public void ApplyDiscount_ExceedingSubtotal_ShouldFail()
    {
        var order = new OrderBuilder().Build();
        var book = new BookBuilder().WithPrice(50m).WithStock(10).Build();
        order.AddItem(book, 1); // Subtotal = 50

        var discount = Money.Create(100m).Value;
        var result = order.ApplyDiscount(discount);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Order.DiscountExceedsSubtotal");
    }

    [Fact]
    public void SetShipping_ShouldAddToTotal()
    {
        var order = new OrderBuilder().Build();
        var book = new BookBuilder().WithPrice(100m).WithStock(10).Build();
        order.AddItem(book, 1);

        var shipping = Money.Create(15m).Value;
        order.SetShipping(shipping);

        order.Total.Amount.Should().Be(115m);
    }

    [Fact]
    public void InvalidTransition_ShouldFail()
    {
        var order = CreateOrderWithItem();

        var result = order.Ship();

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Order.InvalidTransition");
    }
}
