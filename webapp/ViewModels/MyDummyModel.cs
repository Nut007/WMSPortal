using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WMSPortal.ViewModels
{
    public class MyDummyModel
    {
        [UIHint("SignaturePad")]
        public byte[] MySignature { get; set; }
    }
}