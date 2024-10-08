﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Configuration;
using System.Net;

namespace IOAS
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
            var authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                if (authTicket != null)
                {
                    var roles = authTicket.UserData.Split(',');
                    HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(new FormsIdentity(authTicket), roles);
                }
            }
        }
        void Application_BeginRequest(object sender, EventArgs e)
        {
           
            if (ConfigurationManager.AppSettings["MaintenanceMode"] == "true")
            {
                if (!Request.IsLocal)
                {
                    Response.Redirect("maintenance.html");
                }
            }
        }
        void Application_EndRequest(object sender, System.EventArgs e)
        {
            // If the user is not authorized to see this page or access this function, send them to the error page.
            if (Response.StatusCode == 403)
            {
                Response.ClearContent();
                Response.RedirectToRoute("Default", new { controller = "Error", action = "AccessDenied" });
            }
        }
        //void Session_Start(object sender, EventArgs e)
        //{
        //    if (HttpContext.Current.Request.IsAuthenticated)
        //    {
        //        string userName = "";
        //        if (Session["UserName"] != null)
        //            userName = Session["UserName"].ToString();

        //        //var logOff = AccountService.setLogoff(userName);

        //        //old authentication, kill it
        //        FormsAuthentication.SignOut();
        //        //or use Response.Redirect to go to a different page
        //        //FormsAuthentication.RedirectToLoginPage("Session=Expired");
        //        Response.Redirect(("/Account/Login"));
        //        HttpContext.Current.Response.End();
        //    }

        //}

    }
}
