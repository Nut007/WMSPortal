using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMSPortal.Core.Model;

namespace WMSPortal.Data.Repositories
{
    public interface IStockBalanceRepository
    {
        IEnumerable<LotxLocxId> GetInventory(List<string> columnvalues, string viewBy, int pageSize, int page,string userId);
        IEnumerable<LotxLocxId> GetAllInventory();
        IEnumerable<LotxLocxId> GetCommodityInfo(string pcNo);
        IEnumerable<LotxLocxId> GetInventoryBySku(string storerKey, string viewBy);
        IEnumerable<LotxLocxId> GetInventoryGroupBySku(string column, string value, string userId);
    }
}
