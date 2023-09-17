using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace WMSPortal.ViewModels
{
    public class ConsignmentListViewModel
    {
        public string Status { get; set; }
        public Int64 ShptID { get; set; }
        [DisplayName("Consignment ID")]
        public string ConsignmentID { get; set; }
        public string Product { get; set; }
        public Int32 Priority { get; set; }
        public string GPA { get; set; }
        public DateTime Issue_Date { get; set; }
        public string DN { get; set; }
        public string Cmdty { get; set; }
        public string Origin { get; set; }
        public string Dest { get; set; }
        public decimal Peice { get; set; }
        public decimal Weight { get; set; }
        public decimal Vol { get; set; }
        [DisplayName("1st Flight")]
        public string Flight_BKK { get; set; }
        [DisplayName("1st Departed")]
        public DateTime Date_BKK { get; set; }
        [DisplayName("1st Status")]
        public string MVT_BKK { get; set; }
        [DisplayName("2nd Flight")]
        public string Flight_DOH { get; set; }
        [DisplayName("2nd Departed")]
        public DateTime Date_DOH { get; set; }
        [DisplayName("2nd Status")]
        public string MVT_DOH { get; set; }
        public string Mawb { get; set; }
        public string RateType { get; set; }
        public string Remark { get; set; }
        public string ULD { get; set; }
        [DisplayName("Pickup")]
        public DateTime? Pickup_Date { get; set; }

    }
}