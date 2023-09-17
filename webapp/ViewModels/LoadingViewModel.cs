using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WMSPortal.Core.Model;

namespace WMSPortal.ViewModels
{
    public class LoadingViewModel : ViewInformation
    {
        [DisplayName("Loading No")]
        public string LOADINGNO { get; set; }
        [DisplayName("Loading Date")]
        public DateTime? LOADING_DATE { get; set; }
        [DisplayName("Plate No")]
        public string PLATE_NO { get; set; }
        [DisplayName("Container No")]
        public string CONTAINER_NO { get; set; }
        [DisplayName("Status")]
        public string STATUS { get; set; }
        [DisplayName("Add Date")]
        public DateTime? ADDDATE { get; set; }
        [DisplayName("Add Who")]
        public string ADDWHO { get; set; }
        [DisplayName("Edit Date")]
        public DateTime? EDITDATE { get; set; }
        [DisplayName("Edit Who")]
        public string EDITWHO { get; set; }
        public IEnumerable<LoadingDetail> LoadingDetail { get; set; }
      
    }
}