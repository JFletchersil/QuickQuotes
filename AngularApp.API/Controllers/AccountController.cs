using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.Cors;
using AngularApp.API.Models.DBModels;
using AngularApp.API.Models.WebViewModels;
using AngularApp.API.Repository;
using Microsoft.AspNet.Identity;

namespace AngularApp.API.Controllers
{
    //[RoutePrefix("api/Account")]
    [EnableCors("*", "*", "*")]
    public class AccountController : ApiController
    {
        private readonly AuthRepository _repo = null;

        public AccountController()
        {
            _repo = new AuthRepository();
        }

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
        public IHttpActionResult ReturnAllUsers(PagingParameterWebViewModel parameterWebView)
        {
            var users = _repo.GetAllUsers();

            var totalPages = (int)Math.Ceiling(users.Count() / (double)parameterWebView.PageSize);
            var pagedQuotes = users.Skip((parameterWebView.PageNumber - 1) * parameterWebView.PageSize).Take(parameterWebView.PageSize).ToList();

            pagedQuotes = !parameterWebView.ReturnAll ? 
                pagedQuotes.Where(x => x.Roles.Any(y => y.RoleId == WebConfigurationManager.AppSettings["UserRole"])).ToList() :
                pagedQuotes.Where(x => x.Roles.Any(y => y.RoleId == WebConfigurationManager.AppSettings["AdministratorRole"])).ToList();
                

            var returnItems = pagedQuotes.Select(x => new QueueUserWebViewModel()
            {
                UserName = x.UserName,
                AccountLocked = !x.LockoutEndDateUtc.HasValue,
                EmailAddress = x.Email,
                HasTwoFactor = x.TwoFactorEnabled,
                IsAdmin = x.Roles.Any(y => y.RoleId == WebConfigurationManager.AppSettings["AdministratorRole"] || y.RoleId == WebConfigurationManager.AppSettings["SuperAdministratorRole"])
            });
            return Ok(new PaginatedQueueUserResult()
            {
                QueueDisplay = returnItems,
                TotalPages = totalPages,
                TotalItems = users.Count
            });
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
    }
}
