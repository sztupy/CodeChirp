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

        public ActionResult Index(int? Page, string OrderBy, bool? Desc) {
            long numResults;
            int page = 0;
            if (Page != null)
            {
                page = (int)Page;
            }
            IList<Post> Posts = null;
            Posts = PostRepository.GetAll(20, page, out numResults, PostRepository.CreateOrder(OrderBy,Desc==true));
            PaginationData pd = new ThreeWayPaginationData(page, 20, numResults);
            ViewData["Pagination"] = pd;
            return View(Posts);
        }

        public ActionResult Show(int id) {
            Post Post = PostRepository.Get(id);
            return View(Post);
        }

        private readonly INHibernateQueryRepository<Post> PostRepository;
    }
}

