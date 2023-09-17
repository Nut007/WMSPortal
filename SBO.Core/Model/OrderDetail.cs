//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WMSPortal.Core.Model
{
    using MicroOrm.Pocos.SqlGenerator.Attributes;
    using System;
    using System.Collections.Generic;
using System.ComponentModel;
    
    public partial class OrderDetail
    {
      
        [NonStored]
        public Nullable<int> Gennumber { get; set; }
        [NonStored]
        public string WarehouseKey { get; set; }
        [KeyProperty(Identity = false)]
        public string OrderKey { get; set; }
        [KeyProperty(Identity = false)]
        public string OrderLineNumber { get; set; }
        [NonStored]
        public Nullable<int> OrderDetailSysId { get; set; }
        [DefaultValue("")]
        public string ExternOrderKey { get; set; }
        [NonStored]
        public DateTime? DeliveryDate { get; set; }
        [NonStored]
        public DateTime? OrderDate { get; set; }
        [NonStored]
        public string ExternLineNo { get; set; }
        [NonStored]
        public string ShippingInstructions1 { get; set; }
        public string Sku { get; set; }
        [NonStored]
        public string SkuDescription { get; set; }
        public string StorerKey { get; set; }
        [NonStored]
        public string LoadingPoint { get; set; }
        [NonStored]
        public string ManufacturerSku { get; set; }
        [NonStored]
        public string RetailSku { get; set; }
        [NonStored]
        public string AltSku { get; set; }
        [NonStored]
        public int OriginalQty { get; set; }
        public int OpenQty { get; set; }
        [NonStored]
        public int ShippedQty { get; set; }
        [NonStored]
        public int AdjustedQty { get; set; }
        [NonStored]
        public int QtyPreAllocated { get; set; }
        [NonStored]
        public int QtyAllocated { get; set; }
        [NonStored]
        public int QtyPicked { get; set; }
        [NonStored]
        public string UOM { get; set; }
        public string PackKey { get; set; }
        [NonStored]
        public string PickCode { get; set; }
        [NonStored]
        public string CartonGroup { get; set; }
        [NonStored]
        public string Lot { get; set; }
        [NonStored]
        public string ID { get; set; }
        [NonStored]
        public string Facility { get; set; }
        [NonStored]
        public string Status { get; set; }
        [NonStored]
        public double UnitPrice { get; set; }
        public double Tax01 { get; set; }
        [NonStored]
        public double Tax02 { get; set; }
        [NonStored]
        public double ExtendedPrice { get; set; }
        [NonStored]
        public string UpdateSource { get; set; }
        [NonStored]
        public string Lottable01 { get; set; }
        [NonStored]
        public string Lottable02 { get; set; }
        [NonStored]
        public string Lottable03 { get; set; }
        [NonStored]
        public Nullable<System.DateTime> Lottable04 { get; set; }
        [NonStored]
        public Nullable<System.DateTime> Lottable05 { get; set; }
        [NonStored]
        public string Lottable06 { get; set; }
        [NonStored]
        public string Lottable07 { get; set; }
        [NonStored]
        public string Lottable08 { get; set; }
        [NonStored]
        public string Lottable09 { get; set; }
        [NonStored]
        public string Lottable10 { get; set; }
        [NonStored]
        public System.DateTime EffectiveDate { get; set; }
        [NonStored]
        public string forte_flag { get; set; }
        [NonStored]
        public string tariffkey { get; set; }
        [NonStored]
        public string SUSR1 { get; set; }
        [NonStored]
        public string SUSR2 { get; set; }
        [NonStored]
        public string SUSR3 { get; set; }
        [NonStored]
        public string SUSR4 { get; set; }
        [NonStored]
        public string SUSR5 { get; set; }
        [NonStored]
        public string NOTES { get; set; }
        [NonStored]
        public string WORKORDERKEY { get; set; }
        [NonStored]
        public string AllocateStrategyKey { get; set; }
        [NonStored]
        public string PreAllocateStrategyKey { get; set; }
        [NonStored]
        public string AllocateStrategyType { get; set; }
        [NonStored]
        public string SkuRotation { get; set; }
        [NonStored]
        public Nullable<int> ShelfLife { get; set; }
        [NonStored]
        public string Rotation { get; set; }
        [NonStored]
        public string PALLET_ID { get; set; }
        [NonStored]
        public string SUB_FLAG { get; set; }
        [NonStored]
        public double PRODUCT_WEIGHT { get; set; }
        [NonStored]
        public double PRODUCT_CUBE { get; set; }
        [NonStored]
        public int ORIGCASEQTY { get; set; }
        [NonStored]
        public int ORIGPALLETQTY { get; set; }
        [NonStored]
        public int OKTOSUBSTITUTE { get; set; }
        [NonStored]
        public int ISSUBSTITUTE { get; set; }
        [NonStored]
        public string ORIGINALSKU { get; set; }
        [NonStored]
        public string ORIGINALLINENUMBER { get; set; }
        [NonStored]
        public string SHIPGROUP01 { get; set; }
        [NonStored]
        public string SHIPGROUP02 { get; set; }
        [NonStored]
        public string SHIPGROUP03 { get; set; }
        [NonStored]
        public Nullable<System.DateTime> ACTUALSHIPDATE { get; set; }
        [NonStored]
        public string INTERMODALVEHICLE { get; set; }
        [NonStored]
        public string PICKINGINSTRUCTIONS { get; set; }
        [NonStored]
        public System.DateTime AddDate { get; set; }
        public string AddWho { get; set; }
        public System.DateTime EditDate { get; set; }
        public string EditWho { get; set; }
        public string TrafficCop { get; set; }
        public string ArchiveCop { get; set; }
        [NonStored]
        public string MasterSkuFlag { get; set; }
        public Nullable<double> NetWeight { get; set; }
        public Nullable<double> GrossWeight { get; set; }[NonStored]
        public Nullable<double> Capacity { get; set; }
        [NonStored]
        public string DeliveryReference { get; set; }
        [NonStored]
        public string BINLocation { get; set; }
        [NonStored]
        public string Attribute01 { get; set; }
        [NonStored]
        public string ExternTransactionID { get; set; }
        [NonStored]
        public string SerialReqFlag { get; set; }
        [NonStored]
        public string VendorCode { get; set; }
        [NonStored]
        public Nullable<int> AGLOriginalQty { get; set; }
        [NonStored]
        public string ProductionCode { get; set; }
        [NonStored]
        public string ProductGroup { get; set; }
        [NonStored]
        public string PlantNumber { get; set; }
        [NonStored]
        public Nullable<int> QTYREDUCED { get; set; }
        public string Flag1 { get; set; }
        [NonStored]
        public string Flag2 { get; set; }
        [NonStored]
        public Nullable<double> NoOfCarton { get; set; }
        public string OriginCountry { get; set; }
        [NonStored]
        public string Notes1 { get; set; }
        [NonStored]
        public string BINLocation1 { get; set; }
        [NonStored]
        public string POLineNumber { get; set; }
        [NonStored]
        public string DeliveryReference1 { get; set; }
        [NonStored]
        public string LineItemID { get; set; }
        [NonStored]
        public string ProductionCode1 { get; set; }
        [NonStored]
        public string ProductGroup1 { get; set; }
        [NonStored]
        public string ShipFrom { get; set; }
        [NonStored]
        public Nullable<int> BackOrderQty { get; set; }
        [NonStored]
        public string ShipmentStatus { get; set; }
        [NonStored]
        public Nullable<int> AGLOriginalQty1 { get; set; }
        [NonStored]
        public string PlantNumber1 { get; set; }
        [NonStored]
        public Nullable<System.DateTime> PShipDate { get; set; }
        [NonStored]
        public Nullable<System.DateTime> PDeliveryDate { get; set; }
        [NonStored]
        public Nullable<System.DateTime> PArrivalDate { get; set; }
        [NonStored]
        public string Notes11 { get; set; }
        [NonStored]
        public string Notes2 { get; set; }
        [NonStored]
        public string Notes3 { get; set; }
        [NonStored]
        public string Notes4 { get; set; }
        [NonStored]
        public string Notes5 { get; set; }
        [NonStored]
        public string Notes6 { get; set; }
        [NonStored]
        public Nullable<int> QtyPerKit { get; set; }
        [NonStored]
        public string Flag11 { get; set; }
        [NonStored]
        public string Flag21 { get; set; }
        [NonStored]
        public string POLineItemID { get; set; }
        [NonStored]
        public string Instruction { get; set; }
        [NonStored]
        public Nullable<int> QtyReduced1 { get; set; }
        [NonStored]
        public string VendorCode1 { get; set; }
        [NonStored]
        public Nullable<double> TPPrice { get; set; }
        [NonStored]
        public string TPCurrency { get; set; }
        [NonStored]
        public Nullable<double> EndCustomerPrice { get; set; }
        [NonStored]
        public string EndCustomerCurrency { get; set; }
        [NonStored]
        public string CustShipInst01 { get; set; }
        [NonStored]
        public string CustShipInst02 { get; set; }
        [NonStored]
        public string CustShipInst03 { get; set; }
        [NonStored]
        public string CustShipInst04 { get; set; }
        [NonStored]
        public string CustShipInst05 { get; set; }
        [NonStored]
        public string CustWHInst01 { get; set; }
        [NonStored]
        public string CustWHInst02 { get; set; }
        [NonStored]
        public string CustWHInst03 { get; set; }
        [NonStored]
        public string CustWHInst04 { get; set; }
        [NonStored]
        public string CustWHInst05 { get; set; }
        [NonStored]
        public string InternalNote01 { get; set; }
        [NonStored]
        public string InternalNote02 { get; set; }
        [NonStored]
        public string InternalNote03 { get; set; }
        [NonStored]
        public string InternalNote04 { get; set; }
        [NonStored]
        public string InternalNote05 { get; set; }
        [NonStored]
        public string ExternalNote01 { get; set; }
        [NonStored]
        public string ExternalNote02 { get; set; }
        [NonStored]
        public string ExternalNote03 { get; set; }
        [NonStored]
        public string ExternalNote04 { get; set; }
        [NonStored]
        public string ExternalNote05 { get; set; }
        [NonStored]
        public string CTOoption01 { get; set; }
        [NonStored]
        public string CTOoption02 { get; set; }
        [NonStored]
        public string CTOoption03 { get; set; }
        [NonStored]
        public string CTOoption04 { get; set; }
        [NonStored]
        public string CTOoption05 { get; set; }
        [NonStored]
        public string CTOoption06 { get; set; }
        [NonStored]
        public string CTOoption07 { get; set; }
        [NonStored]
        public string CTOoption08 { get; set; }
        [NonStored]
        public string CTOoption09 { get; set; }
        [NonStored]
        public string CTOoption10 { get; set; }
        [NonStored]
        public Nullable<double> GrossWeight1 { get; set; }
        [NonStored]
        public Nullable<double> NetWeight1 { get; set; }
        [NonStored]
        public string UnitWeight { get; set; }
        [NonStored]
        public Nullable<double> Volume { get; set; }
        [NonStored]
        public string UnitVolume { get; set; }
        [NonStored]
        public string Flag3 { get; set; }
        [NonStored]
        public string RoutingNotes { get; set; }
        [NonStored]
        public string KK { get; set; }
        [NonStored]
        public DateTime? Date5 { get; set; }
        
    }
}
