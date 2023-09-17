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
    
    public partial class PickDetail
    {
        public Nullable<int> Gennumber { get; set; }
        public string WarehouseKey { get; set; }
        public string PickDetailKey { get; set; }
        public string CaseID { get; set; }
        public string PickHeaderKey { get; set; }
        public string OrderKey { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string CustomerName { get; set; }
        public string ExternOrderKey { get; set; }
        public string OrderLineNumber { get; set; }
        public string Lot { get; set; }
        public string Storerkey { get; set; }
        public string Sku { get; set; }
        public string AltSku { get; set; }
        public string UOM { get; set; }
        public int UOMQty { get; set; }
        public int Qty { get; set; }
        public int QtyMoved { get; set; }
        public string Status { get; set; }
        public string DropID { get; set; }
        public string Loc { get; set; }
        public string ID { get; set; }
        public string PackKey { get; set; }
        public string UpdateSource { get; set; }
        public string CartonGroup { get; set; }
        public string CartonType { get; set; }
        public string ToLoc { get; set; }
        public string DoReplenish { get; set; }
        public string ReplenishZone { get; set; }
        public string DoCartonize { get; set; }
        public string PickMethod { get; set; }
        public string WaveKey { get; set; }
        public System.DateTime EffectiveDate { get; set; }
        public string forte_flag { get; set; }
        public string FromLoc { get; set; }
        public string TRACKINGID { get; set; }
        public Nullable<double> FREIGHTCHARGES { get; set; }
        public string INTERMODALVEHICLE { get; set; }
        public Nullable<int> LoadID { get; set; }
        public Nullable<int> Stop { get; set; }
        public string Door { get; set; }
        public string Route { get; set; }
        public string SortationLocation { get; set; }
        public string SortationStation { get; set; }
        public string BatchCartonID { get; set; }
        public string IsClosed { get; set; }
        public string QCStatus { get; set; }
        public string PDUDF1 { get; set; }
        public string PDUDF2 { get; set; }
        public string PDUDF3 { get; set; }
        public string PickNotes { get; set; }
        public System.DateTime AddDate { get; set; }
        public string AddWho { get; set; }
        public System.DateTime EditDate { get; set; }
        public string EditWho { get; set; }
        public string TrafficCop { get; set; }
        public string ArchiveCop { get; set; }
        public string OptimizeCop { get; set; }
        public Nullable<int> NumberOfCartonsPicked { get; set; }
        public Nullable<double> Length { get; set; }
        public Nullable<double> Width { get; set; }
        public Nullable<double> Height { get; set; }
        public Nullable<double> Weight { get; set; }
        public Nullable<double> GrossWgt { get; set; }
        public Nullable<double> Cube { get; set; }
        public Nullable<int> CartonNoFrom { get; set; }
        public Nullable<int> CartonNoTo { get; set; }
        public string SerialNo { get; set; }
        public Nullable<System.DateTime> SerialFeedDate { get; set; }
        public string PickWho { get; set; }
        public Nullable<System.DateTime> PickDate { get; set; }
        public string NewSerialNo { get; set; }
        public string DNnumber { get; set; }
        public string Palletid { get; set; }
        public string notes { get; set; }
        public string BoxNumber { get; set; }
        public string TotalBoxCount { get; set; }
    }
}
