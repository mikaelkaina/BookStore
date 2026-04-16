using BookStore.Domain.Common;
using BookStore.Domain.Enums;
using BookStore.Domain.Events;
using BookStore.Domain.ValueObjects;

namespace BookStore.Domain.Entities;

public sealed class Order : Entity
{
    public string OrderNumber { get; private set; } = null!;
    public Guid CustomerId { get; private set; }
    public Customer Customer { get; set; } = null!;
    public OrderStatus Status { get; private set; }
    public Money SubTotal { get; private set; } = null!;
    public Money ShippingCost { get; private set; } = null!;
    public Money Discount { get; private set; } = null!;
    public Money Total { get; private set; } = null!;
    public Address ShippingAddress { get; private set; } = null!;
    public string? Notes { get; private set; }
    public DateTime? ShippedAt { get; private set; }
    public DateTime? DeliveredAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }

    private readonly List<OrderItem> _items = [];
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    private Order() { }

    private Order(Guid customerId, Address shippingAddress, string? notes)
    {
        CustomerId = customerId;
        ShippingAddress = shippingAddress;
        Notes = notes;
        Status = OrderStatus.Pending;
        OrderNumber = GenerateOrderNumber();
        SubTotal = Money.Zero();
        ShippingCost = Money.Zero();
        Discount = Money.Zero();
        Total = Money.Zero();
    }

    public static Result<Order> Create(Guid customerId, Address shippingAddress, string? notes = null)
    {
        if (customerId == Guid.Empty)
            return Result.Failure<Order>(Error.Validation(nameof(CustomerId), "Customer is required"));

        var order = new Order(customerId, shippingAddress, notes);
        order.RaiseDomainEvent(new OrderCreatedEvent(order.Id, order.CustomerId));
        return Result.Success(order);
    }

    public Result AddItem(Book book, int quantity)
    {
        if (!IsEditable)
            return Result.Failure(OrderErrors.CannotModifyOrder(Id, Status));

        if (quantity <= 0)
            return Result.Failure<Order>(Error.Validation(nameof(quantity), "Quantity must be greater than zero"));

        if (!book.HasStock(quantity))
            return Result.Failure(BookErrors.InsufficientStock(book.Id, quantity, book.StockQuantity));

        var existing = _items.FirstOrDefault(i => i.BookId == book.Id);
        if (existing is not null)
        {
            var newQty = existing.Quantity + quantity;
            if (!book.HasStock(newQty))
                return Result.Failure(BookErrors.InsufficientStock(book.Id, newQty, book.StockQuantity));
            existing.IncreaseQuantity(quantity);
        }
        else
        {
            _items.Add(OrderItem.Create(Id, book.Id, book.Title, book.Price, quantity));
        }

        RecalculateTotas();
        return Result.Success();
    }

    public Result RemoveItem(Guid bookId)
    {
        if (!IsEditable)
            return Result.Failure(OrderErrors.CannotModifyOrder(Id, Status));

        var item = _items.FirstOrDefault(i => i.BookId == bookId);
        if (item is null) return Result.Failure(Error.NotFound("OrderItem", bookId));
       
        _items.Remove(item);
        RecalculateTotas();
        return Result.Success();
    }

    public Result ConfirmPayment()
    {
        if (Status != OrderStatus.Pending)
            return Result.Failure(OrderErrors.InvalidStatusTransition(Id, Status, OrderStatus.PaymentConfirmed));

        if (!_items.Any())
            return Result.Failure(OrderErrors.EmptyOrder(Id));

        Status = OrderStatus.PaymentConfirmed;
        SetUpdatedAt();
        RaiseDomainEvent(new OrderPaymentConfirmedEvent(Id, CustomerId, Total));
        return Result.Success();
    }

    public Result StartProcessing()
    {
        if (Status != OrderStatus.PaymentConfirmed)
            return Result.Failure(OrderErrors.InvalidStatusTransition(Id, Status, OrderStatus.Processing));

        Status = OrderStatus.Processing;
        SetUpdatedAt();
        return Result.Success();
    }

    public Result Ship()
    {
        if (Status != OrderStatus.Processing)
            return Result.Failure(OrderErrors.InvalidStatusTransition(Id, Status, OrderStatus.Shipped));

        Status = OrderStatus.Shipped;
        ShippedAt = DateTime.UtcNow;
        SetUpdatedAt();
        RaiseDomainEvent(new OrderShippedEvent(Id, CustomerId, ShippingAddress));
        return Result.Success();
    }

    public Result Deliver()
    {
        if (Status != OrderStatus.Shipped)
            return Result.Failure(OrderErrors.InvalidStatusTransition(Id, Status, OrderStatus.Delivered));

        Status = OrderStatus.Delivered;
        DeliveredAt = DateTime.UtcNow;
        SetUpdatedAt();
        return Result.Success();
    }

    public Result Cancel(string? reason = null)
    {
        if (Status is OrderStatus.Delivered or OrderStatus.Returned)
            return Result.Failure(OrderErrors.InvalidStatusTransition(Id, Status, OrderStatus.Cancelled));

        Status = OrderStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        Notes = reason;
        SetUpdatedAt();
        RaiseDomainEvent(new OrderCancelledEvent(Id, CustomerId, reason));
        return Result.Success();
    }

    public Result ApplyDiscount(Money discount)
    {
        if (discount.IsGreaterThan(SubTotal))
            return Result.Failure(OrderErrors.DiscountExceedsSubtotal(Id));
        
        Discount = discount;
        RecalculateTotas();
        return Result.Success();
    }

    public Result SetShipping(Money shippingCost)
    {
        ShippingCost = shippingCost;
        RecalculateTotas();
        return Result.Success();
    }

    private bool IsEditable => Status is OrderStatus.Pending;

    private void RecalculateTotas()
    {
        SubTotal = _items.Aggregate(Money.Zero(), (acc, item) => acc.Add(item.TotalPrice));
        Total = SubTotal.Add(ShippingCost).Subtract(Discount);
        SetUpdatedAt();
    }

    private static string GenerateOrderNumber() =>
        $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";
}

public sealed class OrderItem : Entity
{
    public Guid OrderId { get; private set; }
    public Guid BookId { get; private set; }
    public string BookTitle { get; private set; } = null!;
    public Money UnitPrice { get; private set; } = null!;
    public int Quantity { get; private set; }
    public Money TotalPrice { get; private set; } = null!;

    private OrderItem() { }

    private OrderItem(Guid orderId, Guid bookId, string bookTitle, Money unitPrice, int quantity)
    {
        OrderId = orderId; BookId = bookId; BookTitle = bookTitle;
        UnitPrice = unitPrice; Quantity = quantity;
        TotalPrice = unitPrice.Multiply(quantity);
    }

    internal static OrderItem Create(Guid orderId, Guid bookId, string bookTitle, Money unitPrice, int quantity) =>
        new(orderId, bookId, bookTitle, unitPrice, quantity);

    internal void IncreaseQuantity(int quantity)
    {
        Quantity += quantity;
        TotalPrice = UnitPrice.Multiply(Quantity);
    }
}

public static class OrderErrors
{
    public static Error NotFound(Guid id) => Error.NotFound(nameof(Order), id);
    public static Error CannotModifyOrder(Guid id, OrderStatus status) =>
        new("Order.CannotModify", $"Order '{id}' cannot be modified in status '{status}'.");
    public static Error InvalidStatusTransition(Guid id, OrderStatus from, OrderStatus to) =>
        new("Order.InvalidTransition", $"Order '{id}' cannot transition from '{from}' to '{to}'.");
    public static Error EmptyOrder(Guid id) =>
        new("Order.Empty", $"Order '{id}' must have at least one item.");
    public static Error DiscountExceedsSubtotal(Guid id) =>
        new("Order.DiscountExceedsSubtotal", $"Discount cannot exceed the subtotal.");
}
