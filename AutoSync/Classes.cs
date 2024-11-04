using CheckmateDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classes
{
    public class SyncStatus
    {
        public Guid ProjectGuid { get; set; }
        public string Project { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public frmSync_Status frmSyncStatus { get; set; }

        //need to run twice to fully sync
        public int DailyRunCount { get; set; }
    }

    public class SyncProject
    {
        public Guid Guid { get; set; }
        public string Number { get; set; }
        public bool IsSync { get; set; }
    }
}
