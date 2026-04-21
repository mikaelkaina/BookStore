using BookStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore.Infrastructure.Persistence.Configurations;

public sealed class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("Books");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .ValueGeneratedNever();

        builder.Property(b => b.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(b => b.Author)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(b => b.Description)
            .HasMaxLength(2000);


        builder.OwnsOne(b => b.Isbn, isbn =>
        {
            isbn.Property(i => i.Value)
                .HasColumnName("Isbn")
                .IsRequired()
                .HasMaxLength(13);

            isbn.HasIndex(i => i.Value)
                .IsUnique();
        });

        builder.OwnsOne(b => b.Price, price =>
        {
            price.Property(p => p.Amount)
                .HasColumnName("Price")
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            price.Property(p => p.Currency)
                .HasColumnName("Currency")
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("BRL");
        });

        builder.Property(b => b.StockQuantity)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(b => b.PageCount)
            .IsRequired();

        builder.Property(b => b.CoverImageUrl)
            .HasMaxLength(500);

        builder.Property(b => b.Publisher)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(b => b.PublishedDate)
            .IsRequired();

        builder.Property(b => b.Format)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(b => b.Language)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(b => b.IsActive)
            .IsRequired()
            .HasDefaultValue(true);


        builder.HasOne(b => b.Category)
            .WithMany()
            .HasForeignKey(b => b.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(b => b.CreatedAt)
            .IsRequired();
    }
}
