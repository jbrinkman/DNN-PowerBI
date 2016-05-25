using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dnn.Modules.PowerBI.Services.ViewModels
{
    public class PBIReports
    {
        public PBIReport[] value { get; set; }
    }
    public class PBIReport
    {
        public string id { get; set; }
        public string name { get; set; }
        public string webUrl { get; set; }
        public string embedUrl { get; set; }
        public string token { get; set; }
    }
}