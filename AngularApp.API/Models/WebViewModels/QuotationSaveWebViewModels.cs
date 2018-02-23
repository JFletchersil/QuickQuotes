using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;

namespace AngularApp.API.Models.WebViewModels
{
    public class QuotationSaveWebViewModels
    {
        public JObject QuotationCalculation { get; set; }
        public JObject QuotationDetails { get; set; }
        public string ParentId { get; set; }
        public string QuoteId { get; set; }
    }
}