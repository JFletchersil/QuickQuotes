using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AngularApp.API.Models.WebViewModels.OAuth
{
    public class Auth0LightUserWebViewModel
    {
        public int Logins_Count { get; set; }
        public string User_Id { get; set; }
        public DateTime Last_Login { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}