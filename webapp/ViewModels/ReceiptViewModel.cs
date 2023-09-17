using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WMSPortal.Core.Model;

namespace WMSPortal.ViewModels
{
    public class ReceiptViewModel
    {
        public string WarehouseKey { get; set; }
        public string ReceiptKey { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public string StorerKey { get; set; }
        public string StorerName { get; set; }
        public string PoKey { get; set; }
        public string WarehouseReference { get; set; }
        public string CarrierReference { get; set; }
        public string VehicleNumber { get; set; }
        public string Sku { get; set; }
        public string Reference1 { get; set; }
        public decimal QtyExpected { get; set; }
        public decimal QtyReceived { get; set; }
        
    }
}