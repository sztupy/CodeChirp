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
    public class BadgesController : Controller
    {
        public BadgesController(INHibernateQueryRepository<Badge> BadgeRepository) {
            Check.Require(BadgeRepository != null, "BadgeRepository may not be null");

            this.BadgeRepository = BadgeRepository;
        }

        public ActionResult Index(int? Page, string OrderBy, bool? Desc) {
            long numResults;
            int page = 0;
            if (Page != null)
            {
                page = (int)Page;
            }
            IList<Badge> Badges = null;
            Badges = BadgeRepository.GetAll(20, page, out numResults, BadgeRepository.CreateOrder(OrderBy,Desc==true));
            PaginationData pd = new ThreeWayPaginationData(page, 20, numResults);
            ViewData["Pagination"] = pd;
            return View(Badges);
        }

        public ActionResult Show(int id) {
            Badge Badge = BadgeRepository.Get(id);
            return View(Badge);
        }

        private readonly INHibernateQueryRepository<Badge> BadgeRepository;
    }
}

