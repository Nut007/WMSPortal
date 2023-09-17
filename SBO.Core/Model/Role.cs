using MicroOrm.Pocos.SqlGenerator.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace WMSPortal.Core.Model
{
    public class Role
    {
        [KeyProperty(Identity = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        public bool _IsNew { get; set; }
        public string ServerName { get; set; }
        public string DatabaseName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ServerType { get; set; }
        public IEnumerable<ApplicationRole> ApplicationRoles { get; set; }
    }
}
