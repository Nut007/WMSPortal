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

    public partial class OrdersDashboard : ViewInformation
    {
        public Int64 OpenRecord { get; set; }
        public Int64 OpenQty { get; set; }
        public Int64 AllocatedQty { get; set; }
        public Int64 PickedQty { get; set; }
        public Int64 TotalExternOrderKey { get; set; }
        public Int64 OriginalQty { get; set; }
        public Int64 ShippedQty { get; set; }
        //public Int64 RowIndex { get; set; }
        public Int64 QtyBoh { get; set; }
        public Int64 BinUsage { get; set; }
        public Int64 TotalBin { get; set; }
        public Int64 UrgentOrder { get; set; }
        public Int64 CompleteUrgent { get; set; }
        public Int64 SameDayOrder { get; set; }
        public Int64 WaitingForShippedQty { get; set; }
        

        public List<OrdersWeekly> OrdersWeekly;

        public List<OrdersWeekly> WorkerPerfomanceWeekly;

        public List<OrdersWeekly> LocationUtilization;

        public List<OrdersWeekly> ReceiptPerfomance;

        public List<OrdersWeekly> OrdersTransaction;
    }
    public class OrdersWeekly
    {
        public DateTime OrderDate { get; set; }
        public Int64 TotalOrders { get; set; }
        public Int64 TotalQty { get; set; }
        public Int64 TotalExternOrderKey { get; set; }
        public string WorkerId { get; set; }
        public string Status { get; set; }
        public string FrtMode { get; set; }
        public string Agent { get; set; }
        public Int64 Open { get; set; }
        public string ShipmentId { get; set; }
        public Int64 RowIndex { get; set; }
        public string Loc { get; set; }
        public Int64 QtyOpen { get; set; }
        public Int64 QtyPicked { get; set; }
        public Int64 ShippedQty { get; set; }
        public Int64 QtyReceived { get; set; }
        public Int64 QtyExpected { get; set; }
        public Int64 QtyBoh { get; set; }
        public Int64 BinUsage { get; set; }
        public Int64 TotalBin { get; set; }
        public string BinUsagePercent { get; set; }
        public string OnDate { get; set; }
       
    }
}
