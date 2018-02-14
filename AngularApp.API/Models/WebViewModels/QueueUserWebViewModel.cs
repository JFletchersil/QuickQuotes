using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AngularApp.API.Models.WebViewModels
{
    public class QueueUserWebViewModel
    {
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public bool IsAdmin { get; set; }
        public bool AccountLocked { get; set; }
        public bool HasTwoFactor { get; set; }
    }

    public class PaginatedQueueUserResult
    {
        public IEnumerable<QueueUserWebViewModel> QueueDisplay;

        public int TotalPages;
        public int TotalItems { get; set; }
    }
}