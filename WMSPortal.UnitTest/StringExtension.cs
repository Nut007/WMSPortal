using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSPortal.UnitTest
{
    public class StringExtension
    {
        public static string NonBlankValueOf(object strTestString)
        {
            if (strTestString == null)
                return string.Empty;
            else
                return Convert.ToString(strTestString).Trim();
          
        }
    }
}
