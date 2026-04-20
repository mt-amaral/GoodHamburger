using GoodHamburger.Shared.Dto.Enums;

namespace GoodHamburger.Api.Entities;

public sealed class MenuItem
{
    public int Id { get; init; }

    public string Name { get; private set; } = string.Empty;

    public MenuItemCategory Category { get; private set; }

    public AccompanimentType? AccompanimentType { get; private set; }

    public decimal Price { get; private set; }

    public string ImageUrl { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    private MenuItem()
    {
    }

    public MenuItem(
        int id,
        string name,
        MenuItemCategory category,
        AccompanimentType? accompanimentType,
        decimal price,
        string imageUrl,
        bool isActive = true)
    {
        Id = ValidateId(id);
        SetValues(name, category, accompanimentType, price, imageUrl, isActive);
    }

    public void ApplySeedData(
        string name,
        MenuItemCategory category,
        AccompanimentType? accompanimentType,
        decimal price,
        string imageUrl,
        bool isActive)
    {
        SetValues(name, category, accompanimentType, price, imageUrl, isActive);
    }

    private void SetValues(
        string name,
        MenuItemCategory category,
        AccompanimentType? accompanimentType,
        decimal price,
        string imageUrl,
        bool isActive)
    {
        var validatedCategory = ValidateCategory(category);

        Name = ValidateRequiredText(name, nameof(name));
        Category = validatedCategory;
        AccompanimentType = ValidateAccompanimentType(validatedCategory, accompanimentType);
        Price = ValidatePrice(price, nameof(price));
        ImageUrl = ValidateRequiredText(imageUrl, nameof(imageUrl));
        IsActive = isActive;
    }

    private static int ValidateId(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "The menu item id must be greater than zero.");
        }

        return id;
    }

    private static string ValidateRequiredText(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("The value cannot be null or whitespace.", paramName);
        }

        return value.Trim();
    }

    private static MenuItemCategory ValidateCategory(MenuItemCategory category)
    {
        if (!Enum.IsDefined(category))
        {
            throw new ArgumentOutOfRangeException(nameof(category), category, "The menu item category is invalid.");
        }

        return category;
    }

    private static AccompanimentType? ValidateAccompanimentType(
        MenuItemCategory category,
        AccompanimentType? accompanimentType)
    {
        if (accompanimentType.HasValue && !Enum.IsDefined(accompanimentType.Value))
        {
            throw new ArgumentOutOfRangeException(
                nameof(accompanimentType),
                accompanimentType,
                "The accompaniment type is invalid.");
        }

        if (category == MenuItemCategory.Sandwich && accompanimentType.HasValue)
        {
            throw new ArgumentException(
                "Sandwich items cannot define an accompaniment type.",
                nameof(accompanimentType));
        }

        if (category == MenuItemCategory.Accompaniment && !accompanimentType.HasValue)
        {
            throw new ArgumentException(
                "Accompaniment items must define an accompaniment type.",
                nameof(accompanimentType));
        }

        return accompanimentType;
    }

    private static decimal ValidatePrice(decimal price, string paramName)
    {
        if (price < 0)
        {
            throw new ArgumentOutOfRangeException(paramName, price, "The price cannot be negative.");
        }

        return price;
    }

}
