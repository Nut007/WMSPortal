using WMSPortal.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSPortal.Data.Repositories
{
    public interface ILoadingRepository
    {
        IEnumerable<Loading> GetLoadingList(string column, string value1, string value2, string sectionView, string userId);
        Loading GetLoading(string loadingNo);
        IEnumerable<Loading> GetLoadingAll();
        IEnumerable<TestLoading> GetTestLoadingAll();
        void SaveLoading(Loading loading);
        bool UpdateLoading(Loading loading);
        bool DeleteLoading(string loading);
        bool CreateOrUpdateLoading(Loading loading);

    }
}
