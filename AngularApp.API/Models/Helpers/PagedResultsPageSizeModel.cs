using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AngularApp.API.Models.Helpers
{
    public class PagedResultsPageSizeModel<T>
    {
        public int TotalPages { get; set; }
        public List<T> Items { get; set; }
    }
}