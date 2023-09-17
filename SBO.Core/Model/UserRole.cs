using MicroOrm.Pocos.SqlGenerator.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace WMSPortal.Core.Model
{
    public class UserRole
    {
        public int UserId { get; set; }
        public string RoleId { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
        public bool _IsNew { get; set; }
    }
}
