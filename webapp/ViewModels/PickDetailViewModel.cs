using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace WMSPortal.ViewModels
{
    public class PickDetailViewModel
    {   
        [DisplayName("Warehouse")]
        public string WarehouseKey { get; set; }
        [DisplayName("Order No")]
        public string OrderKey { get; set; }
        [DisplayName("Line")]
        public string OrderLineNumber { get; set; }
        [DisplayName("Case ID")]
        public string PickDetailKey { get; set; }
        [DisplayName("Lot")]
        public string Lot { get; set; }
        [DisplayName("Location")]
        public string Loc { get; set; }
        [DisplayName("ID")]
        public string ID { get; set; }
        [DisplayName("UOM")]
        public string UOM { get; set; }
        [DisplayName("Picked Qty")]
        public decimal Qty { get; set; }
        [DisplayName("Added Date")]
        public DateTime? AddDate { get; set; }
        [DisplayName("Edit Date")]
        public DateTime? EditDate { get; set; }

    }
}