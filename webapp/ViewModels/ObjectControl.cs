using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WMSPortal.ViewModels
{
    public class ObjectControl
    {
        public string Caption { get; set; }
        public string ControlType { get; set; }
        public string Name { get; set; }
        public List<SelectListItem> Options { get; set; }
        public ObjectControl()
        {
            Options = new List<SelectListItem>();
        }

    }
}