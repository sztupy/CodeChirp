using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shaml.Core;
using CodeChirp.Core;
using CodeChirp.ApplicationServices;
using Shaml.Web;

namespace CodeChirp.Controllers
{
    [HandleError]
    [GenericLogger]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
			Response.AppendHeader("X-XRDS-Location", new Uri(Request.Url, Response.ApplyAppPathModifier("~/OpenId/XRDS")).AbsoluteUri);
            return View();
        }
    }
}
