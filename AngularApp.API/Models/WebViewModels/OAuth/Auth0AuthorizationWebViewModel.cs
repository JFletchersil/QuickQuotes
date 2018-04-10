using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AngularApp.API.Models.WebViewModels.OAuthModels
{
    /// <summary>
    /// Represents a collection of the Authorisation Extension API groups, roles and permissions
    /// </summary>
    /// <remarks>
    /// Each is a list of strings, groups in this context represents potential different customers,
    /// and roles denoting the access rights of various users.
    /// Permissions represents a pool of possible permissions that could be granted.
    /// </remarks>
    public class Auth0AuthorizationWebViewModel
    {
        /// <summary>
        /// A collection of groups stored within the Authorisation Extension API of Auth0
        /// </summary>
        /// <value>
        /// The groups.
        /// </value>
        [JsonProperty("groups")]
        public List<string> Groups { get; set; }
        /// <summary>
        /// A collection of roles stored within the Authorisation Extension API of Auth0
        /// </summary>
        /// <value>
        /// The roles.
        /// </value>
        [JsonProperty("roles")]
        public List<string> Roles { get; set; }
        /// <summary>
        /// A collection of permissions stored within the Authorisation Extension API of Auth0
        /// </summary>
        /// <value>
        /// The permissions.
        /// </value>
        [JsonProperty("permissions")]
        public List<string> Permissions { get; set; }
    }
}