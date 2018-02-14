using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using AngularApp.API.Contexts;
using AngularApp.API.Models.WebViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using WebGrease.Css.Extensions;

namespace AngularApp.API.Repository
{
    public class AuthRepository : IDisposable
    {
        private AuthContext _ctx;

        private UserManager<IdentityUser> _userManager;

        public AuthRepository()
        {
            _ctx = new AuthContext();
            _userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(_ctx));
        }

        public async Task<IdentityResult> RegisterUser(RegisterViewModel userModel)
        {

            var currGuid = Guid.NewGuid().ToString();
            var user = new IdentityUser
            {
                Id = currGuid,
                UserName = userModel.UserName,
                Roles =
                {
                    new IdentityUserRole()
                    {
                        RoleId = (userModel.IsAdministrator) ? WebConfigurationManager.AppSettings["AdministratorRole"] : WebConfigurationManager.AppSettings["UserRole"],
                        UserId = currGuid
                    }
                },
                Email = userModel.Email
            };

            var result = await _userManager.CreateAsync(user, userModel.Password);

            return result;
        }

        public async Task<IdentityUser> FindUser(string userName, string password)
        {
            var user = await _userManager.FindAsync(userName, password);
            return user;
        }

        public List<IdentityUser> GetAllUsers()
        {
            var users = _userManager.Users.ToList();
            return users;
        }

        public void Dispose()
        {
            _ctx.Dispose();
            _userManager.Dispose();

        }
    }
}