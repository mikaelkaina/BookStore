using BookStore.Domain.Common;
using BookStore.Domain.ValueObjets;

namespace BookStore.Domain.Entities;

public sealed class Cart : Entity
{
    public Guid? CustomerId { get; private set; }
    public string? SessionId { get; private set; }
    public bool IsCheckedOut { get; private set; }
    public DateTime? ExpiresAt { get; private set; }

    private readonly List<CartItem> _items = [];
    public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

    public Money Total => _items.Aggregate(Money.Zero(), (acc, item) => acc.Add(item.TotalPrice));
    public int TotalItems => _items.Sum(i => i.Quantity);

    private Cart() { }

    private Cart(Guid? customerId, string? sessionId)
    {
        CustomerId = customerId;
        SessionId = sessionId;
        IsCheckedOut = false;
        ExpiresAt = DateTime.UtcNow.AddDays(7);
    }

    public static Cart CreateForCustomer(Guid customerId) => new(customerId, null);
    public static Cart CreateForGuest(string sessionId) => new(null, sessionId);

    public Result AddItem(Book book, int quantity)
    {
        if (IsCheckedOut) return Result.Failure(CartErrors.AlreadyCheckedOut(Id));
        if (IsExpired) return Result.Failure(CartErrors.Expired(Id));
        if (!book.IsActive) return Result.Failure(CartErrors.BookNotAvailable(book.Id));
        if (!book.HasStock(quantity)) return Result.Failure(BookErrors.InsufficientStock(book.Id, quantity, book.StockQuantity));

        var existing = _items.FirstOrDefault(i => i.BookId == book.Id);
        if (existing is not null)
        {
            var newQty = existing.Quantity + quantity;
            if(!book.HasStock(newQty))
                return Result.Failure(BookErrors.InsufficientStock(book.Id, newQty, book.StockQuantity));
            existing.UpdateQuantity(newQty);
        }
        else
        {
            _items.Add(CartItem.Create(Id, book.Id, book.Title, book.CoverImageUrl, book.Price, quantity));
        }

        SetUpdatedAt();
        return Result.Success();
    }

    public Result RemoveItem(Guid bookId)
    {
        var item = _items.FirstOrDefault(i => i.BookId == bookId);
        if (item is null) return Result.Failure(Error.NotFound("CartItem", bookId));
        _items.Remove(item);
        SetUpdatedAt(); 
        return Result.Success();
    }

    public Result UpdateItemQuantity(Guid bookId, int quantity, int availableStock)
    {
        if (quantity <= 0) return RemoveItem(bookId);

        var item = _items.FirstOrDefault(i => i.BookId == bookId);
        if (item is null) return Result.Failure(Error.NotFound("CartItem", bookId));
        if (availableStock < quantity) return Result.Failure(BookErrors.InsufficientStock(bookId, quantity, availableStock));

        item.UpdateQuantity(quantity);
        SetUpdatedAt();
        return Result.Success();
    }

    public void Clear() { _items.Clear(); SetUpdatedAt(); }

    public Result Checkout()
    {
        if (IsCheckedOut) return Result.Failure(CartErrors.AlreadyCheckedOut(Id));
        if (!_items.Any()) return Result.Failure(CartErrors.Empty(Id));
        IsCheckedOut = true;
        SetUpdatedAt();
        return Result.Success();
    }

    public void AssignToCustomer(Guid customerId) { CustomerId = customerId; SetUpdatedAt(); }
    public bool IsExpired => DateTime.UtcNow > ExpiresAt;
}

public sealed class CartItem : Entity
{
    public Guid CartId { get; private set; }
    public Guid BookId { get; private set; }
    public string BookTitle { get; private set; }
    public string? BookCoverUrl { get; private set; }
    public Money UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public Money TotalPrice { get; private set; }

    private CartItem() { }

    private CartItem(Guid cartId, Guid bookId, string bookTitle,
        string? bookCoverUrl, Money unitPrice, int quantity)
    {
        CartId = cartId; BookId = bookId; BookTitle = bookTitle;
        BookCoverUrl = bookCoverUrl; UnitPrice = unitPrice;
        Quantity = quantity; TotalPrice = unitPrice.Multiply(quantity);
    }

    internal static CartItem Create(Guid cartId, Guid bookId, string bookTitle,
        string? coverUrl, Money unitPrice, int quantity) =>
        new(cartId, bookId, bookTitle, coverUrl, unitPrice, quantity);

    internal void UpdateQuantity(int quantity)
    {
        Quantity = quantity;
        TotalPrice = UnitPrice.Multiply(quantity);
    }
}

public static class CartErrors
{
    public static Error NotFound(Guid id) => Error.NotFound(nameof(Cart), id);
    public static Error AlreadyCheckedOut(Guid id) => new("Cart.AlreadyCheckedOut", $"Cart '{id}' has already been checked out.");
    public static Error Expired(Guid id) => new("Cart.Expired", $"Cart '{id}' has expired.");
    public static Error Empty(Guid id) => new("Cart.Empty", $"Cart '{id}' has no items.");
    public static Error BookNotAvailable(Guid bookId) => new("Cart.BookNotAvailable", $"Book '{bookId}' is not available.");
}

