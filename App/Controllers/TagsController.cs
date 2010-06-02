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

        [Transaction]
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

        [Transaction]
        public ActionResult Show(int id) {
            Tag Tag = TagRepository.Get(id);
            return View(Tag);
        }

        public ActionResult Create() {
            TagFormViewModel viewModel = TagFormViewModel.CreateTagFormViewModel();
            return View(viewModel);
        }

        [ValidateAntiForgeryToken]
        [Transaction]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(Tag Tag) {
            if (ViewData.ModelState.IsValid && Tag.IsValid()) {
                TagRepository.SaveOrUpdate(Tag);

                TempData[ControllerEnums.GlobalViewDataProperty.PageMessage.ToString()] = 
					"The Tag was successfully created.";
                return RedirectToAction("Index");
            }

            TagFormViewModel viewModel = TagFormViewModel.CreateTagFormViewModel();
            viewModel.Tag = Tag;
            return View(viewModel);
        }

        [Transaction]
        public ActionResult Edit(int id) {
            TagFormViewModel viewModel = TagFormViewModel.CreateTagFormViewModel();
            viewModel.Tag = TagRepository.Get(id);
            return View(viewModel);
        }

        [ValidateAntiForgeryToken]
        [Transaction]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Edit(Tag Tag) {
            Tag TagToUpdate = TagRepository.Get(Tag.Id);
            TransferFormValuesTo(TagToUpdate, Tag);

            if (ViewData.ModelState.IsValid && Tag.IsValid()) {
                TempData[ControllerEnums.GlobalViewDataProperty.PageMessage.ToString()] = 
					"The Tag was successfully updated.";
                return RedirectToAction("Index");
            }
            else {
                TagRepository.DbContext.RollbackTransaction();

				TagFormViewModel viewModel = TagFormViewModel.CreateTagFormViewModel();
				viewModel.Tag = Tag;
				return View(viewModel);
            }
        }

        private void TransferFormValuesTo(Tag TagToUpdate, Tag TagFromForm) {
            TagToUpdate.name = TagFromForm.name;
        }


        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Delete(int id)
        {
            Tag TagToDelete = TagRepository.Get(id);
            return View(TagToDelete);
        }

        [ValidateAntiForgeryToken]
        [Transaction]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeleteConfirmed(int id) {
            string resultMessage = "The Tag was successfully deleted.";
            Tag TagToDelete = TagRepository.Get(id);

            if (TagToDelete != null) {
                TagRepository.Delete(TagToDelete);

                try {
                    TagRepository.DbContext.CommitChanges();
                }
                catch {
                    resultMessage = "A problem was encountered preventing the Tag from being deleted. " +
						"Another item likely depends on this Tag.";
                    TagRepository.DbContext.RollbackTransaction();
                }
            }
            else {
                resultMessage = "The Tag could not be found for deletion. It may already have been deleted.";
            }

            TempData[ControllerEnums.GlobalViewDataProperty.PageMessage.ToString()] = resultMessage;
            return RedirectToAction("Index");
        }

		/// <summary>
		/// Holds data to be passed to the Tag form for creates and edits
		/// </summary>
        public class TagFormViewModel
        {
            private TagFormViewModel() { }

			/// <summary>
			/// Creation method for creating the view model. Services may be passed to the creation 
			/// method to instantiate items such as lists for drop down boxes.
			/// </summary>
            public static TagFormViewModel CreateTagFormViewModel() {
                TagFormViewModel viewModel = new TagFormViewModel();
                return viewModel;
            }

            public Tag Tag { get; internal set; }
        }

        private readonly INHibernateQueryRepository<Tag> TagRepository;
    }
}

