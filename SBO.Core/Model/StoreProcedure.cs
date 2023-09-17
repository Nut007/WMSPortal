using MicroOrm.Pocos.SqlGenerator.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace WMSPortal.Core.Model
{
    public class StoreProcedure
    {
        public string PROCEDURE_QUALIFIER { get; set; }
        public string PROCEDURE_OWNER { get; set; }
        public string PROCEDURE_NAME { get; set; }
        public string COLUMN_NAME { get; set; }
        public string COLUMN_TYPE { get; set; }
        public string DATA_TYPE { get; set; }
        public string TYPE_NAME { get; set; }
        //public string COLUMN_VALUE { get; set; }
        private string _COLUMN_VALUE; 
        public string COLUMN_VALUE   
        {
            get {
                if ((_COLUMN_VALUE is null))
                    return string.Empty;
                else
                    return _COLUMN_VALUE;
            }   
            set { _COLUMN_VALUE = value; }  
        }

    }
}
