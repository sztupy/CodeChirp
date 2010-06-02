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
    public class TagsController : Controller
    {
        public TagsController(INHibernateQueryRepository<Tag> TagRepository) {
            Check.Require(TagRepository != null, "TagRepository may not be null");

            this.TagRepository = TagRepository;
        }

        public ActionResult Index(int? Page, string OrderBy, bool? Desc) {
            long numResults;
            int page = 0;
            if (Page != null)
            {
                page = (int)Page;
            }
            IList<Tag> Tags = null;
            Tags = TagRepository.GetAll(20, page, out numResults, TagRepository.CreateOrder(OrderBy,Desc==true));
            PaginationData pd = new ThreeWayPaginationData(page, 20, numResults);
            ViewData["Pagination"] = pd;
            return View(Tags);
        }

        public ActionResult Show(int id) {
            Tag Tag = TagRepository.Get(id);
            return View(Tag);
        }

        private readonly INHibernateQueryRepository<Tag> TagRepository;
    }
}

