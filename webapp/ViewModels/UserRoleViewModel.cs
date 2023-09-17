using WMSPortal.Core.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace WMSPortal.ViewModels
{
    public class UserRoleViewModel
    {
        public int UserId { get; set; }
        public string RoleId { get; set; }
        [DisplayName("User Group")]
        public string Name { get; set; }
        [DisplayName("Act")]
        public bool IsSelected { get; set; }
        public bool _IsNew { get; set; }

        //public IEnumerable<UserRole> UserRoles { get; set; }
    }
}