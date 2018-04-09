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
    /// <summary>
    /// Provides methods to alter and adjust the user details section of the user database
    /// </summary>
    /// <remarks>
    /// This is distinct from the Account controller section due to the fact that this information is not
    /// associated with the direct user object.
    /// In future, this controller will be removed when/if Auth0 is used, instead this will be run through the
    /// meta-data sections of the Auth0 Controller.
    /// </remarks>
    [EnableCors("*", "*", "*")]
    public class UserDetailsController : ApiController
    {
        // TODO - Change this to a GUID
        /// <summary>
        /// Returns a given User Model when given a correct full name
        /// </summary>
        /// <param name="request">The fullname/guid of a given user</param>
        /// <returns>A model representing the user details of a given user</returns>
        /// <remarks>
        /// This model will contain meta-data that is not specifically required to generate the quotes,
        /// but might be useful for providing additional context and restrictions to identify user changes.
        /// </remarks>
        [HttpPost]
        public UserDetails ReturnUserModel(HttpRequestMessage request)
        {
            var fullName = request.Content.ReadAsStringAsync().Result;
            var db = new Database("AngularUsers");
            return db.Query<UserDetails>($"SELECT * FROM AspNetUserDetails WHERE FullName = '{fullName}'")
                .FirstOrDefault();
        }

        /// <summary>
        /// Saves a given model of user details when given a valid user details model
        /// </summary>
        /// <param name="userDetails">A user details model with updated or changed parameters</param>
        /// <returns>A bool representing if the change was successful or not</returns>
        /// <remarks>
        /// This uses PetaPoco to update the database, this allows us to sidestep update issues present
        /// within other sections of the project.
        /// If we get a return of 1, it indicates that a single row has been changed, and thus has updated
        /// correctly.
        /// </remarks>
        [HttpPost]
        public bool SaveUserModel(UserDetails userDetails)
        {
            var db = new Database("AngularUsers");
            var returnVal = db.Update("AspNetUserDetails", "Id", userDetails);
            return returnVal == 1;
        }
    }
}