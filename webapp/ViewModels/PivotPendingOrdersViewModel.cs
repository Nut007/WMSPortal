using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WMSPortal.ViewModels
{
    public class PivotPendingOrdersViewModel
    {
        public DateTime OrderDate { get; set; }
        public Int64 QtyAllocated { get; set; }
        public string Status { get; set; }
        public string Sku { get; set; }
        public string ExternOrderKey { get; set; }
        public string SkuDescription { get; set; }
        
    }
}