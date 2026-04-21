using BookStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore.Infrastructure.Persistence.Configurations;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedNever();

        builder.Property(c => c.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.LastName)
            .IsRequired()
            .HasMaxLength(100);


        builder.ComplexProperty(c => c.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(250);
        });

        builder.HasIndex("Email")
            .IsUnique();

        builder.Property(c => c.Phone)
            .HasMaxLength(20);

        builder.ComplexProperty(c => c.Document, cpf =>
        {
            cpf.Property(x => x.Value)
                .HasColumnName("Document")
                .IsRequired()
                .HasMaxLength(11);
        });

        builder.Property(c => c.BirthDate);

        builder.Property(c => c.Role)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.OwnsMany(c => c.Addresses, adress =>
        {
            adress.ToTable("CustomerAddresses");

            adress.WithOwner().HasForeignKey("CustomerId");

            adress.Property(a => a.Street).IsRequired().HasMaxLength(200);
            adress.Property(a => a.Number).IsRequired().HasMaxLength(10);
            adress.Property(a => a.Complement).HasMaxLength(100);
            adress.Property(a => a.Neighborhood).IsRequired().HasMaxLength(100);
            adress.Property(a => a.City).IsRequired().HasMaxLength(100);
            adress.Property(a => a.State).IsRequired().HasMaxLength(2);
            adress.Property(a => a.ZipCode).IsRequired().HasMaxLength(8);
            adress.Property(a => a.Country).IsRequired().HasMaxLength(50);
        });

        builder.Ignore(c => c.Orders);

        builder.Property(c => c.CreatedAt).IsRequired();
    }
}