using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AngularApp.API.Models.WebViewModels
{
    public abstract class ConfigurationWebViewModels
    {
    }

    public class QuoteTypesViewModel : ConfigurationWebViewModels
    {
        public int TypeID { get; set; }
        public string QuoteType { get; set; }
        public int ProductParentID { get; set; }
        public bool Enabled { get; set; }
    }

    public class QuoteStatusesViewModel : ConfigurationWebViewModels
    {
        public int StatusID { get; set; }
        public string State { get; set; }
        public bool Enabled { get; set; }
    }

    public class QuoteDefaultsViewModel : ConfigurationWebViewModels
    {
        public int TypeID { get; set; }
        public string ElementDescription { get; set; }
        public string XMLTemplate { get; set; }
        public string TotalRepayableTemplate { get; set; }
        public string MonthlyRepayableTemplate { get; set; }
        public bool Enabled { get; set; }
    }

    public class ProductTypesViewModel : ConfigurationWebViewModels
    {
        public int ProductTypeID { get; set; }
        public string ProductType { get; set; }
        public bool Enabled { get; set; }
    }

    public class PaginatedConfigResult
    {
        public IEnumerable<JObject> ConfigResult;
        public int TotalPages;
        public int TotalItems { get; set; }
    }

    public class SaveConfigurationViewModel
    {
        public string ConfigType { get; set; }
        public List<JObject> ConfigsToBeSaved { get; set; }
    }

    public class DefaultConfigurationSearchWebViewModel
    {
        public string ConfigType { get; set; }
        public string FilterText { get; set; }
    }
}