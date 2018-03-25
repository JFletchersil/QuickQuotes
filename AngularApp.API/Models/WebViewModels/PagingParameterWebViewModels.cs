using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AngularApp.API.Models.WebViewModels
{
    public class PagingParameterWebViewModel
    {
        [Required]
        public int PageNumber { get; set; } = 1;
        [Required]
        public int PageSize { get; set; } = 10;
        public string OrderBy { get; set; }
    }

    public class AccountPagingParameterWebViewModel : PagingParameterWebViewModel
    {
        [Required]
        public bool ReturnAll { get; set; }
    }

    public class QueuePagingParameterWebViewModel : PagingParameterWebViewModel
    {
    }

    public class ConfigurationPagingParameterWebViewModel : PagingParameterWebViewModel
    {
        public string ConfigurationType { get; set; }
    }
}