using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using AngularApp.API.Models.DBModels;
using AngularApp.API.Models.WebViewModels;
using AngularApp.API.Models.WebViewModels.OAuth;
using AngularApp.API.ActionFilters;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace AngularApp.API.Controllers
{
    [EnableCors("*", "*", "*")]
    [APIDisabled]
    public class Auth0Controller : ApiController
    {
        private readonly Entities _dbContext = null;
        private string AuthenticationExtenBearerToken = "";
        private string ManagementBearerToken = "";
        private readonly string Connection = "Username-Password-Authentication";
        private readonly string ClientIDFrontEndApp = "cxX97IXqqTE0ziE4w5MfG4GJD1DayIBA";

        public Auth0Controller()
        {
            _dbContext = new Entities();
        }

        [HttpGet]
        public List<QueueUserWebViewModel> ReturnAllUsers()
        {
            var roles = ReturnAllRoles();
            var users = JsonConvert.DeserializeObject<Auth0MultiUserWebViewModel>(RequestWithStringReturn("users", Method.GET, AuthenticationExtenBearerToken));
            var singleUsers = new List<Auth0SingleUserWebViewModel>();
            foreach (var user in users.Users)
            {
                var saUser = JsonConvert.DeserializeObject<Auth0SingleUserWebViewModel>(RequestWithStringReturn($"users/{user.User_Id}", Method.GET, AuthenticationExtenBearerToken));
                saUser.IsAdmin = roles.Roles.Any(x => (x.Name == "Administrator" && x.Users.Any(y => y == user.User_Id) ||
                                                ((x.Name == "SuperAdministrator") && x.Users.Any(y => y == user.User_Id))));
                singleUsers.Add(saUser);
            }
            var trueRetVal = Mapper.Map<List<QueueUserWebViewModel>>(singleUsers);

            return trueRetVal;
        }

        [HttpGet]
        public bool AddUserToRole(string userID, string roleName)
        {
            var roles = ReturnAllRoles();
            var usID = roles.Roles.FirstOrDefault(x => x.Name == roleName).Id;

            return string.IsNullOrEmpty(PatchRequestItems($"users/{userID}/roles", AuthenticationExtenBearerToken, usID));
        }

        [HttpGet]
        public bool DeleteUserFromRole(string userID, string roleName)
        {
            var roles = ReturnAllRoles();
            var usID = roles.Roles.FirstOrDefault(x => x.Name == roleName).Id;

            return string.IsNullOrEmpty(DeleteRequestItems($"users/{userID}/roles", AuthenticationExtenBearerToken, usID));
        }

        [HttpGet]
        public bool ResetPassword(string email)
        {
            var client = new RestClient($"https://quickquotes.eu.auth0.com/dbconnections/change_password");
            var request = new RestRequest(Method.POST);
            request.AddHeader("authorization", $"Bearer {AuthenticationExtenBearerToken}");
            request.AddHeader("Content-type", "application/json; charset=utf-8");
            request.AddJsonBody(new { client_id = ClientIDFrontEndApp, email, password = "", connection = Connection });
            IRestResponse response = client.Execute(request);
            return response.IsSuccessful;
        }

        [HttpGet]
        public bool UpdateRootDocument(string parameterSet, string parameterValue, string userID)
        {
            var client = new RestClient($"https://quickquotes.eu.auth0.com/api/v2/users/{userID}");
            var request = new RestRequest(Method.PATCH);
            request.AddHeader("authorization", $"Bearer {ManagementBearerToken}");
            request.AddHeader("Content-type", "application/json; charset=utf-8");
            var dict = new Dictionary<string, string>
            {
                { parameterSet, parameterValue }
            };
            request.AddJsonBody(dict);
            IRestResponse response = client.Execute(request);
            return response.IsSuccessful;
        }

        [HttpGet]
        public bool CloneRolesFromUserToNewUser(string clientID)
        {
            return false;
        }

        [HttpPost]
        public bool UpdateUserMetaData(JObject metaUpdate)
        {
            var value = metaUpdate.Property("user_id").Value.ToString();
            var client = new RestClient($"https://quickquotes.eu.auth0.com/api/v2/users/{value}");
            var request = new RestRequest(Method.PATCH);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("authorization", $"Bearer {ManagementBearerToken}");
            request.AddParameter("application/json", $"{{\"user_metadata\": {JsonConvert.SerializeObject(metaUpdate)}}}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response.IsSuccessful;
        }

        [HttpPost]
        public bool CreateNewUser(Auth0NewUserModel newModel)
        {
            newModel.connection = Connection;
            var client = new RestClient("https://quickquotes.eu.auth0.com/api/v2/users");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-type", "application/json; charset=utf-8");
            request.AddJsonBody(newModel);
            IRestResponse response = client.Execute(request);
            return response.IsSuccessful;
        }

        private string RequestWithStringReturn(string targetEndpoint, Method methodType, string bearerToken)
        {
            var client = new RestClient($"https://quickquotes.eu.webtask.io/adf6e2f2b84784b57522e3b19dfc9201/api/{targetEndpoint}");
            var request = new RestRequest(methodType);
            request.AddHeader("authorization", $"Bearer {bearerToken}");
            IRestResponse response = client.Execute(request);
            return response.Content;
        }

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

        private Auth0RolesWebViewModel ReturnAllRoles()
        {
            return JsonConvert.DeserializeObject<Auth0RolesWebViewModel>(RequestWithStringReturn("roles", Method.GET, AuthenticationExtenBearerToken));
        }

        private void GetExtensionAuthToken()
        {
            var client = new RestClient($"https://quickquotes.eu.auth0.com/oauth/token");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-type", "application/json; charset=utf-8");
            request.AddJsonBody(new { grant_type = "client_credentials", client_id = "wuytT8ma2Ol2U5B91KWvMchVxMcHvPIr", client_secret = "uPhE-pSnsb62P452dWprJkJ7MKlQkKrcroQXT-Y_MVnAqJ8h-VDeYg-5EHPaWtk6", audience = "urn:auth0-authz-api" });
            IRestResponse response = client.Execute(request);
            AuthenticationExtenBearerToken = JsonConvert.DeserializeObject<JObject>(response.Content).Property("access_token").Value.ToString();
        }

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
