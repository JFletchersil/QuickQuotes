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
        [Required]
        public bool ReturnAll { get; set; }
        public string ConfigurationType { get; set; }
    }
}