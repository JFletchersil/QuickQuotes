using System.Web.Http;

namespace AngularApp.API
{
    /// <summary>
    /// The global namespace for the WebApi, it contains everything inside the project
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    internal class NamespaceDoc
    {

    }

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
