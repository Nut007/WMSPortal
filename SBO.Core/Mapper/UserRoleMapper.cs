using WMSPortal.Core.Model;
using DapperExtensions.Mapper;

namespace WMSPortal.Core.Mapper
{
    public class UserRoleMapper : ClassMapper<UserRole>
    {
        public UserRoleMapper()
        {
            Map(f => f.RoleId).Key(KeyType.Assigned); ;
            Map(f => f.UserId).Key(KeyType.Assigned);
            Map(f => f.Name).Ignore();
            Map(f => f.IsSelected).Ignore();
            Map(f => f._IsNew).Ignore();
           
            AutoMap();
        }
    }
}
