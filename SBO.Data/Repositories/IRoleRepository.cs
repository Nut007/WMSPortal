using WMSPortal.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSPortal.Data.Repositories
{
    public interface IRoleRepository
    {
        IEnumerable<Role> GetRoles(string column, string value);
        Role GetRole(int Id);
        IEnumerable<Role> GetRoleList();
        int CreateRole(Role role);
        bool UpdateRole(Role role);
        bool DeleteRoles(List<int> Ids);
        bool CreateOrUpdateRole(Role role);
      
    }
}
