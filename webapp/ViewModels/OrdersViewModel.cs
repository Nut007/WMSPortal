using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WMSPortal.Core.Model;

namespace WMSPortal.ViewModels
{
    public class OrdersViewModel : ViewInformation
    {
        [DisplayName("Warehouse")]
        public string WarehouseKey { get; set; }
        [DisplayName("Order No")]
        public string OrderKey { get; set; }
        [DisplayName("Line")]
        public string OrderLineNumber { get; set; }
        [DisplayName("Storer")]
        public string StorerKey { get; set; }
        [DisplayName("StorerName")]
        public string StorerName { get; set; }
        [DisplayName("Address")]
        public string SellerAddress { get; set; }
        [DisplayName("Extern Order")]
        public string ExternOrderKey { get; set; }
        [DisplayName("Order Group")]
        public string OrderGroup { get; set; }
        [DisplayName("Order Date")]
        public DateTime? OrderDate { get; set; }
        [DisplayName("Delivery Date")]
        public DateTime? DeliveryDate { get; set; }
        [DisplayName("Consignee")]
        public string ConsigneeKey { get; set; }
        [DisplayName("Consignee Name")]
        public string C_Company { get; set; }
        [DisplayName("Address")]
        public string ConsigneeAddress { get; set; }
        [DisplayName("Ship To")]
        public string ShipToKey { get; set; }
        [DisplayName("Address")]
        public string ShipToAddress { get; set; }
        [DisplayName("Sku")]
        public string Sku { get; set; }
        [DisplayName("Description")]
        public string SkuDescription { get; set; }
        [DisplayName("Open")]
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
        public string Priority { get; set; }
        public string C_contact1 { get; set; }
        public string C_Contact2 { get; set; }
        public string C_Address1 { get; set; }
        public string C_Address2 { get; set; }
        public string C_Address3 { get; set; }
        public string C_Address4 { get; set; }
        public string C_City { get; set; }
        public string C_State { get; set; }
        public string C_Zip { get; set; }
        public string C_Country { get; set; }
        public string C_ISOCntryCode { get; set; }
        public string C_Phone1 { get; set; }
        public string C_Phone2 { get; set; }
        public string C_Fax1 { get; set; }
        public string C_Fax2 { get; set; }
        public string C_vat { get; set; }
        public string C_Email1 { get; set; }
        public string ShippingInstructions1 { get; set; }
        public string IntermodalVehicle { get; set; }
        public string DeliveryPalce { get; set; }
        public string BuyerPO { get; set; }
        public string Notes1 { get; set; }
        public string Notes { get; set; }
        public string Flag3 { get; set; }
        public IEnumerable<Codelkup> PriorityList { get; set; }
        public IEnumerable<Storer> ConsigneeList { get; set; }
        public IEnumerable<Storer> StorerList { get; set; }
        public IEnumerable<OrderDetail> OrderItems { get; set; }
      
    }
}