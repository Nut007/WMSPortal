using MicroOrm.Pocos.SqlGenerator.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WMSPortal.Core.Model
{
    public partial class User
    {
        [KeyProperty(Identity = true)]
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordConfirmed { get; set; }
        public bool DisallowChangePassword { get; set; }
        public bool DisableAccount { get; set; }
        public bool IsImmediately { get; set; }
        public DateTime? DisableAfterDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public bool _IsNew { get; set; }
        public IEnumerable<UserRole> UserRoles { get; set; }
        public IEnumerable<Role> Roles { get; set; }
        public IEnumerable<ApplicationRole> ApplicationRoles { get; set; }
        public Role CurrentRoleConnection { get; set; }
        public int RoleId { get; set; }
    }
}