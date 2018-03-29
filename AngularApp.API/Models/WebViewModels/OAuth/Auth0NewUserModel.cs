using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AngularApp.API.Models.WebViewModels.OAuth
{
    public class Auth0NewUserModel
    {
        public string connection { get; set; }
        public string email { get; set; }
        public string username { get; set; }
        public string password { get { return ""; } }
        public string phone_number { get; set; }
        public bool verify_email { get { return true; } }
    }
}