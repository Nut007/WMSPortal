using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WMSPortal.ViewModels
{
    public class GeneralQueryReportViewModel
    {
        public GeneralQueryReportViewModel()
        {
            FilterCollection = new List<ObjectControl>();
        }
        public List<ObjectControl> FilterCollection { get; set; }
    }
}