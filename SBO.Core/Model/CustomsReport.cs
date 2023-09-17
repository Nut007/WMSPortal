using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSPortal.Core.Model
{
    public class ImportDeclarationReport
    {
        public string ReceiptKey { get; set; }
        public string ImportDeclarationNo { get; set; }
        public string ImportDeclarationItemNo { get; set; }
        public string ImporterName { get; set; }
        public DateTime ImportDeclarationDate { get; set; }
        public DateTime WarehouseReceivedDate { get; set; }
        public DateTime CustomsPermitDate { get; set; }
        public string Sku { get; set; }
        public string SkuDescription { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string UOM { get; set; }
        public string Id { get; set; }
        public decimal NetWgt { get; set; }
        public decimal GrossWgt { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalDuty { get; set; }
        public decimal TotalDutyPaid { get; set; }
        public decimal DutyUnit { get; set; }
        public string ReportType { get; set; }
        public string Invoice { get; set; }
        public string Taxincentive { get; set; }
        public string Remark { get; set; }

    }
    public class ExportDeclarationReport
    {
        public string OrderKey { get; set; }
        public string ExportDeclarationNo { get; set; }
        public string ExportDeclarationItemNo { get; set; }
        public string ExportName { get; set; }
        public DateTime GoodsLoadedDate { get; set; }
        public DateTime ExportDeclarationDate { get; set; }
        public DateTime WarehouseOrderdDate { get; set; }
        public string Sku { get; set; }
        public string SkuDescription { get; set; }
        public string AltSkuDescription { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string UOM { get; set; }
        public decimal NetWgt { get; set; }
        public decimal GrossWgt { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalDuty { get; set; }
        public decimal TotalDutyPaid { get; set; }
        public decimal ExportDutyUnit { get; set; }
        public Int32 InvoiceItemNo { get; set; }
        public string ImportDeclarationNo { get; set; }
        public string ImportDeclarationItemNo { get; set; }
        public string Id { get; set; }
        public string ReportType { get; set; }
        public string ExportInvoiceNo { get; set; }

    }
    public class GLDeclarationReport
    {
        public string ReceiptKey { get; set; }
        public string ImportDeclarationNo { get; set; }
        public string ImportDeclarationItemNo { get; set; }
        public string ImporterName { get; set; }
        public DateTime ImportDeclarationDate { get; set; }
        public DateTime WarehouseReceivedDate { get; set; }
        public string TariffCode { get; set; }
        public string BinLocation { get; set; }
        public string CameraNo { get; set; }
        public string CameraLink { get; set; }
        public string Sku { get; set; }
        public string SkuDescription { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal NetWgt { get; set; }
        public decimal GrossWgt { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderKey { get; set; }
        public string UOM { get; set; }
        public string ExportDeclarationNo { get; set; }
        public string ExportDeclarationItemNo { get; set; }
        public DateTime? ExportDeclarationDate { get; set; }
        public decimal QtyPicked { get; set; }
        public decimal BroughtForward { get; set; }
        public decimal TotalDuty { get; set; }
        public decimal TotalDutyPaid { get; set; }
        public decimal QtyBalance { get; set; }
        public string PackageUnit { get; set; }
        public string Id { get; set; }

    }
    public class InventoryMovementReport
    {
        public IEnumerable<GLDeclarationReport> BroughtForwardItems { get; set; }
        public IEnumerable<ImportDeclarationReport> ImportDeclarationItems { get; set; }
        public IEnumerable<ExportDeclarationReport> ExportDeclarationItems { get; set; }
        public IEnumerable<GLDeclarationReport> GLDeclarationItems { get; set; }
    }

}
