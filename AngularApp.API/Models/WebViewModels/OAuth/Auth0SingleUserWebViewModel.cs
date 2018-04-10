using Newtonsoft.Json;

namespace AngularApp.API.Models.WebViewModels.OAuthModels
{
    /// <summary>
    /// A representation of a single user retrieved from the Management API
    /// </summary>
    public class Auth0SingleUserWebViewModel
    {
        /// <summary>
        /// The id that uniquely identifies that specific given user
        /// </summary>
        [JsonProperty("user_id")]
        public string Guid { get; set; }

        /// <summary>
        /// The username of a given user
        /// </summary>
        [JsonProperty("nickname")]
        public string UserName { get; set; }

        /// <summary>
        /// The email address of a given user
        /// </summary>
        [JsonProperty("email")]
        public string EmailAddress { get; set; }

        /// <summary>
        /// The phone number of a given user
        /// </summary>
        /// <remarks>
        /// Retreieves the data from the meta data, and returns an empty string if it doesn't exist
        /// </remarks>
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

        /// <summary>
        /// Returns a bool parameter that represents if the user has been verified via email or not
        /// </summary>
        [JsonProperty("email_verified")]
        public bool EmailConfirmed { get; set; }

        /// <summary>
        /// Returns a bool parameter that denotes if the user is an Admin level user or not
        /// </summary>
        /// <remarks>
        /// This is to be filled in via object creation, it will NOT be serialised into existence for you
        /// </remarks>
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

        /// <summary>
        /// A summation of the users meta data, includes information such phone number and full name
        /// </summary>
        /// <remarks>
        /// More details about the model can be found in Auth0UserMetaDataWebViewModel
        /// </remarks>
        /// <see cref="Auth0UserMetaDataWebViewModel"/>
        [JsonProperty("user_metadata")]
        public Auth0UserMetaDataWebViewModel UserMetaData { get; set; }

        /// <summary>
        /// A summation of the users app meta data, includes information from roles and permissions
        /// </summary>
        /// <remarks>
        /// More details about the model can be found in Auth0AppMetaDataWebViewModel
        /// </remarks>
        /// <see cref="Auth0AppMetaDataWebViewModel"/>
        [JsonProperty("app_metadata")]
        public Auth0AppMetaDataWebViewModel AppMetaData { get; set; }

        /// <summary>
        /// Returns a bool parameter which signals if a user's account is locked or not
        /// </summary>
        /// <remarks>
        /// This is a property that need to be added manually based on input from elsewhere
        /// </remarks>
        public bool AccountLocked { get; set; }
    }
}