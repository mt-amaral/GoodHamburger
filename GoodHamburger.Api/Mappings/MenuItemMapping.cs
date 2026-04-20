using GoodHamburger.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoodHamburger.Api.Mappings;

public sealed class MenuItemMapping : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.HasKey(menuItem => menuItem.Id);

        builder.Property(menuItem => menuItem.Id)
            .ValueGeneratedNever();

        builder.Property(menuItem => menuItem.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(menuItem => menuItem.Category)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(menuItem => menuItem.AccompanimentType)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired(false);

        builder.Property(menuItem => menuItem.Price)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(menuItem => menuItem.ImageUrl)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(menuItem => menuItem.IsActive)
            .IsRequired();
    }
}
