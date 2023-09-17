using WMSPortal.Core.Model;
using DapperExtensions.Mapper;
public class CodelkupMapper : ClassMapper<Codelkup>
{
    public CodelkupMapper()
    {
        Map(f => f.Code).Key(KeyType.Assigned);
        Map(f => f.LISTNAME).Key(KeyType.Assigned);
        Map(f => f.Timestamp).Ignore();
        AutoMap();
    }
}