using MicroOrm.Pocos.SqlGenerator.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSPortal.Core.Model
{
    public class ViewInformation
    {
        [NonStored]
        public bool IsNew { get; set; }
        [NonStored]
        public bool HasError { get; set; }
        [NonStored]
        public string ExceptionErrorMessage { get; set; }
        [NonStored]
        public string ValidationErrorMessage;
        [NonStored]
        public Int32 CurrentPageNumber { get; set; }
        [NonStored]
        public int PageSize { get; set; }
        [NonStored]
        public int TotalPages { get; set; }
        [NonStored]
        public int TotalRows { get; set; }
        [NonStored]
        public string SortExpression { get; set; }
        [NonStored]
        public string SortDirection { get; set; }
        [NonStored]
        public int RowIndex { get; set; }
        [NonStored]
        public string ViewState { get; set; }
        [NonStored]
        public string PageID { get; set; }

    }
}
