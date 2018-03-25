using System.Collections.Generic;

namespace AngularApp.API.Models.WebViewModels
{
    public class QueueUserWebViewModel
    {
        public string Guid { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool IsAdmin { get; set; }
        public bool AccountLocked { get; set; }
    }

    public class PaginatedQueueUserResult
    {
        public IEnumerable<QueueUserWebViewModel> QueueDisplay;

        public int TotalPages;
        public int TotalItems { get; set; }
    }
}