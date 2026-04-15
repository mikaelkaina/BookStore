using Bogus;
using BookStore.Domain.Entities;
using BookStore.Domain.Enums;
using FluentAssertions;

namespace BookStore.UnitTests.Builders;

public class BookBuilder
{
    private readonly Faker _faker = new("pt_BR");

    private string _title;
    private string _author;
    private string? _description;
    private string _isbn;
    private decimal _price;
    private int _stock;
    private int _pageCount;
    private string? _coverUrl;
    private string _publisher;
    private DateOnly _publishedDate;
    private BookFormat _format;
    private string _language;
    private Guid _categoryId;

    public BookBuilder()
    {
        _title = _faker.Commerce.ProductName();
        _author = _faker.Name.FullName();
        _description = _faker.Lorem.Paragraph();
        _isbn = "9780306406157";
        _price = _faker.Random.Decimal(10, 300);
        _stock = _faker.Random.Int(1, 100);
        _pageCount = _faker.Random.Int(50, 1200);
        _coverUrl = _faker.Internet.Url();
        _publisher = _faker.Company.CompanyName();
        _publishedDate = DateOnly.FromDateTime(_faker.Date.Past(10));
        _format = _faker.PickRandom<BookFormat>();
        _language = "Português";
        _categoryId = Guid.NewGuid();
    }

    public BookBuilder WithTitle(string title) { _title = title; return this; }
    public BookBuilder WithAuthor(string author) { _author = author; return this; }
    public BookBuilder WithIsbn(string isbn) { _isbn = isbn; return this; }
    public BookBuilder WithPrice(decimal price) { _price = price; return this; }
    public BookBuilder WithStock(int stock) { _stock = stock; return this; }
    public BookBuilder WithPageCount(int pageCount) { _pageCount = pageCount; return this; }
    public BookBuilder WithCategoryId(Guid categoryId) { _categoryId = categoryId; return this; }
    public BookBuilder WithFormat(BookFormat format) { _format = format; return this; }
    public BookBuilder WithNoStock() { _stock = 0; return this; }

    public Book Build()
    {
        var result = Book.Create(
            _title, _author, _description, _isbn,
            _price, _stock, _pageCount, _coverUrl,
            _publisher, _publishedDate, _format,
            _language, _categoryId);

        result.IsSuccess.Should().BeTrue("BookBuilder deve sempre gerar um livro válido");
        return result.Value;
    }
}
