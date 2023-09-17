using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMSPortal.Core.Model;


namespace WMSPortal.Data.Repositories
{
    public interface ICodelkupRepository : IDataRepository<Codelkup>
    {
        IEnumerable<Codelkup> GetLookupListByType(LookupType lookupType);
        IEnumerable<Codelkup> GetLookupListByGroupType(LookupType lookupType, int? lookupGroupId);
        IEnumerable<Codelkup> GetLookupListByShort(string shortDescr);
        IEnumerable<Codelkup> GetLookupListByIMPC();
        string GetLookupDescripion(LookupType lookupType, int lookupId);
        string GetShortDescripion(LookupType lookupType, string lookupId);
        bool DeleteLookupItems(List<string> codeLookupId, string codeLookupGroup);
        void InsertLookup(Codelkup lookup);
        void UpdateLookup(Codelkup lookup);
    }
}
