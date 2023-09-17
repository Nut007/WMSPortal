using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSPortal.Core.Model
{
    public class UserLog
    {
        public int LogId { get; set; }        
        public string LoginName { get; set; }
        public DateTime EventDate { get; set; }
        public string EventType { get; set; }
        public string DatabaseName { get; set; }
        public string SqlCommand { get; set; }
        public string EventDescription { get; set; }

    }
}
