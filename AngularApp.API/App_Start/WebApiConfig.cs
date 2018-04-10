using Newtonsoft.Json;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace AngularApp.API
{
    /// <summary>
    /// Configures the WebApi to be used to communicate with the controllers
    /// </summary>
    /// <remarks>
    /// This allows you to configure the basic, as well as alternerate, HTTP Routes to the API controllers
    /// This should not be altered unless you wish to change the default HTTP Rotues to the controllers
    /// JF - 08/04/2018 - This will be adjusted to account for Auth0 implementation when/if it is 
    ///                   integrated into the application
    /// </remarks>
    public static class WebApiConfig
    {
        /// <summary>
        /// Registers this version of the WebApi Config for usage
        /// </summary>
        /// <param name="config">Provides a single HttpConfiguration object to configure the WebApi Settings</param>
        /// <remarks>
        /// This is more or less the standard Api routing, with the enhancement of Cors.
        /// Included here is also the option to directly transfer db view models to the web, but this has been
        /// disabled to increase security.
        /// Also included is the ability to camelcase JSON returns, but this has been disabled to make Json names
        /// more consistent, do not enable unless you wish to break the application.
        /// </remarks>
        public static void Register(HttpConfiguration config)
        {

            // Web API routes
            config.EnableCors();
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            //config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            //JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            //{
            //    Formatting = Newtonsoft.Json.Formatting.Indented,
            //    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            //};
            //jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}
