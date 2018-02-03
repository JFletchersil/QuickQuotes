using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AngularApp.API.Models.WebViewModels
{
    public class PagingParameterWebViewModel
    {
        public int PageNumber { get; set; } = 1;

        //private int _pageSize { get; set; } = 10;
        public int PageSize { get; set; } = 10;

    }
}