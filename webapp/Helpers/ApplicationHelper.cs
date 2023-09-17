using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.EnterpriseServices;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.WebPages.Html;

namespace WMSPortal.Helpers
{
    public class ApplicationHelper
    {
        public static List<SelectListItem> EnumToSelectList(Type enumType)
        {
            FieldInfo fi = enumType.GetField(enumType.ToString());
            return Enum
              .GetValues(enumType)
              .Cast<int>()
              .Select(i => new SelectListItem
                {
                    Value = i.ToString(),
                    Text = Enum.GetName(enumType, i)
                }
              )
              .ToList();
        }
        
    }
}