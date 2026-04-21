using BookStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore.Infrastructure.Persistence.Configurations;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .ValueGeneratedNever();

        builder.Property(o => o.OrderNumber)
            .IsRequired()
            .HasMaxLength(40);

        builder.HasIndex(o => o.OrderNumber)
            .IsUnique();

        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.OwnsOne(o => o.SubTotal, m =>
        {
            m.Property(p => p.Amount).HasColumnName("Subtotal").HasColumnType("decimal(18,2)");
            m.Property(p => p.Currency).HasColumnName("SubtotalCurrency").HasMaxLength(3).HasDefaultValue("BRL");
        });

        builder.OwnsOne(o => o.ShippingCost, m =>
        {
            m.Property(p => p.Amount).HasColumnName("ShippingCost").HasColumnType("decimal(18,2)");
            m.Property(p => p.Currency).HasColumnName("ShippingCurrency").HasMaxLength(3).HasDefaultValue("BRL");
        });

        builder.OwnsOne(o => o.Discount, m =>
        {
            m.Property(p => p.Amount).HasColumnName("Discount").HasColumnType("decimal(18,2)");
            m.Property(p => p.Currency).HasColumnName("DiscountCurrency").HasMaxLength(3).HasDefaultValue("BRL");
        });

        builder.OwnsOne(o => o.Total, m =>
        {
            m.Property(p => p.Amount).HasColumnName("Total").HasColumnType("decimal(18,2)");
            m.Property(p => p.Currency).HasColumnName("TotalCurrency").HasMaxLength(3).HasDefaultValue("BRL");
        });

        builder.OwnsOne(o => o.ShippingAddress, address =>
        {
            address.Property(a => a.Street).HasColumnName("ShippingStreet").IsRequired().HasMaxLength(200);
            address.Property(a => a.Number).HasColumnName("ShippingNumber").IsRequired().HasMaxLength(10);
            address.Property(a => a.Complement).HasColumnName("ShippingComplement").HasMaxLength(100);
            address.Property(a => a.Neighborhood).HasColumnName("ShippingNeighborhood").IsRequired().HasMaxLength(100);
            address.Property(a => a.City).HasColumnName("ShippingCity").IsRequired().HasMaxLength(100);
            address.Property(a => a.State).HasColumnName("ShippingState").IsRequired().HasMaxLength(2);
            address.Property(a => a.ZipCode).HasColumnName("ShippingZipCode").IsRequired().HasMaxLength(8);
            address.Property(a => a.Country).HasColumnName("ShippingCountry").IsRequired().HasMaxLength(50);
        });

        builder.Property(o => o.Notes).HasMaxLength(500);
        builder.Property(o => o.ShippedAt);
        builder.Property(o => o.DeliveredAt);
        builder.Property(o => o.CancelledAt);

        builder.HasOne(o => o.Customer)
            .WithMany()
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.OwnsMany(o => o.Items, item =>
        {
            item.ToTable("OrderItems");

            item.WithOwner().HasForeignKey(i => i.OrderId);

            item.HasKey(i => i.Id);
            item.Property(i => i.Id).ValueGeneratedNever();
            item.Property(i => i.BookId).IsRequired();
            item.Property(i => i.BookTitle).IsRequired().HasMaxLength(200);

            item.OwnsOne(i => i.UnitPrice, m =>
            {
                m.Property(p => p.Amount).HasColumnName("UnitPrice").HasColumnType("decimal(18,2)");
                m.Property(p => p.Currency).HasColumnName("UnitPriceCurrency").HasMaxLength(3).HasDefaultValue("BRL");
            });

            item.OwnsOne(i => i.TotalPrice, m =>
            {
                m.Property(p => p.Amount).HasColumnName("TotalPrice").HasColumnType("decimal(18,2)");
                m.Property(p => p.Currency).HasColumnName("TotalPriceCurrency").HasMaxLength(3).HasDefaultValue("BRL");
            });

            item.Property(i => i.Quantity).IsRequired();
        });

        builder.Property(o => o.CreatedAt).IsRequired();
    }
}


