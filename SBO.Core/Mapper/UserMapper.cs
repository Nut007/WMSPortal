using WMSPortal.Core.Model;
using DapperExtensions.Mapper;
public class UserMapper : ClassMapper<User>
{
    public UserMapper()
    {
        Map(f => f._IsNew).Ignore();
        Map(f => f.Roles).Ignore();
        Map(f => f.RoleId).Ignore();
        Map(f => f.UserRoles).Ignore();
        Map(f => f.CurrentRoleConnection).Ignore();
        Map(f => f.ApplicationRoles).Ignore();
        AutoMap();
    }
}