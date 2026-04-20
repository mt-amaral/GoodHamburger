using System.ComponentModel;

namespace GoodHamburger.Shared.Dto.Enums;

public enum AccompanimentType : byte
{
    [Description("Acompanhamento")]
    Side = 1,

    [Description("Bebida")]
    Drink = 2
}
