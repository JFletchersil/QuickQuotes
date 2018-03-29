using AngularApp.API.Models.WebViewModels.OAuth;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace AngularApp.API.Models.WebViewModels.OAuth
{
    public class Auth0SingleUserWebViewModel
    {
        [JsonProperty("user_id")]
        public string Guid { get; set; }

        [JsonProperty("nickname")]
        public string UserName { get; set; }

        [JsonProperty("email")]
        public string EmailAddress { get; set; }

        public string PhoneNumber
        {
            get
            {
                if (UserMetaData != null && !string.IsNullOrWhiteSpace(UserMetaData.PhoneNumber))
                {
                    return UserMetaData.PhoneNumber;
                }
                else
                {
                    return "";
                }
            }
            set
            {
                UserMetaData.PhoneNumber = value;
            }
        }

        [JsonProperty("email_verified")]
        public bool EmailConfirmed { get; set; }

        public bool IsAdmin 
        {
            get; set;
            //get
            //{
            //    if (AppMetaData != null && AppMetaData.Authorization.Roles.Any())
            //    {
            //        return AppMetaData.Authorization.Roles.Any(x => (x == "Administrator" || x == "SuperAdministrator" || x == "SuperAdmin"));
            //    }
            //    else
            //    {
            //        return false;
            //    }
            //}
            //set
            //{
            //    return value;
            //}
        }

        [JsonProperty("user_metadata")]
        public Auth0UserMetaDataWebViewModel UserMetaData { get; set; }

        [JsonProperty("app_metadata")]
        public Auth0AppMetaDataWebViewModel AppMetaData { get; set; }

        // Properties that need to be added manually
        public bool AccountLocked { get; set; }
    }
}