using WMSPortal.Core.Model;
using DapperExtensions.Mapper;
public class UserLogMapper : ClassMapper<UserLog>
{
    public UserLogMapper()
    {
        Map(f => f.LogId).Key(KeyType.Identity);
        AutoMap();
    }
}