using System.ComponentModel;

namespace GoodHamburger.Shared.Dto.Enums;

public enum MenuItemCategory : byte
{
    [Description("Sanduiche")]
    Sandwich = 1,

    [Description("Acompanhamento")]
    Accompaniment = 2
}
