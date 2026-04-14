using BookStore.Domain.Common;

namespace BookStore.Domain.Entities;

public sealed class Category : Entity
{
    public string Name { get; private set; }
    public string Slug { get; private set; }
    public string? Description { get; private set; }
    public bool IsActvive { get; private set; }

    private readonly List<Book> _books = [];
    public IReadOnlyCollection<Book> Books => _books.AsReadOnly();

    private Category() { }

    public Category(string name, string slug, string? description)
    {
        Name = name;
        Slug = slug;
        Description = description;
        IsActvive = true;
    }

    public static Result<Category> Create(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Category>(Error.Validation(nameof(Name), "Category name is required"));

        if (name.Length > 100) 
            return Result.Failure<Category>(Error.Validation(nameof(Name), "Category name cannot exceed 100 characters"));

        var slug = GenerateSlug(name);
        return Result.Success(new Category(name.Trim(), slug, description?.Trim()));
    }

    public Result Update (string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(Error.Validation(nameof(Name), "Category name is required"));

        Name = name.Trim();
        Slug = GenerateSlug(name);
        Description = description?.Trim();
        SetUpdateAt();
        return Result.Success();
    }

    public void Deacttivate() { IsActvive = false; SetUpdateAt(); }
    public void Activate() { IsActvive = true; SetUpdateAt(); }

    private static string GenerateSlug(string name) =>
        name.Trim().ToLowerInvariant()
        .Replace(" ", "-")
        .Replace("ã", "a").Replace("â", "a").Replace("á", "a").Replace("à", "a")
        .Replace("ê", "e").Replace("é", "e").Replace("è", "e")
        .Replace("î", "i").Replace("í", "i")
        .Replace("õ", "o").Replace("ô", "o").Replace("ó", "o")
        .Replace("ú", "u").Replace("ü", "u")
        .Replace("ç", "c");
}
