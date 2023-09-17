using WMSPortal.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSPortal.Data.Repositories
{
    public interface IApplicationRoleRepository
    {
        IEnumerable<ApplicationRole> GetApplicationRoles(int roleId);
        bool CreateOrUpdateApplicationRoles(IEnumerable<ApplicationRole> applicationRoles);
    }
}
