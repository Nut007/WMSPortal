using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSPortal.Core.Model
{
    public class DeclarationType {
        public const string Inbound = "INBDECT";
        public const string Outbound = "OUBDECT";
        public const string DecType = "DECTYPE";
        public const string MAWB = "MAWB";   
    }
    public enum Gender
    {
        [Description("ไม่ระบุ")]
        None = 0,

        [Description("ชาย")]
        Male = 1,

        [Description("หญิง")]
        Female = 2

    }
    public enum CustomerLevel
    {
        [Description("ทั่วไป")]
        Normal = 1,

        [Description("พิเศษ")]
        VIP = 2,

    }
    public enum LookupType
    {
        [Description("ประเทศ")]
        Country = 1,
        [Description("ธนาคาร")]
        Bank = 2,
        [Description("เว็บ")]
        Owner = 3,
        [Description("ความสำคัญ")]
        Priority =4 ,
        [Description("DropLoc")]
        DropLoc = 5, 
        [Description("Master Airway Bill")]
        MAWB = 6,
        IMPCHKT=7,
        IMPCBKK=8
    }
    public enum TransectionType
    {
        [Description("ประเทศ")]
        All = 1,
        [Description("ฝาก")]
        Deposit = 2,
        [Description("ถอน")]
        Withdraw = 3,
    }
    public enum DocumentType
    { 
        Order,
        PickDetailKey,
        PickHeaderKey,
        CartonID,
        Loading
    }
    public enum SectionView
    {
        Header = 1,
        Detail = 2
    }
    public enum EventLog
    {
        Login = 1,
        Logout = 2,
        Access = 3,
        Execute = 4
    }
    public enum AdditionalQuery
    {
        RPT_ADHOC
    }
}
