using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.Cors;
using AngularApp.API.Models.DBModels;
using AngularApp.API.Models.WebViewModels;
using AngularApp.API.Repository;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AngularApp.API.Controllers
{
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
        private readonly AuthRepository _repo = null;

        /// <summary>
        /// Provides an initalisation for the AuthRepository, allowing you to communicate with the user database.
        /// </summary>
        public AccountController()
        {
            _repo = new AuthRepository();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginViewModel"></param>
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

        [HttpPost]
        public IHttpActionResult ReturnAllUsersAtLevelForSearch(SearchAccounthWebViewModel viewModel)
        {
            var users = _repo.GetAllUsers();
            var returnItems = ConvertToWebViewModel(users, viewModel.ReturnAll);
            var returnItem = returnItems.Where(x => TestFunction(x.UserName, viewModel.FilterTerm) || TestFunction(x.EmailAddress, viewModel.FilterTerm) ||
                                                    TestFunction(x.Guid, viewModel.FilterTerm) || TestFunction(x.PhoneNumber, viewModel.FilterTerm)).ToList();
            return Ok(returnItem);
        }

        [HttpPost]
        public IHttpActionResult DeleteUser(UserDeleteViewModel deleteModel)
        {
            if (!deleteModel.IsDeleting) return Ok();
            var result = _repo.DeleteUser(deleteModel.Guid).Result;
            return result.Succeeded ? Ok() : GetErrorResult(result);
        }

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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _repo.Dispose();
            }

            base.Dispose(disposing);
        }

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

        private bool TestFunction(string str1, string str2)
        {
            return str1 != null && str1.ToUpper().Contains(str2.ToUpper());
        }
    }
}
