using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysAdmin_Remider
{
    internal class Priority
    {
        public static string Little { get; } = "Незначительный";
        public static string Low { get; } = "Низкий";
        public static string Medium { get; } = "Средний";
        public static string High { get; } = "Высокий";
        public static string Critical { get; } = "Критический";
        public static string[] All { get; } = { Little, Low, Medium, High, Critical };
    }
}
