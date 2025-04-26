using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Enums
{
    public enum OrderStatus
    {
        Draft = 0, // редактируем
        Confirmed = 1, // принят
        Cancelled = 2, // отменен
        Completed = 3 // завершен
    }
}
