using BookStore.Application.Features.Books.Commands.CreateBook;
using BookStore.Application.Features.Books.Commands.UpdateBook;
using BookStore.Application.Features.Books.Queries.GetBookById;
using BookStore.Application.Features.Books.Queries.GetBooksPaged;
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
            book.CreatedAt
        );

    public static UpdateBookResponse ToUpdateResponse(this Book book, string categoryName) =>
        new(
            book.Id,
            book.Title,
            book.Author,
            book.Description,
            book.CoverImageUrl,
            book.Publisher,
            categoryName,
            book.UpdatedAt
        );

    public static GetBookByIdResponse ToGetByIdResponse(this Book book) =>
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
            book.Category.Name,
            book.CreatedAt,
            book.UpdatedAt
        );

    public static GetBooksPagedResponse ToGetPagedResponse(this Book book) =>
        new(
            book.Id,
            book.Title,
            book.Author,
            book.Isbn.Value,
            book.Price.Amount,
            book.Price.Currency,
            book.StockQuantity,
            book.CoverImageUrl,
            book.Format.ToString(),
            book.Category.Name
        );
}