using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysAdmin_Remider
{
    internal class ServiceMessage
    {
        public static string ApplicationLaunched { get; set; } = null;
        public static string TasksLoaded { get; set; } = null;
        public static string TaskSaving { get; set; } = null;
        public static string TaskAdded { get; set; } = null;
        public static string TaskNotAdded { get; set; } = null;
        public static string TaskUpdated { get; set; } = null;
        public static string TaskRollback { get; set; } = null;
        public static string TaskFinished { get; set; } = null;
        public static string TaskCancelled { get; set; } = null;
    }
}
