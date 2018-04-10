using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using AngularApp.API.Contexts;
using AngularApp.API.Models.WebViewModels.UserActionModels;
using AngularApp.API.Models.WebViewModels.AccountViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AngularApp.API.Repository
{
    /// <summary>
    /// A collection of repositories that allow the user to work with the various databases that are
    /// associated with the project
    /// </summary>
    /// <remarks>
    /// At present, there is only one repository that is used.
    /// This repository is the Auth Repository
    /// </remarks>
    [System.Runtime.CompilerServices.CompilerGenerated]
    internal class NamespaceDoc
    {

    }

    /// <summary>
    /// A repository for handling Authentication and User Management
    /// </summary>
    /// <seealso cref="IDisposable" />
    public class AuthRepository : IDisposable
    {
        /// <summary>
        /// The Authorization Context that the Repository uses
        /// </summary>
        private AuthContext _ctx;

        /// <summary>
        /// The user manager
        /// </summary>
        private UserManager<IdentityUser> _userManager;
        /// <summary>
        /// The role store
        /// </summary>
        private RoleStore<IdentityRole> _roleStore;
        /// <summary>
        /// The role manager
        /// </summary>
        private RoleManager<IdentityRole> _roleMngr;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthRepository"/> class.
        /// </summary>
        /// <seealso cref="Controllers.AccountController"/>
        public AuthRepository()
        {
            _ctx = new AuthContext();
            _userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(_ctx));
            _roleStore = new RoleStore<IdentityRole>(_ctx);
            _roleMngr = new RoleManager<IdentityRole>(_roleStore);
        }

        /// <summary>
        /// Registers the user.
        /// </summary>
        /// <remarks>
        /// This allows a user to create their new user account with basic permissions.
        /// It also has an extension to allow an administrator to create a new account with 
        /// administration permissions.
        /// </remarks>
        /// <param name="userModel">The user model.</param>
        /// <returns>If the creation of the user was successful or not</returns>
        /// <seealso cref="Controllers.AccountController"/>
        public async Task<IdentityResult> RegisterUser(RegisterViewModel userModel)
        {
            // Gives the new user an identifier
            var currGuid = Guid.NewGuid().ToString();
            var user = new IdentityUser
            {
                Id = currGuid,
                UserName = userModel.UserName,
                // Gives the user the correct roles, by default this is done in such a fashion
                // as to make user the default role but an administrator can alter this.
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

        /// <summary>
        /// Clones the user.
        /// </summary>
        /// <remarks>
        /// This used to clone a user, with a new user name and email address and password.
        /// Effectively, this makes this method a copy of the roles a user has.
        /// In future, if there are custom roles aside from the admin/user distinction
        /// this method would allow for the easy creation of new users with these
        /// custom roles.
        /// </remarks>
        /// <param name="cloneUserViewModel">The clone user view model.</param>
        /// <returns>If the clone of the user was successful or not</returns>
        /// <seealso cref="Controllers.AccountController"/>
        public IdentityResult CloneUser(UserEditViewModel cloneUserViewModel)
        {
            // Validates the user model to ensure that the user has a password and user name
            if (string.IsNullOrEmpty(cloneUserViewModel.Password))
            {
                return new IdentityResult(new List<string> { "No Password for Clone User!" });
            }
            if (string.IsNullOrEmpty(cloneUserViewModel.UserName))
            {
                return new IdentityResult(new List<string> { "No UserName for Clone User!" });
            }

            // Gets a new identifier for the clone of the original user
            var currGuid = Guid.NewGuid().ToString();
            // Gets the old user so that we can copy details from the old user to the clone
            var oldUser = _userManager.Users.FirstOrDefault(x => x.Id == cloneUserViewModel.Guid);
            var user = new IdentityUser
            {
                Id = currGuid,
                UserName = cloneUserViewModel.UserName,
                Email = (string.IsNullOrEmpty(cloneUserViewModel.UserName)) ? "" : cloneUserViewModel.EmailAddress,
            };
            // In a loop, checking to ensure that there are roles, copy
            // the roles that the old user has and add them to the new user.
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

        /// <summary>
        /// Edits the user.
        /// </summary>
        /// <param name="userModel">The user model.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="phoneNumber">The phone number.</param>
        /// <returns>If the edit of the user details was successful or not</returns>
        /// <seealso cref="Controllers.AccountController"/>
        public async Task<IdentityResult> EditUser(RegisterViewModel userModel, string userId, string phoneNumber)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.Id == userId);
            IdentityResult result;
            if (!string.IsNullOrWhiteSpace(userModel.Password))
            {
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(userId);
                result = await _userManager.ChangePasswordAsync(userId, resetToken, userModel.Password);
            }
            // Due to the difficulty of a direct information transfer, successive if statements
            // are used in order to check for values and then copy the new values into the 
            // user before saving the user.
            if (user == null) return new IdentityResult(new List<string> { "Unable to Find User" });
            if (!string.IsNullOrWhiteSpace(userModel.Email))
                user.Email = userModel.Email;
            if (!string.IsNullOrWhiteSpace(userModel.UserName))
                user.UserName = userModel.UserName;
            if (!string.IsNullOrWhiteSpace(phoneNumber))
                user.PhoneNumber = phoneNumber;
            result = await _userManager.UpdateAsync(user);
            return result;
        }

        /// <summary>
        /// Finds a given user.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns>A single user who matches the user name and password</returns>
        /// <seealso cref="Controllers.AccountController"/>
        public async Task<IdentityUser> FindUser(string userName, string password)
        {
            var user = await _userManager.FindAsync(userName, password);
            return user;
        }

        /// <summary>
        /// Returns all roles.
        /// </summary>
        /// <returns>Returns a full list of all roles within the database</returns>
        public List<IdentityRole> ReturnAllRoles()
        {
            return _roleMngr.Roles.ToList();
        }

        /// <summary>
        /// Finds the user by unique identifier.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <returns>Returns a user who matches a given unique identifier</returns>
        public IdentityUser FindUserByGuid(string guid)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.Id == guid);
            return user;
        }

        /// <summary>
        /// Gets all users.
        /// </summary>
        /// <returns>A list of all users currently within the database</returns>
        /// <seealso cref="Controllers.AccountController"/>
        public List<IdentityUser> GetAllUsers()
        {
            var users = _userManager.Users.ToList();
            return users;
        }

        /// <summary>
        /// Deletes the user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>Confirmation if the user was successfully deleted or not</returns>
        /// <seealso cref="Controllers.AccountController"/>
        public async Task<IdentityResult> DeleteUser(string userId)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.Id == userId);
            var result = await _userManager.DeleteAsync(user);
            return result;
        }

        /// <summary>
        /// Swaps the user roles.
        /// </summary>
        /// <remarks>
        /// In context, this swaps a user from administrator role to the user role and back.
        /// But it's not limited to this swap.
        /// </remarks>
        /// <param name="userId">The user identifier.</param>
        /// <param name="oldRole">The old role.</param>
        /// <param name="newRole">The new role.</param>
        /// <returns>A bool representation of if the user had their roles swapped</returns>
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

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _ctx.Dispose();
            _userManager.Dispose();

        }
    }
}