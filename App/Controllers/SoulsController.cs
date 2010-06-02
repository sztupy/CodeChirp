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
    public class SoulsController : Controller
    {
        public SoulsController(INHibernateQueryRepository<Soul> SoulRepository) {
            Check.Require(SoulRepository != null, "SoulRepository may not be null");

            this.SoulRepository = SoulRepository;
        }

        public ActionResult Index(int? Page, string OrderBy, bool? Desc) {
            long numResults;
            int page = 0;
            if (Page != null)
            {
                page = (int)Page;
            }
            IList<Soul> Souls = null;
            Souls = SoulRepository.GetAll(20, page, out numResults, SoulRepository.CreateOrder(OrderBy,Desc==true));
            PaginationData pd = new ThreeWayPaginationData(page, 20, numResults);
            ViewData["Pagination"] = pd;
            return View(Souls);
        }

        public ActionResult Show(int id) {
            Soul Soul = SoulRepository.Get(id);
            return View(Soul);
        }

        private readonly INHibernateQueryRepository<Soul> SoulRepository;
    }
}

