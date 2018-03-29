using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AngularApp.API.Models.WebViewModels.OAuth
{
    public class Auth0RoleWebViewModel
    {
        [JsonProperty("_id")]
        public string Id { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("permissions")]
        public List<string> Permissions { get; set; }
        [JsonProperty("users")]
        public List<string> Users { get; set; }
    }
}