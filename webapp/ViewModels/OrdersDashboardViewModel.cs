using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WMSPortal.Core.Model;

namespace WMSPortal.ViewModels
{
    public class OrdersDashboardViewModel : ViewInformation
    {
        public Int64 OpenQty { get; set; }
        public Int64 AllocatedQty { get; set; }
        public Int64 PickedQty { get; set; }
        public Int64 PackedQty { get; set; }
        public Int64 ShippedQty { get; set; }
        public Int64 BinUsage { get; set; }
        public Int64 TotalBin { get; set; }
        public Int64 AvaliableBin { get; set; }
        public decimal AvaliableBinPercent { get; set; }
        public Int64 TodayReceived { get; set; }
        public Int64 TodayExpected { get; set; }
        public string TodayReceivedPercent { get; set; }

        public List<OrdersWeekly> OrdersWeekly;
        public List<OrdersWeekly> WorkerPerfomanceWeekly;
        public List<OrdersWeekly> LocationUtilization;
        public List<OrdersWeekly> ReceiptPerfomance;
        public List<OrdersWeekly> OrdersTransaction;
    }
 
}