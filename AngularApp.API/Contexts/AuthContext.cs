using Microsoft.AspNet.Identity.EntityFramework;

namespace AngularApp.API.Contexts
{
    public class AuthContext : IdentityDbContext<IdentityUser>
    {
        public AuthContext(): base("AngularUsers")
        {

        }
    }
}