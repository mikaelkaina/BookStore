using BookStore.Domain.Entities;
using BookStore.Domain.Enums;

namespace BookStore.ApplicationTests.Builders;

public class BookBuilder
{
    private int _stock = 10;

    public BookBuilder WithStock(int stock)
    {
        _stock = stock;
        return this;
    }

    public Book Build()
    {
        var result = Book.Create(
            "Clean Code",
            "Robert C. Martin",
            null,
            "9780132350884",
            50,
            _stock,
            200,
            null,
            "Prentice Hall",
            new DateOnly(2008, 1, 1),
            BookFormat.Paperback,
            "EN",
            Guid.NewGuid()
        );

        if (result.IsFailure)
            throw new Exception("BookBuilder falhou");

        return result.Value;
    }
}