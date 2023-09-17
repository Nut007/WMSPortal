using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMSPortal.Core.Model;

namespace WMSPortal.Data.Repositories
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetSkuByCode(string sku,string storerKey);
    }
}
