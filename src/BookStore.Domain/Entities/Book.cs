using BookStore.Domain.Common;
using BookStore.Domain.Enums;
using BookStore.Domain.Events;
using BookStore.Domain.ValueObjets;

namespace BookStore.Domain.Entities;

public sealed class Book : Entity
{
    public string Title { get; private set; }
    public string Author { get; private set; }
    public string? Description { get; private set; }
    public Isbn Isbn { get; private set; }
    public Money Price { get; private set; }
    public int StockQuantity { get; private set; }
    public int PageCount { get; private set; }
    public string? CoverImageUrl { get; private set; }
    public string Publisher { get; private set; }
    public DateOnly PublishedDate { get; private set; }
    public BookFormat Format { get; private set; }
    public string Language { get; private set; }
    public bool IsActive { get; private set; }
    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; } = null!;

    private Book() { }
    private Book(string title, string author, string? description, Isbn isbn, Money price, int stockQuantity, int pageCount,
        string coverImageUrl, string publisher, DateOnly publishedDate, BookFormat format, string language, Guid categoryId)
    {
        Title = title;
        Author = author;
        Description = description;
        Isbn = isbn;
        Price = price;
        StockQuantity = stockQuantity;
        PageCount = pageCount;
        CoverImageUrl = coverImageUrl;
        Publisher = publisher;
        PublishedDate = publishedDate;
        Format = format;
        Language = language;
        CategoryId = categoryId;
        IsActive = true; // Default to active when created
    }

    public static Result<Book> Create(string title, string author, string? description, string isbn, decimal price, int stockQuantity, 
        int pageCount, string? coverImageUrl, string publisher, DateOnly publishedDate, BookFormat format, string language, Guid categoryId)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure<Book>(Error.Validation(nameof(Title),"Book title is required"));

        if (title.Length > 200)
            return Result.Failure<Book>(Error.Validation(nameof(Title),"Book title cannot exceed 200 characters"));

        if (string.IsNullOrWhiteSpace(author))
            return Result.Failure<Book>(Error.Validation(nameof(Author),"Author is required."));

        if (pageCount <= 0)
            return Result.Failure<Book>(Error.Validation(nameof(PageCount), "Page count must be greater than zero."));

        if (stockQuantity < 0)
            return Result.Failure<Book>(Error.Validation(nameof(StockQuantity), "Stock quantity cannot be negative."));

        if (categoryId == Guid.Empty)
            return Result.Failure<Book>(Error.Validation(nameof(CategoryId), "Category is required."));

        var isbnResult = Isbn.Create(isbn);
        if (isbnResult.IsFailure) return Result.Failure<Book>(isbnResult.Error);
        
        var priceResult = Money.Create(price);
        if (priceResult.IsFailure) return Result.Failure<Book>(priceResult.Error);

        var book = new Book(title.Trim(), author.Trim(), description?.Trim(), isbnResult.Value, priceResult.Value, stockQuantity, pageCount,
            coverImageUrl, publisher.Trim(), publishedDate, format, language, categoryId);

        book.RaiseDomainEvent(new BookCreatedEvent(book.Id, book.Title));
        return Result.Success(book);
    }

    public Result UpdatePrice(decimal newPrice)
    {
        var priceResult = Money.Create(newPrice);
        if (priceResult.IsFailure) return Result.Failure(priceResult.Error);
        
        var oldPrice = Price;
        Price = priceResult.Value;
        SetUpdateAt();
        RaiseDomainEvent(new BookPriceChangedEvent(Id, oldPrice, Price));
        return Result.Success();
    }

    public Result AddStock(int quantity)
    {
        if (quantity <= 0)
            return Result.Failure(Error.Validation(nameof(quantity), "Quantity must be greater than zero."));
        StockQuantity += quantity;
        SetUpdateAt();
        return Result.Success();
    }

    public Result DecrementStock(int quantity)
    {
        if (quantity <= 0)
            return Result.Failure(Error.Validation(nameof(quantity), "Quantity must be greater than zero."));

        if (StockQuantity < quantity)
            return Result.Failure(BookErrors.InsufficientStock(Id, quantity, StockQuantity));

        StockQuantity -= quantity;
        SetUpdateAt();

        if(StockQuantity == 0)
            RaiseDomainEvent(new BookOutOfStockEvent(Id, Title));

        return Result.Success();
    }

    public bool HasStock(int quantity = 1) => StockQuantity >= quantity;
    public void Deactivate() { IsActive = false; SetUpdateAt(); }
    public void Activate() { IsActive = true; SetUpdateAt(); }

}
public static class BookErrors
{
    public static Error NotFound(Guid id) => Error.NotFound(nameof(Book), id);

    public static Error InsufficientStock(Guid bookId, int requested, int available) =>
        new("Book.InsufficientStock", 
            $"Book '{bookId}' has insufficient stock. Requested: {requested}, Available: {available}");

    public static Error IsbnAlreadyExists(string isbn) =>
        new("Book.IsbnAlreadyExists", $"A book with ISBN '{isbn}' already exists.");
}
