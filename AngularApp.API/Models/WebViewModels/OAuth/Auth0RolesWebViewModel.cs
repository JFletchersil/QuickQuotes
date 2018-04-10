using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AngularApp.API.Models.WebViewModels.OAuthModels
{
    /// <summary>
    /// Provides a wrapper to store a collection of roles when gathered via a Role Request from 
    /// the Authentication Extension API.
    /// </summary>
    /// <remarks>
    /// As before, this exists as a wrapper for deserialisation purposes into a usable format.
    /// </remarks>
    /// <seealso cref="Auth0RoleWebViewModel"/>
    public class Auth0RolesWebViewModel
    {
        /// <summary>
        /// A collection of roles and their associated members
        /// </summary>
        public List<Auth0RoleWebViewModel> Roles { get; set; }
    }
}