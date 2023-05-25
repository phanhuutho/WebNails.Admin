using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace WebNails.Admin
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            if (FormsAuthentication.CookiesSupported == true)
            {
                if (Request.Cookies[FormsAuthentication.FormsCookieName] == null)
                {
                    FormsAuthentication.SignOut();
                    Context.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
                }
                else
                {
                    var typeOf = Context.User.Identity.GetType();
                    if (typeOf == typeof(FormsIdentity))
                    {
                        var identity = (FormsIdentity)Context.User.Identity;
                        var roles = identity.Ticket.UserData;
                        Context.User = new GenericPrincipal(identity, roles.Split(','));
                    }
                }
            }
        }
    }
}
