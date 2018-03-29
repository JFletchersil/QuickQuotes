using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AngularApp.API.Models.WebViewModels.OAuth
{
    public class Auth0UserMetaDataWebViewModel
    {
        [JsonProperty("fullname")]
        public string FullName { get; set; }
        [JsonProperty("phonenumber")]
        public string PhoneNumber { get; set; }
    }
}