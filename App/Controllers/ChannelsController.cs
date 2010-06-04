using System;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Text;
using System.Linq;
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
using Shaml.Membership.Core;

namespace CodeChirp.Controllers
{
    [HandleError]
    [GenericLogger]
    public class ChannelsController : Controller
    {
        public ChannelsController(INHibernateQueryRepository<Channel> ChannelRepository, INHibernateQueryRepository<Soul> SoulRepository, INHibernateQueryRepository<Post> PostRepository, IRepository<User> UserRepository)
        {
            Check.Require(ChannelRepository != null, "ChannelRepository may not be null");

            this.ChannelRepository = ChannelRepository;
            this.SoulRepository = SoulRepository;
            this.PostRepository = PostRepository;
            this.UserRepository = UserRepository;
        }

        public ActionResult Index(int? Page, bool? Desc, string type, string q)
        {
            int page = 0;
            long numResults;
            if (Page.HasValue && Page.Value >= 0)
            {
                page = Page.Value;
            }
            IList<Channel> Channels = null;
            if (q != null)
            {
                var eb = ChannelRepository.CreateExpressionBuilder();
                IExpression exp = eb.Like("name", "%" + q + "%", true);
                Channels = ChannelRepository.FindByExpression(exp, 40, page, out numResults, ChannelRepository.CreateOrder("name", Desc == true));
            }
            else
            {
                Channels = ChannelRepository.GetAll(40, page, out numResults, ChannelRepository.CreateOrder("name", Desc == true));
            }
            PaginationData pd = new ThreeWayPaginationData(page, 40, numResults);
            ViewData["Pagination"] = pd;
            if (type == "json")
            {
                return new JsonNetResult(Channels);
            }
            else
            {
                return View(Channels);
            }
        }

        [Authorize]
        public ActionResult Manage()
        {
            User u = UserRepository.FindOne(new { Username = User.Identity.Name });
            var eb = ChannelRepository.CreateExpressionBuilder();
            IList<Channel> Channels = ChannelRepository.FindByExpression(eb.Eq("owner", u),0,0,ChannelRepository.CreateOrder("name",false));
            return View(Channels);
        }

        [Authorize]
        public ActionResult UserChannels(int? Id)
        {
          if (Id.HasValue) {
              User u = UserRepository.FindOne(new { Username = User.Identity.Name });
              var eb = ChannelRepository.CreateExpressionBuilder();
              IList<Channel> Channels = ChannelRepository.FindByExpression(eb.Eq("owner", u), 0, 0, ChannelRepository.CreateOrder("name", false));
              ViewData["data"] = Id.Value;
              return View("UserChannels","Empty",Channels);
          } else {
              return Content("");
          }
        }

        [Authorize]
        [Transaction]
        public ActionResult Add(int Id, int Data)
        {
            Channel c = ChannelRepository.Get(Id);
            if (c.owner.Username != User.Identity.Name)
            {
                throw new HttpException(401, "HTTP/1.1 401 Unauthorized");
            }
            Soul s = SoulRepository.Get(Data);
            if ((s != null) && (c.users.Count<10))
            {
                c.AddUser(s);
            }
            return RedirectToAction("Show", "Souls", new { id = Data });
        }

        public IList<Post> GetPostsForChannel(Channel c, int Page)
        {
            SoulsController sc = new SoulsController(SoulRepository, PostRepository, null, null);
            List<Post> p = new List<Post>();
            foreach (var u in c.users)
            {
                p.AddRange(sc.GetPostsForSoul(u.Id, Page, 10));
            }
            return p.OrderBy(x => x.lastedit).Reverse().ToList();
        }

        public ActionResult Show(int id, int? Page, string type)
        {
            Channel c = ChannelRepository.Get(id);
            if (c == null)
            {
                throw new HttpException(404, "HTTP/1.1 404 Not Found");
            }
            int page = 0;
            if (Page.HasValue && Page.Value > 0)
            {
                page = Page.Value;
            }
            IList<Post> p = GetPostsForChannel(c, page);
            ViewData["page"] = page + 1;
            ViewData["channel"] = c;
            if (type == "json")
            {
                return new JsonNetResult(new { channel = c, data = p });
            }
            else
            {
                return View(p);
            }
        }

        [Authorize]
        public ActionResult Create() {
            ChannelFormViewModel viewModel = ChannelFormViewModel.CreateChannelFormViewModel();
            return View(viewModel);
        }

        [ValidateAntiForgeryToken]
        [Transaction]
        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize]
        public ActionResult Create(Channel Channel) {
            Channel.owner = UserRepository.FindOne( new { Username = User.Identity.Name } );
            if (ViewData.ModelState.IsValid && Channel.IsValid()) {
                ChannelRepository.SaveOrUpdate(Channel);

                TempData[ControllerEnums.GlobalViewDataProperty.PageMessage.ToString()] = 
					"The Channel was successfully created.";
                return RedirectToAction("Index");
            }

            ChannelFormViewModel viewModel = ChannelFormViewModel.CreateChannelFormViewModel();
            viewModel.Channel = Channel;
            return View(viewModel);
        }

        [Transaction]
        [Authorize]
        public ActionResult Edit(int id) {
            ChannelFormViewModel viewModel = ChannelFormViewModel.CreateChannelFormViewModel();
            viewModel.Channel = ChannelRepository.Get(id);
            if (viewModel.Channel.owner.Username != User.Identity.Name)
            {
                throw new HttpException(401, "HTTP/1.1 401 Unauthorized");
            }
            return View(viewModel);
        }

        [ValidateAntiForgeryToken]
        [Transaction]
        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize]
        public ActionResult Edit(Channel Channel) {
            Channel ChannelToUpdate = ChannelRepository.Get(Channel.Id);
            TransferFormValuesTo(ChannelToUpdate, Channel);
            if (ChannelToUpdate.owner.Username != User.Identity.Name)
            {
                throw new HttpException(401, "HTTP/1.1 401 Unauthorized");
            }
            if (ViewData.ModelState.IsValid && Channel.IsValid()) {
                TempData[ControllerEnums.GlobalViewDataProperty.PageMessage.ToString()] = 
					"The Channel was successfully updated.";
                return RedirectToAction("Index");
            }
            else {
                ChannelRepository.DbContext.RollbackTransaction();

				ChannelFormViewModel viewModel = ChannelFormViewModel.CreateChannelFormViewModel();
				viewModel.Channel = Channel;
				return View(viewModel);
            }
        }

        private void TransferFormValuesTo(Channel ChannelToUpdate, Channel ChannelFromForm) {
            ChannelToUpdate.name = ChannelFromForm.name;
        }


        [AcceptVerbs(HttpVerbs.Get)]
        [Authorize]
        public ActionResult Delete(int id)
        {
            Channel ChannelToDelete = ChannelRepository.Get(id);
            if (ChannelToDelete.owner.Username != User.Identity.Name)
            {
                throw new HttpException(401, "HTTP/1.1 401 Unauthorized");
            }
            return View(ChannelToDelete);
        }

        [ValidateAntiForgeryToken]
        [Transaction]
        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize]
        public ActionResult DeleteConfirmed(int id) {
            string resultMessage = "The Channel was successfully deleted.";
            Channel ChannelToDelete = ChannelRepository.Get(id);
            if (ChannelToDelete.owner.Username != User.Identity.Name)
            {
                throw new HttpException(401, "HTTP/1.1 401 Unauthorized");
            }
            if (ChannelToDelete != null) {
                ChannelRepository.Delete(ChannelToDelete);

                try {
                    ChannelRepository.DbContext.CommitChanges();
                }
                catch {
                    resultMessage = "A problem was encountered preventing the Channel from being deleted. " +
						"Another item likely depends on this Channel.";
                    ChannelRepository.DbContext.RollbackTransaction();
                }
            }
            else {
                resultMessage = "The Channel could not be found for deletion. It may already have been deleted.";
            }

            TempData[ControllerEnums.GlobalViewDataProperty.PageMessage.ToString()] = resultMessage;
            return RedirectToAction("Index");
        }

		/// <summary>
		/// Holds data to be passed to the Channel form for creates and edits
		/// </summary>
        public class ChannelFormViewModel
        {
            private ChannelFormViewModel() { }

			/// <summary>
			/// Creation method for creating the view model. Services may be passed to the creation 
			/// method to instantiate items such as lists for drop down boxes.
			/// </summary>
            public static ChannelFormViewModel CreateChannelFormViewModel() {
                ChannelFormViewModel viewModel = new ChannelFormViewModel();
                return viewModel;
            }

            public Channel Channel { get; internal set; }
        }

        private readonly INHibernateQueryRepository<Channel> ChannelRepository;
        private readonly INHibernateQueryRepository<Soul> SoulRepository;
        private readonly INHibernateQueryRepository<Post> PostRepository;
        private readonly IRepository<User> UserRepository;
    }
}

