using GoodHamburger.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoodHamburger.Api.Mappings;

public sealed class OrderItemMapping : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(orderItem => orderItem.Id);

        builder.Property(orderItem => orderItem.Id)
            .ValueGeneratedNever();

        builder.Property(orderItem => orderItem.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(orderItem => orderItem.Category)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(orderItem => orderItem.AccompanimentType)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired(false);

        builder.Property(orderItem => orderItem.UnitPrice)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.HasIndex(orderItem => new { orderItem.OrderId, orderItem.Category, orderItem.AccompanimentType })
            .IsUnique();

        builder.HasOne(orderItem => orderItem.MenuItem)
            .WithMany()
            .HasForeignKey(orderItem => orderItem.MenuItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
