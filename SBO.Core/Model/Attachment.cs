﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WMSPortal.Core
{
    public class Attachment
    {
        public int ID { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public byte[] FileContent { get; set; }
    }
}