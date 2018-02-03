using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using AngularApp.API.Models.DBModels;
using AngularApp.API.Models.WebViewModels;
using AngularApp.API.Repository;
using PetaPoco;

namespace AngularApp.API.Controllers
{
    [EnableCors("*", "*", "*")]
    [RoutePrefix("api/Users")]
    public class UsersController : ApiController
    {
        [HttpPost]
        public UserDetails ReturnUserModel(HttpRequestMessage request)
        {
            var fullName = request.Content.ReadAsStringAsync().Result;
            var db = new Database("AngularUsers");
            return db.Query<UserDetails>($"SELECT * FROM AspNetUserDetails WHERE FullName = '{fullName}'")
                .FirstOrDefault();
        }

        [HttpPost]
        public bool SaveUserModel(UserDetails userDetails)
        {
            var db = new Database("AngularUsers");
            var returnVal = db.Update("AspNetUserDetails", "Id", userDetails);
            return returnVal == 1;
        }
    }
}