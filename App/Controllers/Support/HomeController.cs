using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shaml.Core;
using CodeChirp.Core;
using CodeChirp.ApplicationServices;
using Shaml.Web;
using Shaml.Core.PersistenceSupport.NHibernate;

namespace CodeChirp.Controllers
{
    [HandleError]
    [GenericLogger]
    public class HomeController : Controller
    {
        private INHibernateQueryRepository<Post> postRepository;

        public HomeController(INHibernateQueryRepository<Post> postRepository)
        {
            this.postRepository = postRepository;
        }

        public ActionResult Index(int? page)
        {
            int p = 0;
            if (page.HasValue && page.Value>0)
            {
                p = page.Value;
            }
			Response.AppendHeader("X-XRDS-Location", new Uri(Request.Url, Response.ApplyAppPathModifier("~/OpenId/XRDS")).AbsoluteUri);
            IList<Post> post = postRepository.FindByQuery("from Post p left join fetch p.parent left join fetch p.user order by p.lastedit desc",40,p);
            ViewData["page"] = p+1;
            return View(post);
        }
    }
}
