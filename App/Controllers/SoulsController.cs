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
    public class SoulsController : Controller
    {
        public SoulsController(INHibernateQueryRepository<Soul> SoulRepository, INHibernateQueryRepository<Post> PostRepository) {
            Check.Require(SoulRepository != null, "SoulRepository may not be null");

            this.SoulRepository = SoulRepository;
            this.PostRepository = PostRepository;
        }

        public ActionResult Index(int? Page, bool? Desc, string type, string q) {
            int page = 0;
            long numResults;
            if (Page.HasValue && Page.Value>=0)
            {
                page = Page.Value;
            }
            IList<Soul> Souls = null;
            if (q != null)
            {
                var eb = SoulRepository.CreateExpressionBuilder();
                IExpression exp = eb.Like("name", "%" + q + "%", true);
                Souls = SoulRepository.FindByExpression(exp, 54, page, out numResults, SoulRepository.CreateOrder("name", Desc == true));
            }
            else
            {
                Souls = SoulRepository.GetAll(54, page, out numResults, SoulRepository.CreateOrder("name", Desc == true));
            }
            PaginationData pd = new ThreeWayPaginationData(page, 54, numResults);
            ViewData["Pagination"] = pd;
            if (type == "json")
            {
                return new JsonNetResult(Souls);
            }
            else
            {
                return View(Souls);
            }
        }

        public IList<Post> GetPostsForSoul(int id, int Page, int PageSize)
        {
            var eb = PostRepository.CreateExpressionBuilder();
            return PostRepository.FindByQuery("from Post p left join fetch p.user u left join fetch p.tags where u.Id = " + id + " order by lastedit desc",PageSize,Page);
        }

        public ActionResult Show(int id, int? Page, string type) {
            Soul s = SoulRepository.Get(id);
            if (s == null)
            {
                throw new HttpException(404, "HTTP/1.1 404 Not Found");
            }
            int page = 0;
            if (Page.HasValue && Page.Value>0) {
                page = Page.Value;
            }
            IList<Post> p = GetPostsForSoul(id, page,54);
            ViewData["page"] = page + 1;
            ViewData["soul"] = s;
            if (type == "json")
            {
                return new JsonNetResult(new { soul = s, data = p });
            }
            else
            {
                return View(p);
            }
        }

        private readonly INHibernateQueryRepository<Soul> SoulRepository;
        private readonly INHibernateQueryRepository<Post> PostRepository;
    }
}

