using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using AngularApp.API.ActionFilters;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using AngularApp.API.Models.DBModels;
using AngularApp.API.Models.WebViewModels.OAuthModels;
using AngularApp.API.Models.WebViewModels.PagingModels;
using AngularApp.API.Models.WebViewModels.UserQueueDisplayModels;

namespace AngularApp.API.Controllers
{
    /// <summary>
    /// The Auth0 Controller, it allows interaction, via their API to the Auth0 user management system
    /// This will replace the Account Controller when/if we decide to move to Auth0 instead of a self hosted Database
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    /// <remarks>
    /// This API is missing several features that would fully account for all the functionality in the account
    /// controller, however those features are not required for the full running of the account system
    /// In future the API must be secured via the Auth0 middle wear and OWIN in order to fully complete the
    /// transition to Auth0 and it's authentication.
    /// This API is disabled via an APIDiasabled attribute, this attribute must be removed prior to using the
    /// Auth0 controller. This will require another compilation
    /// </remarks>
    [EnableCors("*", "*", "*")]
    [APIDisabled]
    public class Auth0Controller : ApiController
    {
        // Provides a context location to the database containing all application user data
        /// <summary>
        /// The database context
        /// </summary>
        private readonly Entities _dbContext = null;
        // Locations to store the Tokens for Authentication and management API
        // TODO - Must place these as configuration options inside the Web.config area
        /// <summary>
        /// The authentication extension bearer token
        /// </summary>
        private string AuthenticationExtenBearerToken = "";
        /// <summary>
        /// The management bearer token
        /// </summary>
        private string ManagementBearerToken = "";
        // Locations to configure the connection type
        // TODO - Must place this as configuration option inside the Web.config area
        /// <summary>
        /// The connection
        /// </summary>
        private readonly string Connection = "User name-Password-Authentication";
        // Sets the ClientID for the front end app being used for communication
        // TODO - Must place this as configuration option inside the Web.config area
        /// <summary>
        /// The client identifier front end application
        /// </summary>
        private readonly string ClientIDFrontEndApp = "cxX97IXqqTE0ziE4w5MfG4GJD1DayIBA";

        /// <summary>
        /// Initializes the Auth0 Controller with a entity context, allowing access to the Quote Database.
        /// Constructs the default details for the controller on start up.
        /// </summary>
        /// <remarks>
        /// It's the only constructor, so you must use this.
        /// </remarks>
        public Auth0Controller()
        {
            _dbContext = new Entities();
        }

        /// <summary>
        /// Returns a full list of all users contained within the User Database stored in Auth0 Database.
        /// Requires a paging parameter model in order to set the correct ordering and paging for the client.
        /// </summary>
        /// <param name="param">A Paging parameter model, this is used to configure the paging on the server,
        /// as well as setting the order and the page of users that should be returned.</param>
        /// <returns>
        /// A list of users, and their details. Including role, and meta-data.
        /// </returns>
        /// <remarks>
        /// This is used to return users for the administration screen, otherwise it should not be
        /// used for any other purpose.
        /// </remarks>
        /// <example>
        /// This is called like a normal API call, this would take the form of POST [webaddress]/api/ReturnAllUsers
        /// </example>
        [HttpGet]
        public List<QueueUserWebViewModel> ReturnAllUsers(PagingParameterWebViewModel param)
        {
            // Gather all roles prior to forming the users.
            var roles = ReturnAllRoles();
            // Gather all users, in a multi user web view model for processing.
            var users = JsonConvert.DeserializeObject<Auth0MultiUserWebViewModel>(RequestWithStringReturn("users", Method.GET, AuthenticationExtenBearerToken));
            // Creates a list object to hold all the processed single users.
            var singleUsers = new List<Auth0SingleUserWebViewModel>();
            // Processes the list of users from the multi user view and converts it into a more readable single
            // user view. Contains the basics such as locked account, and meta data, as well as roles.
            foreach (var user in users.Users)
            {
                var saUser = JsonConvert.DeserializeObject<Auth0SingleUserWebViewModel>(RequestWithStringReturn($"users/{user.User_Id}", Method.GET, AuthenticationExtenBearerToken));
                saUser.IsAdmin = roles.Roles.Any(x => (x.Name == "Administrator" && x.Users.Any(y => y == user.User_Id) ||
                                                ((x.Name == "SuperAdministrator") && x.Users.Any(y => y == user.User_Id))));
                singleUsers.Add(saUser);
            }
            // Maps the values back to the expected format for the web page, can be removed if the need for conversion
            // is removed.
            var trueRetVal = Mapper.Map<List<QueueUserWebViewModel>>(singleUsers);

            return trueRetVal;
        }

        /// <summary>
        /// Adds the user to role.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <param name="roleName">Name of the role.</param>
        /// <returns></returns>
        [HttpGet]
        public bool AddUserToRole(string userID, string roleName)
        {
            // Gathers all roles and uses that to determine the role ID of the given role name
            var roles = ReturnAllRoles();
            var usID = roles.Roles.FirstOrDefault(x => x.Name == roleName).Id;

            // Makes the request and changes the Role
            return string.IsNullOrEmpty(PatchRequestItems($"users/{userID}/roles", AuthenticationExtenBearerToken, usID));
        }

        /// <summary>
        /// Deletes the user from role.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <param name="roleName">Name of the role.</param>
        /// <returns></returns>
        [HttpGet]
        public bool DeleteUserFromRole(string userID, string roleName)
        {
            // Gathers all roles and uses that to determine the role ID of the given role name
            var roles = ReturnAllRoles();
            var usID = roles.Roles.FirstOrDefault(x => x.Name == roleName).Id;

            // Makes the request and changes the Role
            return string.IsNullOrEmpty(DeleteRequestItems($"users/{userID}/roles", AuthenticationExtenBearerToken, usID));
        }

        /// <summary>
        /// Resets a given users password on request by an administrator
        /// </summary>
        /// <param name="email">The email address of the user whose password is being reset</param>
        /// <returns>
        /// A <see langword="bool"/> that determines if the reset was successful or not
        /// </returns>
        /// <remarks>
        /// This is to be used by the administrator if the administrator wishes to forcibly reset a users
        /// password. This should only be used by administrators, users have alternate methods for forcing a reset.
        /// </remarks>
        /// <example>
        /// Get [websitename]/api/ResetPassword?email=email
        /// </example>
        [HttpGet]
        public bool ResetPassword(string email)
        {
            // Configures the client and request to be sent for the password reset
            var client = new RestClient($"https://quickquotes.eu.auth0.com/dbconnections/change_password");
            // Sets it to a post request
            var request = new RestRequest(Method.POST);
            // Adds the content type as well as authorization token
            request.AddHeader("authorization", $"Bearer {AuthenticationExtenBearerToken}");
            request.AddHeader("Content-type", "application/json; charset=utf-8");
            // Adds the JSON to the body to be posted, includes email, blank password and connection type
            request.AddJsonBody(new { client_id = ClientIDFrontEndApp, email, password = "", connection = Connection });
            // Executes and evaluates the success of the request
            IRestResponse response = client.Execute(request);
            return response.IsSuccessful;
        }

        /// <summary>
        /// Updates the root document.
        /// </summary>
        /// <param name="parameterSet">The parameter set.</param>
        /// <param name="parameterValue">The parameter value.</param>
        /// <param name="userID">The user identifier.</param>
        /// <returns></returns>
        [HttpGet]
        public bool UpdateRootDocument(string parameterSet, string parameterValue, string userID)
        {
            // Creates the client and request to update the root document
            var client = new RestClient($"https://quickquotes.eu.auth0.com/api/v2/users/{userID}");
            var request = new RestRequest(Method.PATCH);
            // Adds the headers to the request to allow the data to be sent and authenticated
            request.AddHeader("authorization", $"Bearer {ManagementBearerToken}");
            request.AddHeader("Content-type", "application/json; charset=utf-8");
            // Creates the Dictionary containing the request data
            var dict = new Dictionary<string, string>
            {
                { parameterSet, parameterValue }
            };
            // Adds the dictionary to the request data
            request.AddJsonBody(dict);
            // Makes the request and evaluates the success of the response
            IRestResponse response = client.Execute(request);
            return response.IsSuccessful;
        }

        /// <summary>
        /// Clones a user, and their roles, from an old role to a new one
        /// </summary>
        /// <param name="clientID">The client identifier.</param>
        /// <returns>
        /// False, function has not been implemented
        /// </returns>
        /// <remarks>
        /// The function has not been implemented yet.
        /// </remarks>
        /// <example>
        /// N/a
        /// </example>
        [HttpGet]
        public bool CloneRolesFromUserToNewUser(string clientID)
        {
            return false;
        }

        /// <summary>
        /// Updates a single users meta data via JSON Object
        /// Is limited in scope in how much can be changed at once, and so it should try to keep the number
        /// of simultaneous changes down.
        /// </summary>
        /// <param name="metaUpdate">The JSON Representation of the data being changed</param>
        /// <returns>
        /// A <see langword="bool"/> notifying if the user has successfully altered their meta-data or not
        /// </returns>
        /// <remarks>
        /// This is used to handle the standard user meta-data, and not root level data.
        /// This is data stored in the meta data section, and no data in the high level area.
        /// </remarks>
        /// <example>
        /// Post [websitename]/api/UpdateUserMetaData [metaUpdate]
        /// </example>
        [HttpPost]
        public bool UpdateUserMetaData(JObject metaUpdate)
        {
            // Gets the client id value to isolate the user to update the user meta data
            var value = metaUpdate.Property("user_id").Value.ToString();
            // Creates the client and request to update the meta-data document
            var client = new RestClient($"https://quickquotes.eu.auth0.com/api/v2/users/{value}");
            var request = new RestRequest(Method.PATCH);
            // Adds the headers to the request to allow the data to be sent and authenticated
            request.AddHeader("content-type", "application/json");
            request.AddHeader("authorization", $"Bearer {ManagementBearerToken}");
            // Serializes the JObject to string for patching to the server
            request.AddParameter("application/json", $"{{\"user_metadata\": {JsonConvert.SerializeObject(metaUpdate)}}}", ParameterType.RequestBody);
            // Makes the request and evaluates the success of the response
            IRestResponse response = client.Execute(request);
            return response.IsSuccessful;
        }

        /// <summary>
        /// Creates a new user when given a new user model.
        /// </summary>
        /// <param name="newModel">A model for the new user that is being created</param>
        /// <returns>
        /// A <see langword="bool"/> notifying if the user has successfully created a new user or not
        /// </returns>
        /// <remarks>
        /// Allows the creation of a new user, must present at least email and user name for successful user creation.
        /// </remarks>
        /// <example>
        /// Post [websitename]/api/CreateNewUser [newModel]
        /// </example>
        [HttpPost]
        public bool CreateNewUser(Auth0NewUserModel newModel)
        {
            // Adds the connection type, required to identify this as a email/password combo
            newModel.connection = Connection;
            // Creates the client and request to add a new user 
            var client = new RestClient("https://quickquotes.eu.auth0.com/api/v2/users");
            var request = new RestRequest(Method.POST);
            // Adds the headers to the request to allow the data to be sent and authenticated
            request.AddHeader("Content-type", "application/json; charset=utf-8");
            request.AddJsonBody(newModel);
            // Makes the request and evaluates the success of the response
            IRestResponse response = client.Execute(request);
            return response.IsSuccessful;
        }

        /// <summary>
        /// Makes a single request, to 0Auth, where it is expected that we will get in return a string
        /// Works only for the authentication extension
        /// </summary>
        /// <param name="targetEndpoint">The target endpoint, where the request is being made</param>
        /// <param name="methodType">What type the method is, such as POST</param>
        /// <param name="bearerToken">The correct bearer token needed to authenticate the request</param>
        /// <returns>
        /// The return string value for evaluation later
        /// </returns>
        private string RequestWithStringReturn(string targetEndpoint, Method methodType, string bearerToken)
        {
            var client = new RestClient($"https://quickquotes.eu.webtask.io/adf6e2f2b84784b57522e3b19dfc9201/api/{targetEndpoint}");
            var request = new RestRequest(methodType);
            request.AddHeader("authorization", $"Bearer {bearerToken}");
            IRestResponse response = client.Execute(request);
            return response.Content;
        }

        /// <summary>
        /// Makes a single patch request to a target endpoint on the authentication extension API
        /// </summary>
        /// <param name="targetEndpoint">The target endpoint, where the request is being made</param>
        /// <param name="bearerToken">The correct bearer token needed to authenticate the request</param>
        /// <param name="roleId">The role id for a given user, used to change the roles of a user</param>
        /// <returns>
        /// The return string value for evaluation later
        /// </returns>
        private string PatchRequestItems(string targetEndpoint, string bearerToken, string roleId)
        {
            var client = new RestClient($"https://quickquotes.eu.webtask.io/adf6e2f2b84784b57522e3b19dfc9201/api/{targetEndpoint}");
            var request = new RestRequest(Method.PATCH);
            request.AddHeader("authorization", $"Bearer {bearerToken}");
            request.AddHeader("Content-type", "application/json; charset=utf-8");
            request.AddJsonBody(new List<string>() { roleId });
            IRestResponse response = client.Execute(request);
            return response.Content;
        }

        /// <summary>
        /// Makes a single delete request to a target endpoint on the authentication extension API
        /// </summary>
        /// <param name="targetEndpoint">The target endpoint, where the request is being made</param>
        /// <param name="bearerToken">The correct bearer token needed to authenticate the request</param>
        /// <param name="roleId">The role id for a given user, used to change the roles of a user</param>
        /// <returns>
        /// The return string value for evaluation later
        /// </returns>
        private string DeleteRequestItems(string targetEndpoint, string bearerToken, string roleId)
        {
            var client = new RestClient($"https://quickquotes.eu.webtask.io/adf6e2f2b84784b57522e3b19dfc9201/api/{targetEndpoint}");
            var request = new RestRequest(Method.DELETE);
            request.AddHeader("authorization", $"Bearer {bearerToken}");
            request.AddHeader("Content-type", "application/json; charset=utf-8");
            request.AddJsonBody(new List<string>() { roleId });
            IRestResponse response = client.Execute(request);
            return response.Content;
        }

        /// <summary>
        /// Returns a summary of all Roles within the application
        /// </summary>
        /// <returns>
        /// Returns an Auth0RolesWebViewModel containing all possible roles within the Auth0 application.
        /// </returns>
        private Auth0RolesWebViewModel ReturnAllRoles()
        {
            return JsonConvert.DeserializeObject<Auth0RolesWebViewModel>(RequestWithStringReturn("roles", Method.GET, AuthenticationExtenBearerToken));
        }

        /// <summary>
        /// Gets the Authentication Extension token when provided with the correct details.
        /// Needed to refresh the AuthToken when required.
        /// </summary>
        private void GetExtensionAuthToken()
        {
            var client = new RestClient($"https://quickquotes.eu.auth0.com/oauth/token");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-type", "application/json; charset=utf-8");
            request.AddJsonBody(new { grant_type = "client_credentials", client_id = "wuytT8ma2Ol2U5B91KWvMchVxMcHvPIr", client_secret = "uPhE-pSnsb62P452dWprJkJ7MKlQkKrcroQXT-Y_MVnAqJ8h-VDeYg-5EHPaWtk6", audience = "urn:auth0-authz-api" });
            IRestResponse response = client.Execute(request);
            AuthenticationExtenBearerToken = JsonConvert.DeserializeObject<JObject>(response.Content).Property("access_token").Value.ToString();
        }

        /// <summary>
        /// Gets the Management API token when provided with the correct details.
        /// Needed to refresh the AuthToken when required.
        /// </summary>
        private void GetManagementBearerAuthToken()
        {
            var client = new RestClient($"https://quickquotes.eu.auth0.com/oauth/token");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-type", "application/json; charset=utf-8");
            request.AddJsonBody(new { grant_type = "client_credentials", client_id = "gyWzEBpnFuB4AW6EhW6BqLiWKT8cIyLW", client_secret = "encks9HJV3sUTb9iYXzuhBmYHTXZT9tZw-fVaoaSp0SBgeM4ark2umvLVtMy2Qqc", audience = "https://quickquotes.eu.auth0.com/api/v2/" });
            IRestResponse response = client.Execute(request);
            AuthenticationExtenBearerToken = JsonConvert.DeserializeObject<JObject>(response.Content).Property("access_token").Value.ToString();
        }
    }
}