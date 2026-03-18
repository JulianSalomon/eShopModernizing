using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;


namespace eShopModernizedMVC.Filters
{
    public class ActionTracerFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
                Trace.TraceInformation($"Received request for action {((ControllerActionDescriptor)filterContext.ActionDescriptor).ActionName} in controller {filterContext.Controller.GetType().Name}.");
                base.OnActionExecuting(filterContext);
        }
    }
}
