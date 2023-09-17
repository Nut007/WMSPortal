using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using WMSPortal.Core.Model;

namespace WMSPortal.ViewModels
{
    public class StockBalanceViewModel
    {
        public Nullable<int> Gennumber { get; set; }
        public string WarehouseKey { get; set; }
        [DisplayName("Status")]
        public string ReceiptType { get; set; }
        public string Lot { get; set; }
        [DisplayName("Location")]
        public string Loc { get; set; }
        public string Id { get; set; }
        public string StorerKey { get; set; }
        public string Sku { get; set; }
        public string SkuDescription { get; set; }
        [DisplayName("Total Qty")]
        public int Qty { get; set; }
        [DisplayName("Allocated")]
        public int QtyAllocated { get; set; }
        [DisplayName("Picked")]
        public int QtyPicked { get; set; }
        public int QtyExpected { get; set; }

        [DisplayName("Avaliable")]
        public int QtyAvaliable{ get; set; }
        public int QtyPickInProcess { get; set; }
        public int PendingMoveIN { get; set; }
        public int ArchiveQty { get; set; }
        public System.DateTime ArchiveDate { get; set; }
        public string Status { get; set; }
        public System.DateTime AddDate { get; set; }
        public string AddWho { get; set; }
        public System.DateTime EditDate { get; set; }
        public string EditWho { get; set; }
        public string TrafficCop { get; set; }
        public string ArchiveCop { get; set; }
        public Nullable<double> ArchiveGrossWgt { get; set; }
        public Nullable<double> ArchiveCube { get; set; }
        public string Lottable01 { get; set; }
        public string Lottable02 { get; set; }
        public string Lottable03 { get; set; }
        public DateTime? Lottable04 { get; set; }
        public DateTime? Lottable05 { get; set; }
        public string Lottable06 { get; set; }
        public string Lottable07 { get; set; }
        public string Lottable08 { get; set; }
        public string Lottable09 { get; set; }
        public string Lottable10 { get; set; }

        [DisplayName("Unit Price")]
        public Nullable<double> UnitPrice { get; set; }
        [DisplayName("Net Wt/Unit")]
        public Nullable<double> NetWgt { get; set; }
        [DisplayName("Gross Wt/Unit")]
        public Nullable<double> GrossWgt { get; set; }
        public string ImportEntry { get; set; }
        [DisplayName("Package No")]
        public string PcNo { get; set; }
        public int QtyOrder { get; set; }
        
    }
}