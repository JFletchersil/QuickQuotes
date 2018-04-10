using Microsoft.AspNet.Identity.EntityFramework;

namespace AngularApp.API.Contexts
{
    /// <summary>
    /// A collection of contexts that allow access to databases
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    internal class NamespaceDoc
    {

    }
    /// <summary>
    /// Creates a connection to the authentication database in order to access the data
    /// </summary>
    /// <seealso cref="IdentityDbContext{IdentityUser}" />

    /// <remarks>
    /// This will be disabled/removed when/if Auth0 integration is done.
    /// </remarks>
    public class AuthContext : IdentityDbContext<IdentityUser>
    {
        /// <summary>
        /// Provides the db connection string name, stored inside the WebConfig, which allows Auth DB access.
        /// </summary>
        /// <remarks>
        /// This does not require any further extension, this acts merely as a way to state the db connection string
        /// for the context.
        /// </remarks>
        public AuthContext(): base("AngularUsers")
        {

        }
    }
}