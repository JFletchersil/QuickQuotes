using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AngularApp.API.Models.WebViewModels.OAuthModels
{
    /// <summary>
    /// Provides a non-complete representation of a single user within the Auth0 Database
    /// </summary>
    /// <remarks>
    /// This class has the scope for extension if required, otherwise only the valid details were
    /// added to the data model.
    /// </remarks>
    public class Auth0LightUserWebViewModel
    {
        /// <summary>
        /// The number of times a user has logged in
        /// </summary>
        /// <value>
        /// The logins count.
        /// </value>
        public int Logins_Count { get; set; }
        /// <summary>
        /// The user GUID that uniquely identifies the user
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public string User_Id { get; set; }
        /// <summary>
        /// Gives the DateTime of the last time the user logged in
        /// </summary>
        /// <value>
        /// The last login.
        /// </value>
        public DateTime Last_Login { get; set; }
        /// <summary>
        /// Gives the user name for the user
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
        /// <summary>
        /// Gives the email for the user
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        public string Email { get; set; }
    }
}