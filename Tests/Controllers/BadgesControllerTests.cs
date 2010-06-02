using System;
using MvcContrib.TestHelper;
using NUnit.Framework;
using Rhino.Mocks;
using Shaml.Core.PersistenceSupport;
using Shaml.Testing;
using Shaml.Testing.NUnit;
using Shaml.Membership.Core;
using System.Collections.Generic;
using System.Web.Mvc;
using CodeChirp;
using CodeChirp.Config;
using CodeChirp.Core;
using CodeChirp.Controllers;
using Shaml.Core.PersistenceSupport.NHibernate;
using Salient.StackApps.Routes;

namespace Tests.Blog.Web.Controllers
{
    [TestFixture]
    public class BadgesControllerTests
    {
        [SetUp]
        public void SetUp() {
            ServiceLocatorInitializer.Init();
            controller = new BadgesController(CreateMockBadgeRepository());
        }

        /// <summary>
        /// Add a couple of objects to the list within CreateBadges and change the 
        /// "ShouldEqual(0)" within this test to the respective number.
        /// </summary>
        [Test]
        public void CanListBadges() {
            ViewResult result = controller.Index(null,null,null).AssertViewRendered();

            result.ViewData.Model.ShouldNotBeNull();
            (result.ViewData.Model as List<Badge>).Count.ShouldEqual(0);
        }

        [Test]
        public void CanShowBadge() {
            ViewResult result = controller.Show(1).AssertViewRendered();

			result.ViewData.ShouldNotBeNull();
			
            (result.ViewData.Model as Badge).Id.ShouldEqual(1);
        }

        [Test]
        public void CanInitBadgeCreation() {
            ViewResult result = controller.Create().AssertViewRendered();
            
            result.ViewData.Model.ShouldNotBeNull();
            result.ViewData.Model.ShouldBeOfType(typeof(BadgesController.BadgeFormViewModel));
            (result.ViewData.Model as BadgesController.BadgeFormViewModel).Badge.ShouldBeNull();
        }

        [Test]
        public void CanEnsureBadgeCreationIsValid() {
            Badge BadgeFromForm = new Badge();
            ViewResult result = controller.Create(BadgeFromForm).AssertViewRendered();

            result.ViewData.Model.ShouldNotBeNull();
            result.ViewData.Model.ShouldBeOfType(typeof(BadgesController.BadgeFormViewModel));
        }

        [Test]
        public void CanCreateBadge() {
            Badge BadgeFromForm = CreateTransientBadge();
            RedirectToRouteResult redirectResult = controller.Create(BadgeFromForm)
                .AssertActionRedirect().ToAction("Index");
            controller.TempData[ControllerEnums.GlobalViewDataProperty.PageMessage.ToString()].ToString()
				.ShouldContain("was successfully created");
        }

        [Test]
        public void CanUpdateBadge() {
            Badge BadgeFromForm = CreateTransientBadge();
            EntityIdSetter.SetIdOf<int>(BadgeFromForm, 1);
            RedirectToRouteResult redirectResult = controller.Edit(BadgeFromForm)
                .AssertActionRedirect().ToAction("Index");
            controller.TempData[ControllerEnums.GlobalViewDataProperty.PageMessage.ToString()].ToString()
				.ShouldContain("was successfully updated");
        }

        [Test]
        public void CanInitBadgeEdit() {
            ViewResult result = controller.Edit(1).AssertViewRendered();

			result.ViewData.Model.ShouldNotBeNull();
            result.ViewData.Model.ShouldBeOfType(typeof(BadgesController.BadgeFormViewModel));
            (result.ViewData.Model as BadgesController.BadgeFormViewModel).Badge.Id.ShouldEqual(1);
        }

        [Test]
        public void CanDeleteBadge() {
            RedirectToRouteResult redirectResult = controller.DeleteConfirmed(1)
                .AssertActionRedirect().ToAction("Index");
            
            controller.TempData[ControllerEnums.GlobalViewDataProperty.PageMessage.ToString()].ToString()
				.ShouldContain("was successfully deleted");
        }

		#region Create Mock Badge Repository

        private INHibernateQueryRepository<Badge> CreateMockBadgeRepository() {

            INHibernateQueryRepository<Badge> mockedRepository = MockRepository.GenerateMock<INHibernateQueryRepository<Badge>>();
            long outres;
            mockedRepository.Expect(mr => mr.GetAll(0, 0, out outres, null)).IgnoreArguments().Return(CreateBadges());
            mockedRepository.Expect(mr => mr.Get(1)).IgnoreArguments().Return(CreateBadge());
            mockedRepository.Expect(mr => mr.SaveOrUpdate(null)).IgnoreArguments().Return(CreateBadge());
            mockedRepository.Expect(mr => mr.Delete(null)).IgnoreArguments();

			IDbContext mockedDbContext = MockRepository.GenerateStub<IDbContext>();
			mockedDbContext.Stub(c => c.CommitChanges());
			mockedRepository.Stub(mr => mr.DbContext).Return(mockedDbContext);
            
            return mockedRepository;
        }

        private Badge CreateBadge() {
            Badge Badge = CreateTransientBadge();
            EntityIdSetter.SetIdOf<int>(Badge, 1);
            return Badge;
        }

          private List<Badge> CreateBadges() {
              List<Badge> Badges = new List<Badge>();

            // Create a number of domain object instances here and add them to the list

            return Badges;
        }
        
        #endregion

        /// <summary>
        /// Creates a valid, transient Badge; typical of something retrieved back from a form submission
        /// </summary>
        private Badge CreateTransientBadge() {
            Badge Badge = new Badge() {
                name = "TheBadge",
                count = 0,
                rank = BadgesRank.gold,
                description = "TheDesc"
            };
            
            return Badge;
        }

        private BadgesController controller;
    }
}
