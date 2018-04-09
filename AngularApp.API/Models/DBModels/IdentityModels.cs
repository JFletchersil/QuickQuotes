using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AngularApp.API.Models.WebViewModels
{
    /// <summary>
    /// Provides a basic model of an application user within our application
    /// This inherates from the standard asp.net Identity User and otherwise remains untouched
    /// </summary>
    /// <remarks>
    /// This is barely used in any contexts, and can be safely ignored.
    /// </remarks>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Generates a single user identit
        /// </summary>
        /// <param name="manager">a User Manager that is filled with application users</param>
        /// <returns>An async with an identity claim</returns>
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var UserIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom User claims here
            return UserIdentity;
        }
    }

    /// <summary>
    /// Creates an User Database Context for accessing the user database
    /// </summary>
    /// <remarks>
    /// Legacy generated code from the creation of the database, this can be safely ignored.
    /// </remarks>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        /// <summary>
        /// Creates a new context based off the MyConnection connection string
        /// </summary>
        /// <remarks>
        /// This is not used in the project, it can be safely ignored.
        /// </remarks>
        public ApplicationDbContext()
            : base("MyConnection", throwIfV1Schema: false)
        {
        }

        /// <summary>
        /// Creates a ApplicationDBContext and returns it for usage in other application areas
        /// </summary>
        /// <returns>A new applicationdbcontext</returns>
        /// <remarks>
        /// This is not used in the project, it can be safely ignored.
        /// </remarks>
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}