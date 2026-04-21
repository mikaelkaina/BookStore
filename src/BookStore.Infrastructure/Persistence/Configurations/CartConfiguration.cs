using BookStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore.Infrastructure.Persistence.Configurations;

public sealed class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("Carts");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedNever();

        builder.Property(c => c.CustomerId);

        builder.Property(c => c.SessionId)
            .HasMaxLength(100);

        builder.HasIndex(c => c.CustomerId)
            .IsUnique()
            .HasFilter("[CustomerId] IS NOT NULL");

        builder.HasIndex(c => c.SessionId)
            .IsUnique()
            .HasFilter("[SessionId] IS NOT NULL");

        builder.Property(c => c.IsCheckedOut)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(c => c.ExpiresAt)
            .IsRequired();

        builder.OwnsMany(c => c.Items, item =>
        {
            item.ToTable("CartItems");

            item.WithOwner()
                .HasForeignKey(i => i.CartId);

            item.HasKey(i => i.Id);

            item.Property(i => i.Id)
                .ValueGeneratedNever();

            item.Property(i => i.BookId)
                .IsRequired();

            item.Property(i => i.BookTitle)
                .IsRequired()
                .HasMaxLength(200);

            item.Property(i => i.BookCoverUrl)
                .HasMaxLength(500);

            item.Property(i => i.Quantity)
                .IsRequired();

            item.OwnsOne(i => i.UnitPrice, m =>
            {
                m.Property(p => p.Amount)
                    .HasColumnName("UnitPriceAmount")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                m.Property(p => p.Currency)
                    .HasColumnName("UnitPriceCurrency")
                    .HasMaxLength(3)
                    .HasDefaultValue("BRL");
            });

            item.OwnsOne(i => i.TotalPrice, m =>
            {
                m.Property(p => p.Amount)
                    .HasColumnName("TotalPriceAmount")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                m.Property(p => p.Currency)
                    .HasColumnName("TotalPriceCurrency")
                    .HasMaxLength(3)
                    .HasDefaultValue("BRL");
            });
        });

        builder.Property(c => c.CreatedAt)
            .IsRequired();
    }
}