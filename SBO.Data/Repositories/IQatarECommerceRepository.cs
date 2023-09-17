using WMSPortal.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSPortal.Data.Repositories
{
    public interface IQatarECommerceRepository
    {
        List<QatarECommerce> GetConsignmentInfo(string issueStartDate, string issueStopDate,string column, string value, int connectionId, string userId);
        
    }
}
