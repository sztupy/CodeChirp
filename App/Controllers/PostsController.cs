using System;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;

using NHibernate.Validator.Engine;

using Shaml.Web;
using Shaml.Web.CommonValidator;
using Shaml.Web.NHibernate;
using Shaml.Web.HtmlHelpers;
using Shaml.Core;
using Shaml.Core.PersistenceSupport;
using Shaml.Core.DomainModel;
using Shaml.Core.PersistenceSupport.NHibernate;

using CodeChirp.Core;
using Shaml.Web.JsonNet;

namespace CodeChirp.Controllers
{
    [HandleError]
    [GenericLogger]
    public class PostsController : Controller
    {
        public PostsController(INHibernateQueryRepository<Post> PostRepository) {
            Check.Require(PostRepository != null, "PostRepository may not be null");

            this.PostRepository = PostRepository;
        }

        public ActionResult Index(int? Page, string type, bool? Desc) {
            int page = 0;
            if (Page != null)
            {
                page = (int)Page;
            }
            IList<Post> Posts = PostRepository.FindByQuery("from Post p left join fetch p.parent left join fetch p.user order by p.lastedit desc", 40, page);
            ViewData["page"] = page+1;
            if (type == "json")
            {
                return new JsonNetResult(Posts);
            } else
            {
                return View(Posts);
            }
        }

        public ActionResult Show(int id, string type) {
            Post Post = PostRepository.Get(id);
            if (type == "html")
            {
                return PartialView("MaxiPost", Post);
            }
            else if (type == "json")
            {
                return new JsonNetResult(Post);
            } else
            {
                return View(Post);
            }
        }

        private readonly INHibernateQueryRepository<Post> PostRepository;
    }
}

