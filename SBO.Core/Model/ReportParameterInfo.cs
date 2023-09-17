using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSPortal.Core.Model
{
    public class InboundReportParameters
    {
        public string DateType { get; set; }
        public string StartDate { get; set; }
        public string StopDate { get; set; }
        public string YearSelected { get; set; }
        public string MonthSelected { get; set; }
        public string MonthNameSelected { get; set; }
        public string[] DeclarationType { get; set; }
        public string[] Importers { get; set; }
        public string Sku { get; set; }
        
    }

    public class OutboundReportParameters
    {
        public string DateType { get; set; }
        public string StartDate { get; set; }
        public string StopDate { get; set; }
        public string YearSelected { get; set; }
        public string MonthSelected { get; set; }
        public string MonthNameSelected { get; set; }
        public string[] DeclarationType { get; set; }
        public string[] Importers { get; set; }
        public string Sku { get; set; }
    }

    public class StockBalanceReportParameters
    {
        public string DateType { get; set; }
        public string InventoryDate { get; set; }
        public string YearSelected { get; set; }
        public string MonthSelected { get; set; }
        public string MonthNameSelected { get; set; }
        public string[] DeclarationType { get; set; }
        public string[] Importers { get; set; }
        public string Sku { get; set; }
        public bool isShowLocation { get; set; }
    }
    public class OrdersTransectionReportParameters
    {
        public string orderKey { get; set; }
    }
}
