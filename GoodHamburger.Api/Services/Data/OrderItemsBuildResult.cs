using GoodHamburger.Api.Entities;

namespace GoodHamburger.Api.Services.Data;

public sealed record OrderItemsBuildResult(
    Guid OrderId,
    List<OrderItem>? Items,
    string? Error);
