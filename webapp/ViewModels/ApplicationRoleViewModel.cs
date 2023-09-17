using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace WMSPortal.ViewModels
{
    public class ApplicationRoleViewModel
    {
        public int RoleId { get; set; }
        [DisplayName("Module")]
        public string ApplicationName { get; set; }
        public int ApplicationId { get; set; }
        [DisplayName("Show")]
        public bool IsAllowAccess { get; set; }
        [DisplayName("Read")]
        public bool IsRead { get; set; }
        [DisplayName("Edit")]
        public bool IsReadWrite { get; set; }
        [DisplayName("Approval")]
        public bool IsAllowApprove { get; set; }
        public bool IsShowApproval { get; set; }
        public bool _IsNew { get; set; }
       
    }
}