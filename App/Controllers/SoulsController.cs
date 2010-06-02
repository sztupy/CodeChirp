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

        [Transaction]
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

        [Transaction]
        public ActionResult Show(int id) {
            Soul Soul = SoulRepository.Get(id);
            return View(Soul);
        }

        public ActionResult Create() {
            SoulFormViewModel viewModel = SoulFormViewModel.CreateSoulFormViewModel();
            return View(viewModel);
        }

        [ValidateAntiForgeryToken]
        [Transaction]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(Soul Soul) {
            if (ViewData.ModelState.IsValid && Soul.IsValid()) {
                SoulRepository.SaveOrUpdate(Soul);

                TempData[ControllerEnums.GlobalViewDataProperty.PageMessage.ToString()] = 
					"The Soul was successfully created.";
                return RedirectToAction("Index");
            }

            SoulFormViewModel viewModel = SoulFormViewModel.CreateSoulFormViewModel();
            viewModel.Soul = Soul;
            return View(viewModel);
        }

        [Transaction]
        public ActionResult Edit(int id) {
            SoulFormViewModel viewModel = SoulFormViewModel.CreateSoulFormViewModel();
            viewModel.Soul = SoulRepository.Get(id);
            return View(viewModel);
        }

        [ValidateAntiForgeryToken]
        [Transaction]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Edit(Soul Soul) {
            Soul SoulToUpdate = SoulRepository.Get(Soul.Id);
            TransferFormValuesTo(SoulToUpdate, Soul);

            if (ViewData.ModelState.IsValid && Soul.IsValid()) {
                TempData[ControllerEnums.GlobalViewDataProperty.PageMessage.ToString()] = 
					"The Soul was successfully updated.";
                return RedirectToAction("Index");
            }
            else {
                SoulRepository.DbContext.RollbackTransaction();

				SoulFormViewModel viewModel = SoulFormViewModel.CreateSoulFormViewModel();
				viewModel.Soul = Soul;
				return View(viewModel);
            }
        }

        private void TransferFormValuesTo(Soul SoulToUpdate, Soul SoulFromForm) {
            SoulToUpdate.name = SoulFromForm.name;
            SoulToUpdate.gravatar = SoulFromForm.gravatar;
            SoulToUpdate.point = SoulFromForm.point;
        }


        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Delete(int id)
        {
            Soul SoulToDelete = SoulRepository.Get(id);
            return View(SoulToDelete);
        }

        [ValidateAntiForgeryToken]
        [Transaction]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeleteConfirmed(int id) {
            string resultMessage = "The Soul was successfully deleted.";
            Soul SoulToDelete = SoulRepository.Get(id);

            if (SoulToDelete != null) {
                SoulRepository.Delete(SoulToDelete);

                try {
                    SoulRepository.DbContext.CommitChanges();
                }
                catch {
                    resultMessage = "A problem was encountered preventing the Soul from being deleted. " +
						"Another item likely depends on this Soul.";
                    SoulRepository.DbContext.RollbackTransaction();
                }
            }
            else {
                resultMessage = "The Soul could not be found for deletion. It may already have been deleted.";
            }

            TempData[ControllerEnums.GlobalViewDataProperty.PageMessage.ToString()] = resultMessage;
            return RedirectToAction("Index");
        }

		/// <summary>
		/// Holds data to be passed to the Soul form for creates and edits
		/// </summary>
        public class SoulFormViewModel
        {
            private SoulFormViewModel() { }

			/// <summary>
			/// Creation method for creating the view model. Services may be passed to the creation 
			/// method to instantiate items such as lists for drop down boxes.
			/// </summary>
            public static SoulFormViewModel CreateSoulFormViewModel() {
                SoulFormViewModel viewModel = new SoulFormViewModel();
                return viewModel;
            }

            public Soul Soul { get; internal set; }
        }

        private readonly INHibernateQueryRepository<Soul> SoulRepository;
    }
}

