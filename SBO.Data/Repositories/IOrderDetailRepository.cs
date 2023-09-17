using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMSPortal.Core.Model;

namespace WMSPortal.Data.Repositories
{
    public interface IOrderDetailRepository
    {
        IEnumerable<OrderDetail> GetOrderDetail(string orderKey);
        bool InsertOrderItem(OrderDetail item);
        void UpdateOrderItem(OrderDetail item);
        void DeleteOrderItem(OrderDetail item);
        string GetNextOrderLineNumber(string orderKey);
        IEnumerable<OrderDetail> GetOrderRemaining(string orderKey,string orderLineNumber);
        void AllocateOrderItems(string orderKey, string orderLineNumber);
        void BatchAllocateItems(string orderKey, IEnumerable<LotxLocxId> orderItems,string userId);
     
    }
}
