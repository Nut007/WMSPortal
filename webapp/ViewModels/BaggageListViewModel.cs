using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WMSPortal.ViewModels
{
    public class BaggageListViewModel
    {
        [DisplayName("CN35 No")]
        [Required(ErrorMessage = "CN35 Required")]
        [StringLength(29, ErrorMessage = "CN35 have to be {1} characters.", MinimumLength = 29)]
        public string ID { get; set; }
        [DisplayName("ULD No")]
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
        [DisplayName("Mawb")]
        public string MASTER_ID { get; set; }
        [DisplayName("CN38 No")]
        public string LOT { get; set; }
        public DateTime? EXPECTED_DATE { get; set; }
        public string ORIGIN_COUNTRY { get; set; }
        public string ORIGIN_PORT { get; set; }
        [DisplayName("Destination Country")]
        public string DESTINATION_COUNTRY { get; set; }
        [DisplayName("Destination Port")]
        [Required(ErrorMessage = "Destination Port Required")]
        public string DESTINATION_PORT { get; set; }
        public string ORIGIN_POST_CODE { get; set; }
        public string DESTINATION_POST_CODE { get; set; }
        [DisplayName("Despatch No")]
        [Required(ErrorMessage = "Despatch No Required")]
        public string DESPATCH_NO { get; set; }
        [DisplayName("Receptacle No")]
        [Required(ErrorMessage = "Receptacle No Required")]
        public string RECEPTACLE_NO { get; set; }
        public string SERVICE_TYPE { get; set; }
        [DisplayName("Gross Weight")]
        [RegularExpression(@"\d+(\.\d{1,1})?", ErrorMessage = "Gross weight must be a natural number or decimal must be one digit")]
        [Required(ErrorMessage = "Gross weight Required")]
        public decimal GROSS_WEIGHT { get; set; }
        [DisplayName("Status")]
        public string SHIPMENT_STATUS { get; set; }
        [DisplayName("Added Date")]
        public DateTime? ADD_DATE { get; set; }
        [DisplayName("Scanned Date")]
        public DateTime? EDIT_DATE { get; set; }
        public string MAWB { get; set; }
        [DisplayName("CN 38")]
        [Required(ErrorMessage = "CN38 Required")]
        public string CN38 { get; set; }
    }
}