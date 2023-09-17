using MicroOrm.Pocos.SqlGenerator.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSPortal.Core.Model
{
    [StoredAs("LOADING_DETAIL")]
    public class LoadingDetail
    {
        [KeyProperty(Identity = false)]
        public string LOADINGNO { get; set; }
        public string PACKINGNO { get; set; }
        [KeyProperty(Identity = false)]
        public string ITEMNO { get; set; }
        public DateTime? ADD_DATE { get; set; }
    }
}
