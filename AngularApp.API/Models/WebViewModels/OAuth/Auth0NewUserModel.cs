using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AngularApp.API.Models.WebViewModels.OAuth
{
    /// <summary>
    /// A model representing a new Auth0 user
    /// </summary>
    /// <remarks>
    /// This model is used in the creation of new Auth0 users only, and it should only be so.
    /// </remarks>
    public class Auth0NewUserModel
    {
        /// <summary>
        /// The connection type for the user, this is how the user will authenticate
        /// </summary>
        /// <remarks>
        /// In the case of a standard username/email password authenticate, this will be displayed as
        /// Username-Password-Authentication
        /// </remarks>
        public string connection { get; set; }
        /// <summary>
        /// The email address for the new user
        /// </summary>
        [Required]
        public string email { get; set; }
        /// <summary>
        /// The username for the new user
        /// </summary>
        [Required]
        public string username { get; set; }
        /// <summary>
        /// The password for the new user, if set to blank they will be prompted to create a new password
        /// </summary>
        public string password { get { return ""; } }
        /// <summary>
        /// The phone number for the new user
        /// </summary>
        public string phone_number { get; set; }
        /// <summary>
        /// Sets that the new user must verify their email address
        /// </summary>
        public bool verify_email { get { return true; } }
    }
}