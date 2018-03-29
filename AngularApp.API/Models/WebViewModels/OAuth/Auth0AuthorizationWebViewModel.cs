using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AngularApp.API.Models.WebViewModels.OAuth
{
    public class Auth0AuthorizationWebViewModel
    {
        [JsonProperty("groups")]
        public List<string> Groups { get; set; }
        [JsonProperty("roles")]
        public List<string> Roles { get; set; }
        [JsonProperty("permissions")]
        public List<string> Permissions { get; set; }
    }
}