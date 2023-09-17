using WMSPortal.Core.Model;
using DapperExtensions.Mapper;
public class RoleMapper : ClassMapper<Role>
{
    public RoleMapper()
    {
        Map(f => f._IsNew).Ignore();
        Map(f => f.ApplicationRoles).Ignore();
        AutoMap();
    }
}