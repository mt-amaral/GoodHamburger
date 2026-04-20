using GoodHamburger.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoodHamburger.Api.Mappings;

public sealed class OrderMapping : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {

        builder.HasKey(order => order.Id);

        builder.Property(order => order.Id)
            .ValueGeneratedNever();

        builder.Property(order => order.Subtotal)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(order => order.DiscountPercentage)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(order => order.DiscountAmount)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(order => order.Total)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(order => order.CreatedAt)
            .IsRequired();

        builder.Property(order => order.UpdatedAt)
            .IsRequired();

        builder.HasMany(order => order.Items)
            .WithOne(orderItem => orderItem.Order)
            .HasForeignKey(orderItem => orderItem.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
