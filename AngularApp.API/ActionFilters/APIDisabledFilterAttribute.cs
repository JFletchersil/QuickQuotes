using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace AngularApp.API.ActionFilters
{
    public class APIDisabledAttribute : ActionFilterAttribute
    {
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