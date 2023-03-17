using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SysAdmin_Remider
{
    internal class Task
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public DateTime ExecutionTime { get; set; }
        public object Priority { get; set; }

        public Task(long Id, string Description, DateTime ExecutionTime, object Priority)
        {
            this.Id = Id;
            this.Description = Description;
            this.ExecutionTime = ExecutionTime;
            this.Priority = Priority;
        }

        public Task() { }

        public override string ToString()
        {
            return $"{Id} {Description} {ExecutionTime} {Priority}";
        }
    }
}
