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

        [Transaction]
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

        [Transaction]
        public ActionResult Show(int id) {
            Post Post = PostRepository.Get(id);
            return View(Post);
        }

        public ActionResult Create() {
            PostFormViewModel viewModel = PostFormViewModel.CreatePostFormViewModel();
            return View(viewModel);
        }

        [ValidateAntiForgeryToken]
        [Transaction]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(Post Post) {
            if (ViewData.ModelState.IsValid && Post.IsValid()) {
                PostRepository.SaveOrUpdate(Post);

                TempData[ControllerEnums.GlobalViewDataProperty.PageMessage.ToString()] = 
					"The Post was successfully created.";
                return RedirectToAction("Index");
            }

            PostFormViewModel viewModel = PostFormViewModel.CreatePostFormViewModel();
            viewModel.Post = Post;
            return View(viewModel);
        }

        [Transaction]
        public ActionResult Edit(int id) {
            PostFormViewModel viewModel = PostFormViewModel.CreatePostFormViewModel();
            viewModel.Post = PostRepository.Get(id);
            return View(viewModel);
        }

        [ValidateAntiForgeryToken]
        [Transaction]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Edit(Post Post) {
            Post PostToUpdate = PostRepository.Get(Post.Id);
            TransferFormValuesTo(PostToUpdate, Post);

            if (ViewData.ModelState.IsValid && Post.IsValid()) {
                TempData[ControllerEnums.GlobalViewDataProperty.PageMessage.ToString()] = 
					"The Post was successfully updated.";
                return RedirectToAction("Index");
            }
            else {
                PostRepository.DbContext.RollbackTransaction();

				PostFormViewModel viewModel = PostFormViewModel.CreatePostFormViewModel();
				viewModel.Post = Post;
				return View(viewModel);
            }
        }

        private void TransferFormValuesTo(Post PostToUpdate, Post PostFromForm) {
            PostToUpdate.type = PostFromForm.type;
            PostToUpdate.summary = PostFromForm.summary;
            PostToUpdate.body = PostFromForm.body;
            PostToUpdate.community = PostFromForm.community;
            PostToUpdate.score = PostFromForm.score;
            PostToUpdate.user = PostFromForm.user;
            PostToUpdate.lastactivity = PostFromForm.lastactivity;
        }


        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Delete(int id)
        {
            Post PostToDelete = PostRepository.Get(id);
            return View(PostToDelete);
        }

        [ValidateAntiForgeryToken]
        [Transaction]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeleteConfirmed(int id) {
            string resultMessage = "The Post was successfully deleted.";
            Post PostToDelete = PostRepository.Get(id);

            if (PostToDelete != null) {
                PostRepository.Delete(PostToDelete);

                try {
                    PostRepository.DbContext.CommitChanges();
                }
                catch {
                    resultMessage = "A problem was encountered preventing the Post from being deleted. " +
						"Another item likely depends on this Post.";
                    PostRepository.DbContext.RollbackTransaction();
                }
            }
            else {
                resultMessage = "The Post could not be found for deletion. It may already have been deleted.";
            }

            TempData[ControllerEnums.GlobalViewDataProperty.PageMessage.ToString()] = resultMessage;
            return RedirectToAction("Index");
        }

		/// <summary>
		/// Holds data to be passed to the Post form for creates and edits
		/// </summary>
        public class PostFormViewModel
        {
            private PostFormViewModel() { }

			/// <summary>
			/// Creation method for creating the view model. Services may be passed to the creation 
			/// method to instantiate items such as lists for drop down boxes.
			/// </summary>
            public static PostFormViewModel CreatePostFormViewModel() {
                PostFormViewModel viewModel = new PostFormViewModel();
                return viewModel;
            }

            public Post Post { get; internal set; }
        }

        private readonly INHibernateQueryRepository<Post> PostRepository;
    }
}

