using MicroOrm.Pocos.SqlGenerator.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSPortal.Core.Model
{
    [StoredAs("LOADING_HEADER")]
    public class Loading
    {
        [KeyProperty(Identity = false)]
        public string LOADINGNO { get; set; }
        public DateTime LOADING_DATE { get; set; }
        public string PLATE_NO { get; set; }
        public string CONTAINER_NO { get; set; }
        public string STATUS { get; set; }
        public DateTime? ADDDATE { get; set; }
        public string ADDWHO { get; set; }
        public DateTime? EDITDATE { get; set; }
        public string EDITWHO { get; set; }
        [NonStored]
        public string PACKINGNO { get; set; }
        [NonStored]
        public string ITEMNO { get; set; }
        [NonStored]
        public IEnumerable<LoadingDetail> LoadingDetail { get; set; }
    }

    public class TestLoading
    {
        public string name { get; set; }
        public string est { get; set; }
        public string contacts { get; set; }
        public string status { get; set; }
        public string targetactual { get; set; }
        public string starts { get; set; }
        public string ends { get; set; }
        public string comments { get; set; }
        public string action { get; set; }
    }
}
