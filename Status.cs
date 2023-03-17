using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysAdmin_Remider
{
    internal class Status
    {
        public static string Running { get; } = "Running";
        public static string Finished { get; } = "Finished";
        public static string Cancelled { get; } = "Cancelled";
        public static string[] All { get; } = { Running, Finished, Cancelled };
    }
}
