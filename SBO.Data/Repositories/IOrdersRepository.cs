using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMSPortal.Core.Model;

namespace WMSPortal.Data.Repositories
{
    public interface IOrdersRepository 
    {
        IEnumerable<OrderDetail> PickingDashboard();

        IEnumerable<OrderDetail> GetPivotOrders(string startDate,string stopDate);
        IEnumerable<OrderDetail> GetOutboundShipment(string column, string value1, string value2,string sectionView,string userId);
        IEnumerable<OrderDetail> GetOutboundShipmentToday(string userId);
        IEnumerable<PickDetail> GetPickDetail(string orderKey,string orderLineNumber);
        IEnumerable<PickDetail> GetOrdersTransection(string orderKey);
        OrdersDashboard GetOrdersDashboard();
        OrdersDashboard GetOrdersPerfomance();
        Orders GetOrders(string orderKey);
        void SaveShipmentOrder(Orders order);
        void DeleteShipmentOrder(string orderKey);
        void PostOrder(string orderKey);

    }
}
