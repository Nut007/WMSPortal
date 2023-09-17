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
    using System;
    using System.Collections.Generic;
    
    public partial class Receipt
    {
        public Nullable<int> Gennumber { get; set; }
        public string WarehouseKey { get; set; }
        public string ReceiptKey { get; set; }
        public string ExternReceiptKey { get; set; }
        public string ReceiptGroup { get; set; }
        public string StorerKey { get; set; }
        public Nullable<System.DateTime> ReceiptDate { get; set; }
        public string POKey { get; set; }
        public string CarrierKey { get; set; }
        public string CarrierName { get; set; }
        public string CarrierAddress1 { get; set; }
        public string CarrierAddress2 { get; set; }
        public string CarrierCity { get; set; }
        public string CarrierState { get; set; }
        public string CarrierZip { get; set; }
        public string CarrierReference { get; set; }
        public string WarehouseReference { get; set; }
        public string OriginCountry { get; set; }
        public string DestinationCountry { get; set; }
        public string VehicleNumber { get; set; }
        public Nullable<System.DateTime> VehicleDate { get; set; }
        public string PlaceOfLoading { get; set; }
        public string PlaceOfDischarge { get; set; }
        public string PlaceofDelivery { get; set; }
        public string IncoTerms { get; set; }
        public string TermsNote { get; set; }
        public string ContainerKey { get; set; }
        public string Signatory { get; set; }
        public string PlaceofIssue { get; set; }
        public Nullable<int> OpenQty { get; set; }
        public string forte_flag { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public System.DateTime EffectiveDate { get; set; }
        public string CONTAINERTYPE { get; set; }
        public Nullable<int> CONTAINERQTY { get; set; }
        public Nullable<int> BILLEDCONTAINERQTY { get; set; }
        public string TRANSPORTATIONMODE { get; set; }
        public string EXTERNALRECEIPTKEY2 { get; set; }
        public string SUSR1 { get; set; }
        public string SUSR2 { get; set; }
        public string SUSR3 { get; set; }
        public string SUSR4 { get; set; }
        public string SUSR5 { get; set; }
        public string TYPE { get; set; }
        public string RMA { get; set; }
        public System.DateTime EXPECTEDRECEIPTDATE { get; set; }
        public string TrailerNumber { get; set; }
        public string TrailerOwner { get; set; }
        public string DriverName { get; set; }
        public Nullable<System.DateTime> ArrivalDateTime { get; set; }
        public System.DateTime AddDate { get; set; }
        public string AddWho { get; set; }
        public System.DateTime EditDate { get; set; }
        public string EditWho { get; set; }
        public string CUSTINVNUM { get; set; }
        public Nullable<decimal> INVVALUE { get; set; }
        public string INVCURRENCY { get; set; }
        public Nullable<System.DateTime> POInvDate { get; set; }
        public string DELIVERYTERMS { get; set; }
        public string DELIVERYTERMSCITY { get; set; }
        public string DELIVERYTERMSCNTRY { get; set; }
        public string ConsigneeKey { get; set; }
        public string Billtokey { get; set; }
        public string PTWYPRINTSTATUS { get; set; }
        public string CUSTSTATUS { get; set; }
        public Nullable<decimal> FREIGHTCOSTS { get; set; }
        public string FREIGHTCOSTSCURRENCY { get; set; }
        public Nullable<decimal> INSCOSTS { get; set; }
        public string INSCOSTSCURRENCY { get; set; }
        public Nullable<decimal> OTHERCOSTS { get; set; }
        public string OTHERCOSTSCURRENCY { get; set; }
        public string COUNTRYOFDISPATCH { get; set; }
        public string CONTAINERTRANSPORT { get; set; }
        public string DEPARTUREAIRPORT { get; set; }
        public string ARRIVALAIRPORT { get; set; }
        public string ERRORSTATUS { get; set; }
        public string ERRORMESSAGE { get; set; }
        public string custdocnumber { get; set; }
        public string custdoctype { get; set; }
        public string XIMPORTER { get; set; }
        public string XCUSTSTAT { get; set; }
        public string RHTYPE { get; set; }
        public Nullable<decimal> IBIP { get; set; }
        public Nullable<decimal> IBCS { get; set; }
        public string VENDORNAME { get; set; }
        public string FROMWHSE { get; set; }
        public Nullable<decimal> LINECOUNT { get; set; }
        public Nullable<decimal> NR_CONTAINERS2 { get; set; }
        public string CONTAINERTYPE2 { get; set; }
        public string TrafficCop { get; set; }
        public string ArchiveCop { get; set; }
        public string Reason { get; set; }
        public string Remarks { get; set; }
        public string ASNStatus { get; set; }
        public string ContainerNumber { get; set; }
        public Nullable<System.DateTime> ContainerArrivalDate { get; set; }
        public Nullable<System.DateTime> UnstuffedDate { get; set; }
        public Nullable<System.DateTime> TruckOutDate { get; set; }
        public Nullable<int> NumberOfCartons { get; set; }
        public string ContainerRemarks { get; set; }
        public string ExportStatus { get; set; }
        public string Remark1 { get; set; }
        public string Remark2 { get; set; }
        public string Remark3 { get; set; }
        public string Remark4 { get; set; }
        public string Remark5 { get; set; }
        public string Remark6 { get; set; }
        public string HoldFlag { get; set; }
        public string FinaliseRcvFlag { get; set; }
        public string ReceiptType { get; set; }
        public string Reference1 { get; set; }
        public string Reference2 { get; set; }
        public Nullable<int> TotalCases { get; set; }
        public string MEAFlag { get; set; }
        public Nullable<int> TotalGrossWeight { get; set; }
        public string PackType { get; set; }
        public Nullable<int> LadingQty { get; set; }
        public string TransportMode { get; set; }
        public string Trackno_AW { get; set; }
        public string Trackno_MB { get; set; }
        public string Trackno_OC { get; set; }
        public string Trackno_SN { get; set; }
        public string AirwaybillNo { get; set; }
        public string Billoflading { get; set; }
        public string OceanContainerNo { get; set; }
        public string SealNo { get; set; }
        public string Shipto { get; set; }
        public string ShipName { get; set; }
        public string ShipContact { get; set; }
        public string ShipAdd1 { get; set; }
        public string ShipAdd2 { get; set; }
        public string ShipCity { get; set; }
        public string ShipState { get; set; }
        public string ShipZip { get; set; }
        public string ShipCountry { get; set; }
        public string ShipMarkCode { get; set; }
        public string ShipRemark { get; set; }
        public string CarrierCode { get; set; }
        public Nullable<System.DateTime> ShipDate { get; set; }
        public string C00101UOM { get; set; }
        public Nullable<System.DateTime> Date1 { get; set; }
        public Nullable<System.DateTime> Date2 { get; set; }
        public string ShipmentID { get; set; }
        public string SCAC { get; set; }
        public string Mode { get; set; }
        public string Reference3 { get; set; }
        public string Reference4 { get; set; }
        public string Reference5 { get; set; }
        public string ShipFromCountry { get; set; }
        public string MasterAWBBOL { get; set; }
        public string HouseAWB { get; set; }
        public string ShippingVendor { get; set; }
        public string Notes1 { get; set; }
        public string Notes2 { get; set; }
        public Nullable<decimal> Weight { get; set; }
        public Nullable<decimal> Volume { get; set; }
        public string PacklistID { get; set; }
        public Nullable<System.DateTime> TruckDeparture { get; set; }
        public string StorerName { get; set; }
        public string BillToKeyName { get; set; }
        public string ConsigneeKeyName { get; set; }
        public IEnumerable<ReceiptDetail> ReceiptDetail { get; set; }
    }
}