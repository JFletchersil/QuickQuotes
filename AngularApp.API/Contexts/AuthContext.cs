using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AngularApp.Contexts
{
    public class AuthContext : IdentityDbContext<IdentityUser>
    {
        public AuthContext(): base("MyConnection")
        {

        }
    }
}