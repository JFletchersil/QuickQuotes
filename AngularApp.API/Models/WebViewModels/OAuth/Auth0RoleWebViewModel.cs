using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AngularApp.API.Models.WebViewModels.OAuth
{
    /// <summary>
    /// A representation of a single role, as well as all users and permissions associated with the role
    /// </summary>
    public class Auth0RoleWebViewModel
    {
        /// <summary>
        /// The Auth0 GUID that uniquely identifies a given role
        /// </summary>
        [JsonProperty("_id")]
        public string Id { get; set; }
        /// <summary>
        /// The description of a given role
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }
        /// <summary>
        /// The name of a given role, used to identify the role in a more user friendly format
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// A list of permissions associated with the role
        /// </summary>
        [JsonProperty("permissions")]
        public List<string> Permissions { get; set; }
        /// <summary>
        /// A list of users associated with the role
        /// </summary>
        [JsonProperty("users")]
        public List<string> Users { get; set; }
    }
}