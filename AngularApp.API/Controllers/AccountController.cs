using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.Cors;
using AngularApp.API.Models.DBModels;
using AngularApp.API.Models.WebViewModels.UserActionModels;
using AngularApp.API.Models.WebViewModels.AccountViewModels;
using AngularApp.API.Models.WebViewModels.PagingModels;
using AngularApp.API.Models.WebViewModels.UserQueueDisplayModels;
using AngularApp.API.Repository;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AngularApp.API.Controllers
{
    //[RoutePrefix("api/Account")]
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    [EnableCors("*", "*", "*")]
    public class AccountController : ApiController
    {
        /// <summary>
        /// The Authentication Repository
        /// </summary>
        private readonly AuthRepository _repo = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        public AccountController()
        {
            _repo = new AuthRepository();
        }

        /// <summary>
        /// Logins the specified login view model.
        /// </summary>
        /// <param name="loginViewModel">The login view model.</param>
        /// <returns></returns>
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

                return Ok(new
                {
                    FullName = returnData?.FullName.Trim(),
                    result.UserName,
                    IsSuperAdmin = result.Roles.Any(y => y.RoleId == WebConfigurationManager.AppSettings["SuperAdministratorRole"]),
                    IsAdmin = result.Roles.Any(y => y.RoleId == WebConfigurationManager.AppSettings["AdministratorRole"])
                });
            }
            else
            {
                return BadRequest();
            }
        }

        // POST api/Account/Register
        /// <summary>
        /// Registers the specified user model.
        /// </summary>
        /// <param name="userModel">The user model.</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IHttpActionResult> Register(RegisterViewModel userModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _repo.RegisterUser(userModel);

            var errorResult = GetErrorResult(result);

            return errorResult ?? Ok();
        }

        /// <summary>
        /// Returns all users.
        /// </summary>
        /// <param name="parameterWebView">The parameter web view.</param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult ReturnAllUsers(AccountPagingParameterWebViewModel parameterWebView)
        {
            var users = _repo.GetAllUsers();

            if (!string.IsNullOrWhiteSpace(parameterWebView.OrderBy))
            {
                if (parameterWebView.OrderBy.Contains("-"))
                {
                    users.OrderByDescending(x => x.GetType().GetProperty(parameterWebView.OrderBy.Split('-')[1]));
                }
                else
                {
                    users.OrderBy(x => x.GetType().GetProperty(parameterWebView.OrderBy));
                }
            }
            var pagedQuotes = !parameterWebView.ReturnAll ?
                users.Where(x => x.Roles.Any(y => y.RoleId == WebConfigurationManager.AppSettings["UserRole"])).ToList() :
                users.Where(x => x.Roles.Any(y => y.RoleId == WebConfigurationManager.AppSettings["AdministratorRole"])).ToList();

            var totalPages = (int)Math.Ceiling(pagedQuotes.Count() / (double)parameterWebView.PageSize);
            pagedQuotes = pagedQuotes.Skip((parameterWebView.PageNumber - 1) * parameterWebView.PageSize).Take(parameterWebView.PageSize).ToList();

            var returnItems = pagedQuotes.Select(x => new QueueUserWebViewModel()
            {
                Guid = x.Id,
                UserName = x.UserName,
                AccountLocked = !x.LockoutEndDateUtc.HasValue,
                EmailAddress = x.Email,
                PhoneNumber = string.IsNullOrEmpty(x.PhoneNumber) ? "00000 000000" : x.PhoneNumber,
                EmailConfirmed = x.EmailConfirmed,
                IsAdmin = x.Roles.Any(y => y.RoleId == WebConfigurationManager.AppSettings["AdministratorRole"] || y.RoleId == WebConfigurationManager.AppSettings["SuperAdministratorRole"])
            });
            return Ok(new PaginatedQueueUserResult()
            {
                QueueDisplay = returnItems,
                TotalPages = totalPages,
                TotalItems = pagedQuotes.Count
            });
        }

        /// <summary>
        /// Returns all users at level for search.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult ReturnAllUsersAtLevelForSearch(SearchAccounthWebViewModel viewModel)
        {
            var users = _repo.GetAllUsers();
            var returnItems = ConvertToWebViewModel(users, viewModel.ReturnAll);
            var returnItem = returnItems.Where(x => TestFunction(x.UserName, viewModel.FilterTerm) || TestFunction(x.EmailAddress, viewModel.FilterTerm) ||
                                                    TestFunction(x.Guid, viewModel.FilterTerm) || TestFunction(x.PhoneNumber, viewModel.FilterTerm)).ToList();
            return Ok(returnItem);
        }

        /// <summary>
        /// Deletes the user.
        /// </summary>
        /// <param name="deleteModel">The delete model.</param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult DeleteUser(UserDeleteViewModel deleteModel)
        {
            if (!deleteModel.IsDeleting) return Ok();
            var result = _repo.DeleteUser(deleteModel.Guid).Result;
            return result.Succeeded ? Ok() : GetErrorResult(result);
        }

        /// <summary>
        /// Alters the account login details.
        /// </summary>
        /// <param name="editViewModel">The edit view model.</param>
        /// <returns></returns>
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
                throw;
            }
        }

        /// <summary>
        /// Clones the user.
        /// </summary>
        /// <param name="cloneUserViewModel">The clone user view model.</param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult CloneUser(UserEditViewModel cloneUserViewModel)
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
        /// <param name="typeViewModel">The type view model.</param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult ChangeUserType(UserTypeViewModel typeViewModel)
        {
            try
            {
                if (typeViewModel.IsAdmin)
                {
                    _repo.SwapUserRoles(typeViewModel.Guid, "Administrator", "User");
                    return Ok();
                }
                else
                {
                    _repo.SwapUserRoles(typeViewModel.Guid, "User", "Administrator");
                    return Ok();
                }
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
        /// <returns></returns>
        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
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

            return null;
        }

        /// <summary>
        /// Converts to web view model.
        /// </summary>
        /// <param name="pagedQuotes">The paged quotes.</param>
        /// <param name="returnAll">if set to <c>true</c> [return all].</param>
        /// <returns></returns>
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
        /// Tests the function.
        /// </summary>
        /// <param name="str1">The STR1.</param>
        /// <param name="str2">The STR2.</param>
        /// <returns></returns>
        private bool TestFunction(string str1, string str2)
        {
            return str1 != null && str1.ToUpper().Contains(str2.ToUpper());
        }
    }
}
