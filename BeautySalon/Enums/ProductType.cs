using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Enums
{
    [Flags]
    public enum ProductType
    {
        None = 0,
        Tools = 1,
        DecorativeСosmetics = 2,
        CareСosmetics = 4
    }
}
