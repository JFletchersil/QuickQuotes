using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AngularApp.API.Models.WebViewModels
{
    public class QuickSearch
    {
        public string QuoteReference { get; set; }
        public string QuoteType { get; set; }
        public string QuoteAuthor { get; set; }
    }

    public class QueueDisplayWebViewModel
    {
        public string QuoteReference { get; set; }
        public string QuoteType { get; set; }
        public string QuoteStatus { get; set; }
        public DateTime QuoteDate{ get; set; }
        public string QuoteAuthor { get; set; }
    }

    public class PaginatedQueueResult
    {
        public IEnumerable<QueueDisplayWebViewModel> QueueDisplay;

        public int TotalPages;
        public int TotalItems { get; set; }
    }
}