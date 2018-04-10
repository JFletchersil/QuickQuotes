using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AngularApp.API.Models.WebViewModels.OAuthModels
{
    /// <summary>
    /// Represents the bits of user meta-data we are interested in
    /// </summary>
    /// <remarks>
    /// This class has scope for extension to represent more enhancements to the meta data if required.
    /// However, it only maps the users full name and phone number at the minute.
    /// </remarks>
    public class Auth0UserMetaDataWebViewModel
    {
        /// <summary>
        /// A full name of a user stored within the meta data of a user
        /// </summary>
        /// <value>
        /// The full name.
        /// </value>
        [JsonProperty("fullname")]
        public string FullName { get; set; }
        /// <summary>
        /// The phone number of a user stored within the meta data of a user
        /// </summary>
        /// <value>
        /// The phone number.
        /// </value>
        [JsonProperty("phonenumber")]
        public string PhoneNumber { get; set; }
    }
}