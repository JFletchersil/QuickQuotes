using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AngularApp.API.Models.WebViewModels.OAuth
{
    public class Auth0AppMetaDataWebViewModel
    {
        public Auth0AuthorizationWebViewModel Authorization { get; set; }
    }
}