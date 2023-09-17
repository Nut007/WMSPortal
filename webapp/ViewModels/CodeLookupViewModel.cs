using MicroOrm.Pocos.SqlGenerator.Attributes;
using WMSPortal.Core.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace WMSPortal.ViewModels
{
    public class CodeLookupViewModel
    {
        [DisplayName("Lookup Id")]
        public string Code { get; set; }
        [DisplayName("Name")]
        [Required]
        public string Description { get; set; }
        [DisplayName("Origin Port")]
        public string Short { get; set; }
        [DisplayName("Flight")]
        public string Long { get; set; }
        [DisplayName("ETD")]
        public string Notes { get; set; }
        public string LISTNAME { get; set; }
        public DateTime? AddDate { get; set; }
        public string AddWho { get; set; }
        public DateTime? EditDate { get; set; }
        public string EditWho { get; set; }
        public bool _IsNew { get; set; }
        public bool IsSelected { get; set; }
        public IEnumerable<Codelkup> CodeLookups { get; set; }
    }
}