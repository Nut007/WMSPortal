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
    public class RoleListViewModel
    {
        [HiddenInput(DisplayValue = false)]
        [KeyProperty(Identity = true)]
        [DisplayName("Code")]
        public int Id { get; set; }
        [DisplayName("Name")]
        public string Name { get; set; }
        public bool _IsNew { get; set; }
        [DisplayName("Server Name")]
        public string ServerName { get; set; }
        [DisplayName("Database Name")]
        public string DatabaseName { get; set; }
        [DisplayName("User Name")]
        public string UserName { get; set; }
        [DisplayName("Password")]
        public string Password { get; set; }
        public IEnumerable<ApplicationRole> ApplicationRoles { get; set; }
      
    }
}