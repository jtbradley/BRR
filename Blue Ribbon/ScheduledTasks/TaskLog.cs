using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blue_Ribbon.ScheduledTasks
{
    public class TaskLog
    {
        public int TaskLogId { get; set; }
        public string TaskDescription { get; set; }
        public DateTime SuccessDatestamp { get; set; }
    }
}