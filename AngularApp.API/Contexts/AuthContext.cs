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
    public class AuthContext : IdentityDbContext<IdentityUser>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthContext"/> class.
        /// </summary>
        public AuthContext(): base("AngularUsers")
        {

        }
    }
}