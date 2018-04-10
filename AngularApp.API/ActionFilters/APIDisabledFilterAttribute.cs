using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace AngularApp.API.ActionFilters
{
    /// <summary>
    /// A collection of custom action filters that provide alterations to how the web api operates
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    internal class NamespaceDoc
    {

    }

    /// <summary>
    /// Disables a given controller, preventing the controller from being accessed via the web api
    /// </summary>
    /// <seealso cref="ActionFilterAttribute" />
    public class APIDisabledAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Defines the default behaviour when it comes to the API Disabled Attribute
        /// </summary>
        /// <param name="actionContext">A HTTP Action context, I.E. when you make a call to the API</param>
        /// <remarks>
        /// This define the default behaviour when you make a call to an API with the attribute applied to it
        /// </remarks>
        /// <example>
        /// [APIDisabled]
        /// </example>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            //if (IfDisabledLogic(actionContext))
            //{
            //    actionContext.Response = new HttpResponseMessage(HttpStatusCode.NotFound);
            //}
            //else
            //    base.OnActionExecuting(actionContext);

            // Defines that all responses, when making a HTTP Request to a part of the API, must return 404 code
            actionContext.Response = new HttpResponseMessage(HttpStatusCode.NotFound);
        }
    }
}