using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Security;
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
        private RoleStore<IdentityRole> _roleStore;
        private RoleManager<IdentityRole> _roleMngr;

        public AuthRepository()
        {
            _ctx = new AuthContext();
            _userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(_ctx));
            _roleStore = new RoleStore<IdentityRole>(_ctx);
            _roleMngr = new RoleManager<IdentityRole>(_roleStore);
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

        public IdentityResult CloneUser(UserEditViewModel cloneUserViewModel)
        {
            if (string.IsNullOrEmpty(cloneUserViewModel.Password))
            {
                return new IdentityResult(new List<string>{"No Password for Clone User!"});
            }
            if (string.IsNullOrEmpty(cloneUserViewModel.UserName))
            {
                return new IdentityResult(new List<string> { "No UserName for Clone User!" });
            }
            var currGuid = Guid.NewGuid().ToString();
            var oldUser = _userManager.Users.FirstOrDefault(x => x.Id == cloneUserViewModel.Guid);
            var user = new IdentityUser
            {
                Id = currGuid,
                UserName = cloneUserViewModel.UserName,
                Email = (string.IsNullOrEmpty(cloneUserViewModel.UserName)) ? "" : cloneUserViewModel.EmailAddress,
            };

            if (oldUser?.Roles != null)
                foreach (var role in oldUser.Roles)
                {
                    user.Roles.Add(new IdentityUserRole()
                    {
                        UserId = currGuid,
                        RoleId = role.RoleId
                    });
                }

            var result = _userManager.Create(user, cloneUserViewModel.Password);

            return result;
        }

        public async Task<IdentityResult> EditUser(RegisterViewModel userModel, string userId, string phoneNumber)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.Id == userId);
            IdentityResult result;
            if (!string.IsNullOrWhiteSpace(userModel.Password))
            {
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(userId);
                result = await _userManager.ChangePasswordAsync(userId, resetToken, userModel.Password);
            }
            if (user == null) return new IdentityResult(new List<string> {"Unable to Find User"});
            if (!string.IsNullOrWhiteSpace(userModel.Email))
                user.Email = userModel.Email;
            if (!string.IsNullOrWhiteSpace(userModel.UserName))
                user.UserName = userModel.UserName;
            if (!string.IsNullOrWhiteSpace(phoneNumber))
                user.PhoneNumber = phoneNumber;
            result = await _userManager.UpdateAsync(user);
            return result;
        }

        public async Task<IdentityUser> FindUser(string userName, string password)
        {
            var user = await _userManager.FindAsync(userName, password);
            return user;
        }

        public List<IdentityRole> ReturnAllRoles()
        {
            return _roleMngr.Roles.ToList();
        }

        public IdentityUser FindUserByGuid(string guid)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.Id == guid);
            return user;
        }

        public List<IdentityUser> GetAllUsers()
        {
            var users = _userManager.Users.ToList();
            return users;
        }

        public async Task<IdentityResult> DeleteUser(string userId)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.Id == userId);
            var result = await _userManager.DeleteAsync(user);
            return result;
        }

        public bool SwapUserRoles(string userId, string oldRole, string newRole)
        {
            try
            {
                _userManager.RemoveFromRole(userId, oldRole);
                _userManager.AddToRole(userId, newRole);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void Dispose()
        {
            _ctx.Dispose();
            _userManager.Dispose();

        }
    }
}