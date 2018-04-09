using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AngularApp.API.Models.WebViewModels.OAuth
{
    /// <summary>
    /// Provides a representation of the Auth0AppMetaData
    /// This is comprised of several different levels of models, 
    /// the first one represents a standard Auth0 Authorization model
    /// </summary>
    public class Auth0AppMetaDataWebViewModel
    {
        public Auth0AuthorizationWebViewModel Authorization { get; set; }
    }
}