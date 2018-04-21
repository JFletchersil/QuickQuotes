using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.Cors;
using AngularApp.API.Helpers;
using AngularApp.API.Models.DBModels;
using AngularApp.API.Models.WebViewModels.UserActionModels;
using AngularApp.API.Models.WebViewModels.AccountViewModels;
using AngularApp.API.Models.WebViewModels.PagingModels;
using AngularApp.API.Models.WebViewModels.UserQueueDisplayModels;
using AngularApp.API.Repository;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json.Linq;

namespace AngularApp.API.Controllers
{
    /// <summary>
    /// A collection of controllers designed to provide an api that allows for the full
    /// management of the application
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    internal class NamespaceDoc
    {

    }

    /// <summary>
    /// The Account Controller
    /// Provides access, via the API, to various operations that can be performed on the user database.
    /// Includes access to all functions regarding Account Functionality.
    /// </summary>
    /// <remarks>
    /// This controller will be replaced, if Auth0 is used. This works only on a self hosted Database in the style
    /// of Microsoft's authentication Database.
    /// There are no Cors Limits on the requests made.
    /// This has been partially created based off helper code from Microsoft.
    /// </remarks>
    [EnableCors("*", "*", "*")]
    public class AccountController : ApiController
    {
        /// <summary>
        /// The Authentication Repository
        /// </summary>
        private AuthRepository _repo = null;
        private readonly CommonFunctionsHelper _helper;

        /// <summary>
        /// Provides an initialisation for the AuthRepository, allowing you to communicate with the user database.
        /// </summary>
        public AccountController()
        {
            _repo = new AuthRepository();
            _helper = new CommonFunctionsHelper();
        }

        /// <summary>
        /// Provides an initialisation for the AuthRepository, for UnitTests to give the Account Controller a repository
        /// </summary>
        /// <param name="repo">A unit tested AuthRepository</param>
        public AccountController(AuthRepository repo)
        {
            _repo = repo;
            _helper = new CommonFunctionsHelper();
        }

        /// <summary>
        /// Logins the specified login view model.
        /// </summary>
        /// <param name="loginViewModel">The login view model.</param>
        /// <returns>A 200 or 500 response depending on if the login action was successful or not</returns>
        [HttpPost]
        public async Task<IHttpActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _repo.FindUser(loginViewModel.Email, loginViewModel.Password);

            if (result != null)
            {
                var db = new PetaPoco.Database("AngularUsers");
                var returnData = db.Query<UserDetails>($"SELECT * FROM AspNetUserDetails WHERE Id = '{result.Id}'").FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(returnData.FullName))
                {
                    return Ok(new JObject()
                    {
                        new JProperty("FullName", returnData?.FullName.Trim()),
                        new JProperty("UserName", result.UserName),
                        new JProperty("IsSuperAdmin", result.Roles.Any(y => y.RoleId == WebConfigurationManager.AppSettings["SuperAdministratorRole"])),
                        new JProperty("IsAdmin", result.Roles.Any(y => y.RoleId == WebConfigurationManager.AppSettings["AdministratorRole"]))
                    });
                }
                 return BadRequest();
            }
            return BadRequest();
        }

        // POST api/Account/Register
        /// <summary>
        /// Registers the specified user model.
        /// </summary>
        /// <param name="userModel">The user model.</param>
        /// <returns>A 200 or 500 response depending on if the user was successfully registered or not</returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IHttpActionResult> Register(RegisterViewModel userModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (userModel.Password != userModel.ConfirmPassword)
                return BadRequest();

            var result = await _repo.RegisterUser(userModel);

            var errorResult = GetErrorResult(result);

            return errorResult ?? Ok();
        }

        /// <summary>
        /// Returns all users.
        /// </summary>
        /// <param name="parameterWebView">The parameter web view.</param>
        /// <returns>Returns a 200 response with a list of all users or an exception</returns>
        [HttpPost]
        public IHttpActionResult ReturnAllUsers(AccountPagingParameterWebViewModel parameterWebView)
        {
            try
            {
                var users = _repo.GetAllUsers().ToList();

                // Orders the users according to a parameter if they were given an order by attribute
                users = _helper.ReturnSortedList(users, parameterWebView.OrderBy);

                // Filters out irrelevant users to the role type that was requested
                var tamp = users.Where(x =>
                        x.Roles.Any(y => y.RoleId == WebConfigurationManager.AppSettings["UserRole"]))
                    .ToList();
                var tamp2 = users.Where(x => x.Roles.Any(y => y.RoleId == WebConfigurationManager.AppSettings["AdministratorRole"]))
                    .ToList();
                var pagedUsers = !parameterWebView.ReturnAll
                    ? users.Where(x => x.Roles.Any(y => y.RoleId == WebConfigurationManager.AppSettings["UserRole"]))
                        .ToList()
                    : users.Where(x => x.Roles.Any(y => y.RoleId == WebConfigurationManager.AppSettings["AdministratorRole"]))
                        .ToList();

                // Paginates the results returned from the database once they have been filtered by role and 
                // ordered by a property
                var pagedModels = _helper.PaginateDbTables(pagedUsers, parameterWebView.PageNumber, parameterWebView.PageSize);

                var returnItems = pagedModels.Items.Select(x => new QueueUserWebViewModel()
                {
                    Guid = x.Id,
                    UserName = x.UserName,
                    AccountLocked = !x.LockoutEndDateUtc.HasValue,
                    EmailAddress = x.Email,
                    PhoneNumber = string.IsNullOrEmpty(x.PhoneNumber) ? "00000 000000" : x.PhoneNumber,
                    EmailConfirmed = x.EmailConfirmed,
                    IsAdmin = x.Roles.Any(y =>
                        y.RoleId == WebConfigurationManager.AppSettings["AdministratorRole"] ||
                        y.RoleId == WebConfigurationManager.AppSettings["SuperAdministratorRole"])
                });

                return Ok(new PaginatedQueueUserResult()
                {
                    QueueDisplay = returnItems,
                    TotalPages = pagedModels.TotalPages,
                    TotalItems = pagedModels.Items.Count
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        /// <summary>
        /// Returns all users at level for search.
        /// </summary>
        /// <remarks>
        /// Effectively, this is the search function used when there is a need to search
        /// for users at a given role level. This is used in the administration
        /// screen within the front end application.
        /// </remarks>
        /// <param name="viewModel">The view model.</param>
        /// <returns>Returns a 200 response with all users, at a given role level, or an exception</returns>
        [HttpPost]
        public IHttpActionResult ReturnAllUsersAtLevelForSearch(SearchAccounthWebViewModel viewModel)
        {
            var users = _repo.GetAllUsers();
            var returnItems = ConvertToWebViewModel(users, viewModel.ReturnAll);
            // Filters users based on the filter/search text provided
            var returnItem = returnItems.Where(x => TestFunction(x.UserName, viewModel.FilterTerm) || TestFunction(x.EmailAddress, viewModel.FilterTerm) ||
                                                    TestFunction(x.Guid, viewModel.FilterTerm) || TestFunction(x.PhoneNumber, viewModel.FilterTerm)).ToList();
            return Ok(returnItem);
        }

        /// <summary>
        /// Deletes the user.
        /// </summary>
        /// <param name="deleteModel">The delete model.</param>
        /// <returns>Either a 200 response with no content or a 500 response detailing why the delete
        /// action failed.</returns>
        [HttpPost]
        public IHttpActionResult DeleteUser(UserDeleteViewModel deleteModel)
        {
            try
            {
                if (!deleteModel.IsDeleting) return Ok();
                var result = _repo.DeleteUser(deleteModel.Guid).Result;
                return result.Succeeded ? Ok() : GetErrorResult(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }

        /// <summary>
        /// Alters the account login details.
        /// </summary>
        /// <remarks>
        /// Alters a given account's log in details such as user name, password or email.
        /// </remarks>
        /// <param name="editViewModel">The edit view model.</param>
        /// <returns>A 200 response or an error result detailing why the edit details function failed</returns>
        [HttpPost]
        public async Task<IHttpActionResult> AlterAccountLoginDetails(UserEditViewModel editViewModel)
        {
            try
            {
                var result = await _repo.EditUser(new RegisterViewModel()
                {
                    UserName = editViewModel.UserName,
                    Email = editViewModel.EmailAddress,
                    Password = editViewModel.Password,
                    ConfirmPassword = editViewModel.ConfirmPassword
                }, editViewModel.Guid, editViewModel.PhoneNumber);

                var errorResult = GetErrorResult(result);

                return errorResult ?? Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e.ToString());
            }
        }

        /// <summary>
        /// Clones the user.
        /// </summary>
        /// <remarks>
        /// Effectively clones a users permissions and roles and gives them to a new user.
        /// If the old user does not exist, or has no roles, the new user will be created regardless.
        /// </remarks>
        /// <param name="cloneUserViewModel">The clone user view model.</param>
        /// <returns>A 200 response or an error result detailing why the clone user function failed</returns>
        [HttpPost]
        public IHttpActionResult CloneUser(CloneUserViewModel cloneUserViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = _repo.CloneUser(cloneUserViewModel);

            var errorResult = GetErrorResult(result);

            return errorResult ?? Ok();
        }

        /// <summary>
        /// Changes the type of the user.
        /// </summary>
        /// <remarks>
        /// Changes the user type between Administrator and user in an alternating fashion
        /// </remarks>
        /// <param name="typeViewModel">The type view model.</param>
        /// <returns>A 200 response or an error result detailing why the change user role function failed</returns>
        [HttpPost]
        public IHttpActionResult ChangeUserType(UserTypeViewModel typeViewModel)
        {
            try
            {
                if (_repo.FindUserByGuid(typeViewModel.Guid).Roles.Any
                    (x => x.RoleId == WebConfigurationManager.AppSettings["SuperAdministratorRole"]))
                    return BadRequest("User is not eligable for Role Swap");
                if (typeViewModel.IsAdmin)
                {
                    _repo.SwapUserRoles(typeViewModel.Guid, "Administrator", "User");
                    return Ok();
                }
                _repo.SwapUserRoles(typeViewModel.Guid, "User", "Administrator");
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Releases the unmanaged resources that are used by the object and, optionally, releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _repo.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets the error result.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>A server response that contains the unpacked reasons why an identity transaction failed.</returns>
        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (result.Succeeded) return null;
            if (result.Errors != null)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }

            if (ModelState.IsValid)
            {
                // No ModelState errors are available to send, so just return an empty BadRequest.
                return BadRequest();
            }

            return BadRequest(ModelState);

        }

        /// <summary>
        /// Converts to web view model.
        /// </summary>
        /// <remarks>
        /// Converts a list of identity users into a list of QueueUserWebViewModels in order to
        /// send back to the front end
        /// </remarks>
        /// <param name="pagedQuotes">The paged quotes.</param>
        /// <param name="returnAll">if set to <c>true</c> [return all].</param>
        /// <returns>A list of QueueUserWebViewModels</returns>
        private List<QueueUserWebViewModel> ConvertToWebViewModel(List<IdentityUser> pagedQuotes, bool returnAll)
        {
            pagedQuotes = !returnAll ?
                pagedQuotes.Where(x => x.Roles.Any(y => y.RoleId == WebConfigurationManager.AppSettings["UserRole"])).ToList() :
                pagedQuotes.Where(x => x.Roles.Any(y => y.RoleId == WebConfigurationManager.AppSettings["AdministratorRole"])).ToList();

            var returnItems = pagedQuotes.Select(x => new QueueUserWebViewModel()
            {
                Guid = x.Id,
                UserName = x.UserName,
                AccountLocked = !x.LockoutEndDateUtc.HasValue,
                EmailAddress = x.Email,
                PhoneNumber = string.IsNullOrEmpty(x.PhoneNumber) ? "00000 000000" : x.PhoneNumber,
                EmailConfirmed = x.EmailConfirmed,
                IsAdmin = x.Roles.Any(y => y.RoleId == WebConfigurationManager.AppSettings["AdministratorRole"] || y.RoleId == WebConfigurationManager.AppSettings["SuperAdministratorRole"])
            }).ToList();

            return returnItems;
        }

        /// <summary>
        /// Determines if string one contains string two as a sub string
        /// </summary>
        /// <param name="str1">The string that is being searched</param>
        /// <param name="str2">The string that is being searched for</param>
        /// <returns>A bool which indicates if string one contains string two</returns>
        private bool TestFunction(string str1, string str2)
        {
            return str1 != null && str1.ToUpper().Contains(str2.ToUpper());
        }
    }
}
