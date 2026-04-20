namespace GoodHamburger.Api.Entities;

public sealed class Order
{
    public Guid Id { get; init; }
    
    public decimal Subtotal { get; private set; } //Soma dos itens antes da aplicação de desconto.


    public decimal DiscountPercentage { get; private set; } // Percentual aplicado conforme a combinação do pedido.


    public decimal DiscountAmount { get; private set; } // Valor monetário abatido do subtotal.


    public decimal Total { get; private set; } // Valor final do pedido após o desconto.


    public DateTimeOffset CreatedAt { get; private set; }


    public DateTimeOffset UpdatedAt { get; private set; }


    public ICollection<OrderItem> Items { get; private set; } = [];

    private Order()
    {
    }

    public Order(
        Guid id,
        decimal subtotal,
        decimal discountPercentage,
        decimal discountAmount,
        decimal total,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt,
        IEnumerable<OrderItem>? items = null)
    {
        Id = id;
        Subtotal = subtotal;
        DiscountPercentage = discountPercentage;
        DiscountAmount = discountAmount;
        Total = total;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        Items = items?.ToList() ?? [];
    }

    public void Update(
        decimal subtotal,
        decimal discountPercentage,
        decimal discountAmount,
        decimal total,
        DateTimeOffset updatedAt,
        IEnumerable<OrderItem> items)
    {
        Subtotal = subtotal;
        DiscountPercentage = discountPercentage;
        DiscountAmount = discountAmount;
        Total = total;
        UpdatedAt = updatedAt;
        Items = items.ToList();
    }
}
