using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace WMSPortal.ViewModels
{
    public class ReceiptListViewModel
    {
        [DisplayName("WH#")]
        public string WarehouseKey { get; set; }
        [DisplayName("Receipt No")]
        public string ReceiptKey { get; set; }
        [DisplayName("Line")]
        public string ReceiptLineNumber { get; set; }
        [DisplayName("Pack")]
        public string PackKey { get; set; }
        [DisplayName("UOM")]
        public string UOM { get; set; }
        [DisplayName("Date Received")]
        public System.DateTime DateReceived { get; set; }
        public Nullable<System.DateTime> ReceiptDate { get; set; }
        [DisplayName("ID")]
        public string ToId { get; set; }
        [DisplayName("Location")]
        public string ToLoc { get; set; }
        [DisplayName("Storer")]
        public string StorerKey { get; set; }
        [DisplayName("Status")]
        public string ReceiptType { get; set; }
        [DisplayName("PO#")]
        public string PoKey { get; set; }
        [DisplayName("Customer Ref#")]
        public string ExternReceiptKey { get; set; }
        
        [DisplayName("Invoice No")]
        public string WarehouseReference { get; set; }
        public string Sku { get; set; }
        [DisplayName("Description")]
        public string SkuDescription { get; set; }

        [DisplayName("Expected")]
        public decimal QtyExpected { get; set; }
        [DisplayName("Received")]
        public decimal QtyReceived { get; set; }
        [DisplayName("Import Entry")]
        public string Reference1 { get; set; }
        [DisplayName("Invoice#")]
        public string Reference8 { get; set; }
        [DisplayName("Unit Price")]
        public Nullable<double> UnitPrice { get; set; }
        [DisplayName("Net Wt/Unit")]
        public Nullable<double> NetWgt { get; set; }
        [DisplayName("Gross Wt/Unit")]
        public Nullable<double> GrossWgt { get; set; }

    }
}