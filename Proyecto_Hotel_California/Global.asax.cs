using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace Proyecto_Hotel_California
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

        //protected void Application_EndRequest(object sender, EventArgs e)
        //{
        //    // this code will mark the forms authentication cookie and the
        //    // session cookie as Secure.
        //    if (Response.Cookies.Count > 0)
        //    {
        //        foreach (string s in Response.Cookies.AllKeys)
        //        {
        //            if (s == FormsAuthentication.FormsCookieName || s.ToLower() == "asp.net_sessionid")
        //            {
        //                Response.Cookies[s].Secure = true;
        //            }
        //        }
        //    }
        //}
    }
}
