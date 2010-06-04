using System;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using System.Linq;

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
    public class BadgesController : Controller
    {
        public BadgesController(INHibernateQueryRepository<Badge> BadgeRepository, INHibernateQueryRepository<Soul> SoulRepository, INHibernateQueryRepository<Post> PostRepository)
        {
            Check.Require(BadgeRepository != null, "BadgeRepository may not be null");

            this.BadgeRepository = BadgeRepository;
            this.SoulRepository = SoulRepository;
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
            IList<Badge> Badges = null;
            if (q != null)
            {
                var eb = BadgeRepository.CreateExpressionBuilder();
                IExpression exp = eb.Like("name", "%" + q + "%", true);
                Badges = BadgeRepository.FindByExpression(exp, 40, page, out numResults, BadgeRepository.CreateOrder("name", Desc == true));
            }
            else
            {
                Badges = BadgeRepository.GetAll(40, page, out numResults, BadgeRepository.CreateOrder("name", Desc == true));
            }
            PaginationData pd = new ThreeWayPaginationData(page, 40, numResults);
            ViewData["Pagination"] = pd;
            if (type == "json")
            {
                return new JsonNetResult(Badges);
            }
            else
            {
                return View(Badges);
            }
        }

        public IList<Post> GetPostsForBadge(Badge b, int Page)
        {
            SoulsController sc = new SoulsController(SoulRepository, PostRepository,null,null);
            List<Post> p = new List<Post>();
            foreach (var u in b.users)
            {
                p.AddRange(sc.GetPostsForSoul(u.Id,Page,10));
            }
            return p.OrderBy(x => x.lastedit).Reverse().ToList();
        }

        public ActionResult Show(int id, int? Page, string type)
        {
            Badge b = BadgeRepository.Get(id);
            if (b == null)
            {
                throw new HttpException(404, "HTTP/1.1 404 Not Found");
            }
            int page = 0;
            if (Page.HasValue && Page.Value > 0)
            {
                page = Page.Value;
            }
            IList<Post> p = GetPostsForBadge(b, page);
            ViewData["page"] = page + 1;
            ViewData["badge"] = b;
            if (type == "json")
            {
                return new JsonNetResult(new { badge = b, data = p });
            }
            else
            {
                return View(p);
            }
        }

        private readonly INHibernateQueryRepository<Soul> SoulRepository;
        private readonly INHibernateQueryRepository<Badge> BadgeRepository;
        private readonly INHibernateQueryRepository<Post> PostRepository;
    }
}

