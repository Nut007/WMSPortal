using WMSPortal.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSPortal.Data.Repositories
{
    public interface IUserRoleRepository
    {
        IEnumerable<UserRole> GetUserRoles(int userId);
        bool CreateOrUpdateUserRoles(IEnumerable<UserRole> userRoles);
    }
}
