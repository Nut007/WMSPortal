using MicroOrm.Pocos.SqlGenerator.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSPortal.Core.Model
{
    [StoredAs("TEMP_ID")]
    public class TEMP_ID : ICloneable
    {
        public string ID { get; set; }
        public string PCNO { get; set; }
        public string SKU { get; set; }
        public string TYPE { get; set; }
        public string ID_OPTION { get; set; }
        public string COLOR { get; set; }
        public string PACKTYPE { get; set; }
        public string PACKUNIT { get; set; }
        public string VANPLANT { get; set; }
        public string VANSIZE { get; set; }
        public string SITE { get; set; }
        public string MASTER_ID { get; set; }
        public string LOT { get; set; }
        public string ISSUED_DATE { get; set; }
        public DateTime? EXPECTED_DATE { get; set; }
        public string ORIGIN_COUNTRY { get; set; }
        public string ORIGIN_PORT { get; set; }
        public string DESTINATION_COUNTRY { get; set; }
        public string DESTINATION_PORT { get; set; }
        public string ORIGIN_POST_CODE { get; set; }
        public string DESTINATION_POST_CODE { get; set; }
        public string DESPATCH_NO { get; set; }
        public string RECEPTACLE_NO { get; set; }
        public string SERVICE_TYPE { get; set; }
        public decimal GROSS_WEIGHT { get; set; }
        public string SHIPMENT_STATUS { get; set; }
        public string MAWB { get; set; }
        public string CN38 { get; set; }
        public DateTime? ADD_DATE { get; set; }
        public DateTime? EDIT_DATE { get; set; }
        public decimal PEICES { get; set; }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
