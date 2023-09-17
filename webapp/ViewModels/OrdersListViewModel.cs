using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace WMSPortal.ViewModels
{
    public class OrdersListViewModel
    {
        [DisplayName("WH#")]
        public string WarehouseKey { get; set; }
        [DisplayName("Order No")]
        public string OrderKey { get; set; }
        [DisplayName("Line")]
        public string OrderLineNumber { get; set; }
        [DisplayName("Storer")]
        public string StorerKey { get; set; }
        [DisplayName("Loading Point")]
        public string LoadingPoint { get; set; }
        [DisplayName("Customer Ref#")]
        public string ExternOrderKey { get; set; }
        public string BuyerPO { get; set; }
        [DisplayName("Order Date")]
        public DateTime? OrderDate { get; set; }
        [DisplayName("Delivery Date")]
        public DateTime? DeliveryDate { get; set; }
        [DisplayName("Consignee")]
        public string ConsigneeKey { get; set; }
        [DisplayName("Consignee Address")]
        public string ConsigneeAddress { get; set; }
        [DisplayName("Sku")]
        public string Sku { get; set; }
        [DisplayName("Description")]
        public string SkuDescription { get; set; }
        [DisplayName("Qty")]
        public decimal OpenQty { get; set; }
        [DisplayName("Allocated")]
        public decimal QtyAllocated { get; set; }
        [DisplayName("Picked")]
        public decimal QtyPicked { get; set; }
        [DisplayName("Shipped")]
        public decimal ShippedQty { get; set; }
        [DisplayName("Pack")]
        public string PackKey { get; set; }
        [DisplayName("UOM")]
        public string UOM { get; set; }
        [DisplayName("Added Date")]
        public DateTime? AddedDate { get; set; }
        [DisplayName("Edit Date")]
        public DateTime? EditDate { get; set; }

        [DisplayName("Export Entry")]
        public string ShippingInstructions1 { get; set; }
        [DisplayName("Import Entry")]
        public string ImportEntry { get; set; }
        [DisplayName("Import Item no")]
        public string ImportEntryLine { get; set; }

        [DisplayName("Unit Price")]
        public Nullable<double> UnitPrice { get; set; }
        [DisplayName("Net Wt/Unit")]
        public Nullable<double> NetWeight { get; set; }
        [DisplayName("Gross Wt/Unit")]
        public Nullable<double> GrossWeight { get; set; }
        public Nullable<double> Qty { get; set; }
        public string Flag3 { get; set; }
        public string Status { get; set; }
        [DisplayName("Routing")]
        public string RoutingNotes { get; set; }
        [DisplayName("Schedule Pickup")]
        public DateTime? Date5 { get; set; }
        [DisplayName("Ship Confirm Status")]
        public string KK { get; set; }
        
    }
}