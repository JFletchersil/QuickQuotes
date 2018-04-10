using System.Web.Http;

namespace AngularApp.API
{
    /// <summary>
    /// The Startup/Global asax of the Api Application
    /// </summary>
    /// <seealso cref="System.Web.HttpApplication" />
    public class WebApiApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// Applications the start.
        /// </summary>
        protected void Application_Start()
        {
            // Registers the WebApi as valid.
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
