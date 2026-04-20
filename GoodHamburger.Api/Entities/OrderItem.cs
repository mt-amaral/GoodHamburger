using GoodHamburger.Shared.Dto.Enums;

namespace GoodHamburger.Api.Entities;

public sealed class OrderItem
{
    public Guid Id { get; init; }

    public Guid OrderId { get; private set; }

    public int MenuItemId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public MenuItemCategory Category { get; private set; }

    public AccompanimentType? AccompanimentType { get; private set; }

    public decimal UnitPrice { get; private set; }

    public Order Order { get; private set; } = null!;

    public MenuItem MenuItem { get; private set; } = null!;

    private OrderItem()
    {
    }

    public OrderItem(
        Guid id,
        Guid orderId,
        int menuItemId,
        string name,
        MenuItemCategory category,
        AccompanimentType? accompanimentType,
        decimal unitPrice)
    {
        Id = ValidateRequiredGuid(id, nameof(id));
        OrderId = ValidateRequiredGuid(orderId, nameof(orderId));
        MenuItemId = ValidateMenuItemId(menuItemId);
        SetValues(name, category, accompanimentType, unitPrice);
    }

    private void SetValues(
        string name,
        MenuItemCategory category,
        AccompanimentType? accompanimentType,
        decimal unitPrice)
    {
        var validatedCategory = ValidateCategory(category);

        Name = ValidateRequiredText(name, nameof(name));
        Category = validatedCategory;
        AccompanimentType = ValidateAccompanimentType(validatedCategory, accompanimentType);
        UnitPrice = ValidatePrice(unitPrice, nameof(unitPrice));
    }

    private static Guid ValidateRequiredGuid(Guid value, string paramName)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("The value cannot be an empty guid.", paramName);
        }

        return value;
    }

    private static int ValidateMenuItemId(int menuItemId)
    {
        if (menuItemId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(menuItemId), "The menu item id must be greater than zero.");
        }

        return menuItemId;
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
            throw new ArgumentOutOfRangeException(nameof(category), category, "The order item category is invalid.");
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

    private static decimal ValidatePrice(decimal unitPrice, string paramName)
    {
        if (unitPrice < 0)
        {
            throw new ArgumentOutOfRangeException(paramName, unitPrice, "The unit price cannot be negative.");
        }

        return unitPrice;
    }
}
