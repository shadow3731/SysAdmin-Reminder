using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysAdmin_Remider
{
    internal class ServiceMessage
    {
        public static string ApplicationLaunched { get; } = "Приложение запущено.";
        public static string TasksLoaded { get; } = "Активные задания выгружены.";
        public static string TaskAdded { get; } = "Задание добавлено.";
        public static string TaskUpdated { get; } = "Задание обновлено.";
        public static string TaskRollback { get; } = "Задание не обновлено.";
        public static string TaskFinished { get; } = "Задание завершено.";
        public static string TaskCancelled { get; } = "Задание отменено.";
    }
}
