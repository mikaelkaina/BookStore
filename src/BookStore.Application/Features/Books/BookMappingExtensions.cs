using BookStore.Application.Features.Books.Commands.CreateBook;
using BookStore.Domain.Entities;

namespace BookStore.Application.Features.Books;

public static class BookMappingExtensions
{
    public static CreateBookResponse ToCreateResponse(this Book book, string categoryName) =>
        new(
            book.Id,
            book.Title,
            book.Author,
            book.Description,
            book.Isbn.Value,
            book.Price.Amount,
            book.Price.Currency,
            book.StockQuantity,
            book.PageCount,
            book.CoverImageUrl,
            book.Publisher,
            book.PublishedDate,
            book.Format.ToString(),
            book.Language,
            book.IsActive,
            book.CategoryId,
            categoryName,
            book.CreatedAt);
}