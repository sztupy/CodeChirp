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

        [Transaction]
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

        [Transaction]
        public ActionResult Show(int id) {
            Badge Badge = BadgeRepository.Get(id);
            return View(Badge);
        }

        public ActionResult Create() {
            BadgeFormViewModel viewModel = BadgeFormViewModel.CreateBadgeFormViewModel();
            return View(viewModel);
        }

        [ValidateAntiForgeryToken]
        [Transaction]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(Badge Badge) {
            if (ViewData.ModelState.IsValid && Badge.IsValid()) {
                BadgeRepository.SaveOrUpdate(Badge);

                TempData[ControllerEnums.GlobalViewDataProperty.PageMessage.ToString()] = 
					"The Badge was successfully created.";
                return RedirectToAction("Index");
            }

            BadgeFormViewModel viewModel = BadgeFormViewModel.CreateBadgeFormViewModel();
            viewModel.Badge = Badge;
            return View(viewModel);
        }

        [Transaction]
        public ActionResult Edit(int id) {
            BadgeFormViewModel viewModel = BadgeFormViewModel.CreateBadgeFormViewModel();
            viewModel.Badge = BadgeRepository.Get(id);
            return View(viewModel);
        }

        [ValidateAntiForgeryToken]
        [Transaction]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Edit(Badge Badge) {
            Badge BadgeToUpdate = BadgeRepository.Get(Badge.Id);
            TransferFormValuesTo(BadgeToUpdate, Badge);

            if (ViewData.ModelState.IsValid && Badge.IsValid()) {
                TempData[ControllerEnums.GlobalViewDataProperty.PageMessage.ToString()] = 
					"The Badge was successfully updated.";
                return RedirectToAction("Index");
            }
            else {
                BadgeRepository.DbContext.RollbackTransaction();

				BadgeFormViewModel viewModel = BadgeFormViewModel.CreateBadgeFormViewModel();
				viewModel.Badge = Badge;
				return View(viewModel);
            }
        }

        private void TransferFormValuesTo(Badge BadgeToUpdate, Badge BadgeFromForm) {
            BadgeToUpdate.name = BadgeFromForm.name;
            BadgeToUpdate.count = BadgeFromForm.count;
            BadgeToUpdate.rank = BadgeFromForm.rank;
            BadgeToUpdate.description = BadgeFromForm.description;
        }


        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Delete(int id)
        {
            Badge BadgeToDelete = BadgeRepository.Get(id);
            return View(BadgeToDelete);
        }

        [ValidateAntiForgeryToken]
        [Transaction]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeleteConfirmed(int id) {
            string resultMessage = "The Badge was successfully deleted.";
            Badge BadgeToDelete = BadgeRepository.Get(id);

            if (BadgeToDelete != null) {
                BadgeRepository.Delete(BadgeToDelete);

                try {
                    BadgeRepository.DbContext.CommitChanges();
                }
                catch {
                    resultMessage = "A problem was encountered preventing the Badge from being deleted. " +
						"Another item likely depends on this Badge.";
                    BadgeRepository.DbContext.RollbackTransaction();
                }
            }
            else {
                resultMessage = "The Badge could not be found for deletion. It may already have been deleted.";
            }

            TempData[ControllerEnums.GlobalViewDataProperty.PageMessage.ToString()] = resultMessage;
            return RedirectToAction("Index");
        }

		/// <summary>
		/// Holds data to be passed to the Badge form for creates and edits
		/// </summary>
        public class BadgeFormViewModel
        {
            private BadgeFormViewModel() { }

			/// <summary>
			/// Creation method for creating the view model. Services may be passed to the creation 
			/// method to instantiate items such as lists for drop down boxes.
			/// </summary>
            public static BadgeFormViewModel CreateBadgeFormViewModel() {
                BadgeFormViewModel viewModel = new BadgeFormViewModel();
                return viewModel;
            }

            public Badge Badge { get; internal set; }
        }

        private readonly INHibernateQueryRepository<Badge> BadgeRepository;
    }
}

