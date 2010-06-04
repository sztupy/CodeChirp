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
using System.Web;

namespace CodeChirp.Controllers
{
    [HandleError]
    [GenericLogger]
    public class TagsController : Controller
    {
        public TagsController(INHibernateQueryRepository<Tag> TagRepository, INHibernateQueryRepository<Post> PostRepository) {
            Check.Require(TagRepository != null, "TagRepository may not be null");

            this.TagRepository = TagRepository;
            this.PostRepository = PostRepository;
        }

        public ActionResult Index(int? Page, bool? Desc, string type, string q)
        {
            int page = 0;
            long numResults;
            if (Page.HasValue && Page.Value >= 0)
            {
                page = Page.Value;
            }
            IList<Tag> Tags = null;
            if (q != null)
            {
                var eb = TagRepository.CreateExpressionBuilder();
                IExpression exp = eb.Like("name", "%" + q + "%", true);
                Tags = TagRepository.FindByExpression(exp, 40, page, out numResults, TagRepository.CreateOrder("name", Desc == true));
            }
            else
            {
                Tags = TagRepository.GetAll(40, page, out numResults, TagRepository.CreateOrder("name", Desc == true));
            }
            PaginationData pd = new ThreeWayPaginationData(page, 40, numResults);
            ViewData["Pagination"] = pd;
            if (type == "json")
            {
                return new JsonNetResult(Tags);
            }
            else
            {
                return View(Tags);
            }
        }

        public IList<Post> GetPostsForTag(int id, int Page)
        {
            return PostRepository.FindByQuery("select p from Post p left join fetch p.user left join fetch p.parent left join p.tags t where t.Id = " + id + " order by p.lastedit desc",40,Page);
        }

        public ActionResult Show(int id, int? Page, string type)
        {
            Tag t = TagRepository.Get(id);
            if (t == null)
            {
                throw new HttpException(404, "HTTP/1.1 404 Not Found");
            }
            int page = 0;
            if (Page.HasValue && Page.Value > 0)
            {
                page = Page.Value;
            }
            IList<Post> p = GetPostsForTag(id, page);
            ViewData["page"] = page + 1;
            ViewData["tag"] = t;
            if (type == "json")
            {
                return new JsonNetResult(new { tag = t, data = p });
            }
            else
            {
                return View(p);
            }
        }

        private readonly INHibernateQueryRepository<Tag> TagRepository;
        private readonly INHibernateQueryRepository<Post> PostRepository;
    }
}

