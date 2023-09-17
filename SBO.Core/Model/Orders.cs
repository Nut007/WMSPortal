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

    public partial class Orders : ViewInformation
    {
        [NonStored]
        public Nullable<int> Gennumber { get; set; }
        [NonStored]
        public string WarehouseKey { get; set; }
        [KeyProperty(Identity = false)]
        public string OrderKey { get; set; }
        public string StorerKey { get; set; }
        [NonStored]
        public string StorerName { get; set; }
        public string ExternOrderKey { get; set; }
        public string ShippingInstructions1 { get; set; }
       
        public DateTime OrderDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string Priority { get; set; }
        public string ConsigneeKey { get; set; }
        public string C_contact1 { get; set; }
        public string C_Contact2 { get; set; }
        public string C_Company { get; set; }
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
        public string BuyerPO { get; set; }
        public string BillToKey { get; set; }
        public string B_contact1 { get; set; }
        public string B_Contact2 { get; set; }
        public string B_Company { get; set; }
        public string B_Address1 { get; set; }
        public string B_Address2 { get; set; }
        public string B_Address3 { get; set; }
        public string B_Address4 { get; set; }
        public string B_City { get; set; }
        public string B_State { get; set; }
        public string B_Zip { get; set; }
        public string B_Country { get; set; }
        public string B_ISOCntryCode { get; set; }
        public string B_Phone1 { get; set; }
        public string B_Phone2 { get; set; }
        public string B_Fax1 { get; set; }
        public string B_Fax2 { get; set; }
        public string B_Vat { get; set; }
        public string IncoTerm { get; set; }
        public string PmtTerm { get; set; }
        public string Door { get; set; }
        [NonStored]
        public string SortationLocation { get; set; }
        [NonStored]
        public string BatchFlag { get; set; }
        [NonStored]
        public string BulkCartonGroup { get; set; }
        public string Route { get; set; }
        public string Stop { get; set; }
        [NonStored]
        public Nullable<int> OpenQty { get; set; }
        [NonStored]
        public string Status { get; set; }
        public string DischargePlace { get; set; }
        public string DeliveryPlace { get; set; }
        [NonStored]
        public string IntermodalVehicle { get; set; }
        [NonStored]
        public string CountryOfOrigin { get; set; }
        [NonStored]
        public string CountryDestination { get; set; }
        [NonStored]
        public string UpdateSource { get; set; }
        [NonStored]
        public string Type { get; set; }
        [NonStored]
        public string OrderGroup { get; set; }
        public string Notes { get; set; }
        public System.DateTime EffectiveDate { get; set; }
        [NonStored]
        public string Stage { get; set; }
        [NonStored]
        public string DC_ID { get; set; }
        [NonStored]
        public string WHSE_ID { get; set; }
        [NonStored]
        public string SPLIT_ORDERS { get; set; }
        [NonStored]
        public string APPT_STATUS { get; set; }
        [NonStored]
        public string CHEPPALLETINDICATOR { get; set; }
        [NonStored]
        public string CONTAINERTYPE { get; set; }
        [NonStored]
        public Nullable<int> CONTAINERQTY { get; set; }
        [NonStored]
        public Nullable<int> BILLEDCONTAINERQTY { get; set; }
        [NonStored]
        public string TRANSPORTATIONMODE { get; set; }
        [NonStored]
        public string TRANSPORTATIONSERVICE { get; set; }
        [NonStored]
        public string EXTERNALORDERKEY2 { get; set; }
        [NonStored]
        public string C_EMAIL1 { get; set; }
        [NonStored]
        public string C_EMAIL2 { get; set; }
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
        public string NOTES2 { get; set; }
        [NonStored]
        public Nullable<decimal> Item_number { get; set; }
        [NonStored]
        public string forte_flag { get; set; }
        [NonStored]
        public string LOADID { get; set; }
        [NonStored]
        public string SHIPTOGETHER { get; set; }
        [NonStored]
        public Nullable<System.DateTime> DELIVERYDATE2 { get; set; }
        [NonStored]
        public Nullable<System.DateTime> REQUESTEDSHIPDATE { get; set; }
        [NonStored]
        public Nullable<System.DateTime> ACTUALSHIPDATE { get; set; }
        [NonStored]
        public Nullable<System.DateTime> DELIVER_DATE { get; set; }
        [NonStored]
        public Nullable<decimal> ORDERVALUE { get; set; }
        [NonStored]
        public string OHType { get; set; }
        [NonStored]
        public string EXTERNALLOADID { get; set; }
        public System.DateTime AddDate { get; set; }
        public string AddWho { get; set; }
        public System.DateTime EditDate { get; set; }
        public string EditWho { get; set; }
        [NonStored]
        public string TrafficCop { get; set; }
        [NonStored]
        public string ArchiveCop { get; set; }
        [NonStored]
        public string Reason { get; set; }
        [NonStored]
        public string Remarks { get; set; }
        [NonStored]
        public string SOStatus { get; set; }
        [NonStored]
        public Nullable<double> GrossWeight { get; set; }
        [NonStored]
        public Nullable<double> Capacity { get; set; }
        [NonStored]
        public string ExportStatus { get; set; }
        [NonStored]
        public string DeliveryReference { get; set; }
        [NonStored]
        public string SalesPerson { get; set; }
        [NonStored]
        public string SelfCollectionFlag { get; set; }
        [NonStored]
        public string Remark1 { get; set; }
        [NonStored]
        public string Remark2 { get; set; }
        [NonStored]
        public string Remark3 { get; set; }
        [NonStored]
        public string Remark4 { get; set; }
        [NonStored]
        public string Remark5 { get; set; }
        [NonStored]
        public string Remark6 { get; set; }
        [NonStored]
        public string so { get; set; }
        [NonStored]
        public string mode { get; set; }
        [NonStored]
        public string Flag1 { get; set; }
        [NonStored]
        public string Flag2 { get; set; }
        [NonStored]
        public string webtype { get; set; }
        [NonStored]
        public string DeliveryType { get; set; }
        public string ShiptoKey { get; set; }
        [NonStored]
        public Nullable<int> Length { get; set; }
        [NonStored]
        public Nullable<int> Width { get; set; }
        [NonStored]
        public Nullable<int> Height { get; set; }
        [NonStored]
        public Nullable<int> Carton { get; set; }
        [NonStored]
        public string TransactionCategory { get; set; }
        [NonStored]
        public string CustomerOrderNumber { get; set; }
        [NonStored]
        public string InternalOrderNumber { get; set; }
        [NonStored]
        public string ReleaseNumber { get; set; }
        [NonStored]
        public Nullable<double> NoOfPallet { get; set; }
        [NonStored]
        public string MBOLKey { get; set; }
        [NonStored]
        public Nullable<double> GrossWeight1 { get; set; }
        public string B_Company1 { get; set; }
        [NonStored]
        public string B_Company2 { get; set; }
        [NonStored]
        public string B_Region { get; set; }
        [NonStored]
        public string C_Company1 { get; set; }
        [NonStored]
        public string C_Company2 { get; set; }
        [NonStored]
        public string C_Region { get; set; }
        [NonStored]
        public string ShipToKey1 { get; set; }
        [NonStored]
        public string S_Company { get; set; }
        [NonStored]
        public string S_Company1 { get; set; }
        [NonStored]
        public string S_Company2 { get; set; }
        [NonStored]
        public string S_Address1 { get; set; }
        [NonStored]
        public string S_Address2 { get; set; }
        [NonStored]
        public string S_City { get; set; }
        [NonStored]
        public string S_Region { get; set; }
        [NonStored]
        public string S_State { get; set; }
        [NonStored]
        public string S_Zip { get; set; }
        [NonStored]
        public string S_Country { get; set; }
        [NonStored]
        public string S_ISOCntryCode { get; set; }
        [NonStored]
        public string KK { get; set; }
        [NonStored]
        public string PE { get; set; }
        [NonStored]
        public string PH { get; set; }
        [NonStored]
        public string PO { get; set; }
        [NonStored]
        public string SO1 { get; set; }
        [NonStored]
        public string SR { get; set; }
        [NonStored]
        public string TC { get; set; }
        [NonStored]
        public string Mode1 { get; set; }
        [NonStored]
        public string FOBPoint { get; set; }
        [NonStored]
        public string SCAC { get; set; }
        [NonStored]
        public string PlantNumber { get; set; }
        [NonStored]
        public Nullable<int> TotalOrderQty { get; set; }
        [NonStored]
        public string RoutingNotes { get; set; }
        [NonStored]
        public string ShipFromKey { get; set; }
        [NonStored]
        public Nullable<double> FreightCharges { get; set; }
        [NonStored]
        public string FreightCurrency { get; set; }
        [NonStored]
        public string Flag11 { get; set; }
        [NonStored]
        public string Flag21 { get; set; }
        [NonStored]
        public string Flag3 { get; set; }
        [NonStored]
        public string Flag4 { get; set; }
        [NonStored]
        public string HAWB { get; set; }
        [NonStored]
        public string Flight { get; set; }
        [NonStored]
        public Nullable<System.DateTime> Date1 { get; set; }
        public Nullable<System.DateTime> Date2 { get; set; }
        [NonStored]
        public string ConsigneeAddress()
        {
            return C_Address1;
        }
        [NonStored]
        public IEnumerable<Codelkup> StorerList { get; set; }
    }
}
