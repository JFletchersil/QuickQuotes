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
        /// Occurs before the action method is invoked.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            //if (IfDisabledLogic(actionContext))
            //{
            //    actionContext.Response = new HttpResponseMessage(HttpStatusCode.NotFound);
            //}
            //else
            //    base.OnActionExecuting(actionContext);
            actionContext.Response = new HttpResponseMessage(HttpStatusCode.NotFound);
        }
    }
}