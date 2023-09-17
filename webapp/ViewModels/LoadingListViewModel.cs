using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace WMSPortal.ViewModels
{
    public class LoadingListViewModel
    {
        [DisplayName("Loading No#")]
        public string LOADINGNO { get; set; }
        [DisplayName("Line#")]
        public string ITEMNO { get; set; }
        [DisplayName("Loading Date")]
        public DateTime LOADING_DATE { get; set; }
        [DisplayName("Packing No")]
        public string PACKINGNO { get; set; }
        [DisplayName("Plate No")]
        public string PLATE_NO { get; set; }
        public string Status { get; set; }
    }
}