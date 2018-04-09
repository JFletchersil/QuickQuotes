using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AngularApp.API.Models.WebViewModels.OAuth
{
    public class Auth0NewUserModel
    {
        public string connection { get; set; }
        [Required]
        public string email { get; set; }
        [Required]
        public string username { get; set; }
        public string password { get { return ""; } }
        public string phone_number { get; set; }
        public bool verify_email { get { return true; } }
    }
}