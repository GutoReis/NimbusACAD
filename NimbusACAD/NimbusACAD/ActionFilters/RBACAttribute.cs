using System;
using System.Web.Mvc;
using System.Web.Routing;

public class RBACAttribute : AuthorizeAttribute
{
    public override void OnAuthorization(AuthorizationContext filterContext)
    {
        try
        {
            if (!filterContext.HttpContext.Request.IsAuthenticated)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(
                    new { controller = "Account", action = "Login", returnUrl = filterContext.HttpContext.Request.FilePath }));
            }
            else
            {
                string requiredPermission = String.Format("{0}-{1}", filterContext.ActionDescriptor.ControllerDescriptor.ControllerName, filterContext.ActionDescriptor.ActionName);
            }
        }
        catch (Exception ex)
        {
            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Desautorizado", action = "Erro", _erroMsg = ex.Message }));
        }
    }
}