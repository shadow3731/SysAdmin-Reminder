using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysAdmin_Remider
{
    internal class Status
    {
        public static string Running { get; set; } = "В процессе";
        public static string Finished { get; set; } = "Завершён";
        public static string Cancelled { get; set; } = "Отменён";
        public static string[] All { get; } = { Running, Finished, Cancelled };
    }
}
