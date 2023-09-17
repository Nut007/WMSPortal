using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WMSPortal.ViewModels
{
    public class InboundTransReportViewModels
    {
        public string ReceiptKey { get; set; }
        public string ImportDeclarationNo { get; set; }
        public string ImportDeclarationItemNo { get; set; }
        public string ImporterName { get; set; }
        public DateTime ImportDeclarationDate { get; set; }
        public DateTime WarehouseReceivedDate { get; set; }
        public string Sku { get; set; }
        public string SkuDescription { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal NetWgt { get; set; }
        public decimal GrossWgt { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DutyAmount { get; set; }
        public string OrderKey { get; set; }
        public string UOM { get; set; }
        public string ExportDeclarationNo { get; set; }
        public string ExportDeclarationItemNo { get; set; }
        public DateTime ExportDeclarationDate { get; set; }
        public decimal QtyPicked { get; set; }
        public decimal BroughtForward { get; set; }
        public decimal TotalCustomsTax { get; set; }
        public decimal QtyBalance { get; set; }
        public string PackageUnit { get; set; }
        public string Id { get; set; }

    }

}