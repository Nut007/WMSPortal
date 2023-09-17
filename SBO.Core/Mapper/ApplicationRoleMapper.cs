using WMSPortal.Core.Model;
using DapperExtensions.Mapper;

namespace WMSPortal.Core.Mapper
{
    public class ApplicationRoleMapper : ClassMapper<ApplicationRole>
    {
        public ApplicationRoleMapper()
        {
            Map(f => f.RoleId).Key(KeyType.Assigned);
            Map(f => f.ApplicationId).Key(KeyType.Assigned);
            Map(f => f.ApplicationName).Ignore();
            Map(f => f.IsShowApproval).Ignore();
            Map(f => f._IsNew).Ignore();
            AutoMap();
        }
    }
}
