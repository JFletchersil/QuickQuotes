using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AngularApp.API.Models.WebViewModels.OAuthModels
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
        /// <value>
        /// The connection.
        /// </value>
        /// <remarks>
        /// In the case of a standard username/email password authenticate, this will be displayed as
        /// Username-Password-Authentication
        /// </remarks>
        public string connection { get; set; }
        /// <summary>
        /// The email address for the new user
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        [Required]
        public string email { get; set; }
        /// <summary>
        /// The username for the new user
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        [Required]
        public string username { get; set; }
        /// <summary>
        /// The password for the new user, if set to blank they will be prompted to create a new password
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string password { get { return ""; } }
        /// <summary>
        /// The phone number for the new user
        /// </summary>
        /// <value>
        /// The phone number.
        /// </value>
        public string phone_number { get; set; }
        /// <summary>
        /// Sets that the new user must verify their email address
        /// </summary>
        /// <value>
        ///   <c>true</c> if [verify email]; otherwise, <c>false</c>.
        /// </value>
        public bool verify_email { get { return true; } }
    }
}