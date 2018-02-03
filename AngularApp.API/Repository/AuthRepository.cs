using System;
using System.Threading.Tasks;
using AngularApp.API.Contexts;
using AngularApp.API.Models.WebViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AngularApp.API.Repository
{
    public class AuthRepository : IDisposable
    {
        private AuthContext _ctx;

        private UserManager<IdentityUser> _UserManager;

        public AuthRepository()
        {
            _ctx = new AuthContext();
            _UserManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(_ctx));
        }

        public async Task<IdentityResult> RegisterUser(RegisterViewModel UserModel)
        {
            var User = new IdentityUser
            {
                UserName = UserModel.Email
            };

            var result = await _UserManager.CreateAsync(User, UserModel.Password);

            return result;
        }

        public async Task<IdentityUser> FindUser(string UserName, string password)
        {
            var User = await _UserManager.FindAsync(UserName, password);

            return User;
        }

        public void Dispose()
        {
            _ctx.Dispose();
            _UserManager.Dispose();

        }
    }
}