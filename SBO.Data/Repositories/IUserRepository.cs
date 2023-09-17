using WMSPortal.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSPortal.Data.Repositories
{
    public interface IUserRepository
    {
        IEnumerable<User> GetUsers(string column, string value);
        User GetUser(int userID);
        IEnumerable<User> GetUserList();
        int CreateUser(User user);
        bool UpdateUser(User user);
        bool DeleteUsers(List<int> userIDs);
        bool CreateOrUpdateUser(User user);
        User FindByUserAndPassword(string userName, string password);
        IEnumerable<Role> GetRolesByUser(string userName);
        IEnumerable<Role> GetRolesByUserId(int userId);
    }
}
