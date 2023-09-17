using WMSPortal.Core.Model;
using DapperExtensions.Mapper;
public class TEMP_IDMapper : ClassMapper<TEMP_ID>
{
    public TEMP_IDMapper()
    {
        Map(f => f.ADD_DATE).Ignore();
        Map(f => f.CN38).Ignore();
        Map(f => f.PEICES).Ignore();
        //Map(f => f.EDIT_DATE).Ignore();
        AutoMap();
    }
}