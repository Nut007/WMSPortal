using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSPortal.Core.Model
{
    public class ApplicationRole
    {
        public int RoleId { get; set; }
        [DisplayName("โมดูล")]
        public string ApplicationName { get; set; }
        public int ApplicationId { get; set; }
        [DisplayName("อนุญาติ?")]
        public bool IsAllowAccess { get; set; }
        [DisplayName("อ่าน")]
        public bool IsRead { get; set; }
        [DisplayName("อ่าน/เขียน")]
        public bool IsReadWrite { get; set; }
        [DisplayName("อนุมัติรายการ")]
        public bool IsAllowApprove { get; set; }
        public bool IsShowApproval { get; set; }
        public bool _IsNew { get; set; }
    }
}
