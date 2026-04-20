using Microsoft.AspNetCore.Components;

namespace GoodHamburger.Layout;

public partial class MainLayout
{
    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    private bool IsOrdersListPage => Navigation.Uri.Contains("/orders/list", StringComparison.OrdinalIgnoreCase);

    protected string HeaderActionLabel => IsOrdersListPage ? "Novo Pedido" : "Ver Pedidos";

    protected string HeaderActionHref => IsOrdersListPage ? "/orders" : "/orders/list";
}
