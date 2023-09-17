using MicroOrm.Pocos.SqlGenerator.Attributes;
using WMSPortal.Core.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace WMSPortal.ViewModels
{
    public class UserListViewModel
    {
        [HiddenInput(DisplayValue = false)]
        [KeyProperty(Identity = true)]
        [DisplayName("Code")]
        public int UserID { get; set; }
        [DisplayName("User Name")]
        [Required]
        public string UserName { get; set; }
        [DisplayName("Email")]
        public string Email { get; set; }
        [DisplayName("Password")]
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        [DisplayName("Confirm Password")]
        public string PasswordConfirmed { get; set; }
        [DisplayName("Disable Change Password")]
        public bool DisallowChangePassword { get; set; }
        [DisplayName("Account is Disable")]
        public bool DisableAccount { get; set; }
        [DisplayName("Disable Account")]
        public bool IsImmediately { get; set; }
        [DisplayName("Disable account if date exceeds")]
        public DateTime? DisableAfterDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public bool _IsNew { get; set; }
        public IEnumerable<UserRole> UserRoles { get; set; }
    }
}