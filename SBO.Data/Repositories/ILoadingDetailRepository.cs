using WMSPortal.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSPortal.Data.Repositories
{
    public interface ILoadingDetailRepository
    {
        IEnumerable<LoadingDetail> GetLoadingDetailList(string column, string value);
        IEnumerable<LoadingDetail> GetLoadingDetail(string loadingNo);
        IEnumerable<LoadingDetail> GetLoadingAll();
        bool CreateLoadingDetail(LoadingDetail item);
        bool UpdateLoadingDetail(LoadingDetail item);
        bool DeleteLoadingDetail(LoadingDetail item);
     

    }
}
