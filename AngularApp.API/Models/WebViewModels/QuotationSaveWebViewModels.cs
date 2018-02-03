using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AngularApp.API.Models.WebViewModels
{
    public class QuotationSaveWebViewModels
    {
        public CarHirePurchaseWebQuotationResultViewModel QuotationCalculation { get; set; }
        public CarHirePurchaseWebViewModel QuotationDetails { get; set; }
        public string ParentId { get; set; }
        public string QuoteId { get; set; }
    }
}