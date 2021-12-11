using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IOAS.Controllers
{
    public class ErrorController : Controller
    {
        public ViewResult AccessDenied()
        {
            // Do not set this or else you get a redirect loop
            return View();
            //where View is the friendly .cshtml page
        }
    }
}