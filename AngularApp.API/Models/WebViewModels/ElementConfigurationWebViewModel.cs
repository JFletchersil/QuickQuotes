using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AngularApp.API.Models.WebViewModels
{
    //public class ElementConfigurationListWebViewModel
    //{
    //    public List<ElementConfigurationWebViewModel> ListView { get; set; }
    //}

    public class ElementConfigurationWebViewModel
    {
        public string ElementName { get; set; }
        public string WarningLabel { get; set; }
        public string LabelName { get; set; }
        public ElementDescription ElementDescription { get; set; }
    }

    public class ElementDescription
    {
        public string Type { get; set; }
        public bool IsCurrency { get; set; }
        public bool Required { get; set; }
        public decimal Max { get; set; }
        public decimal Min { get; set; }
    }
}