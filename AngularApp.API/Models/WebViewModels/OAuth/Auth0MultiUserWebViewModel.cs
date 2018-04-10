using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AngularApp.API.Models.WebViewModels.OAuthModels
{
    /// <summary>
    /// Models a collection of multiple users in a form that allows the string value to be deserialised
    /// </summary>
    /// <remarks>
    /// To handle the issues provided with deserialising text data, I have had to encase the
    /// List of Auth0LightUserWebViewModel inside another model to provide easy deserialisation
    /// </remarks>
    /// <seealso cref="Auth0LightUserWebViewModel"/>
    public class Auth0MultiUserWebViewModel
    {
        /// <summary>
        /// The users gathered from the Auth0 Database
        /// </summary>
        public List<Auth0LightUserWebViewModel> Users;
    }
}