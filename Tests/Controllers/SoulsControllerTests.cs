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

namespace Tests.Blog.Web.Controllers
{
    [TestFixture]
    public class SoulsControllerTests
    {
        [SetUp]
        public void SetUp() {
            ServiceLocatorInitializer.Init();
            controller = new SoulsController(CreateMockSoulRepository());
        }

        /// <summary>
        /// Add a couple of objects to the list within CreateSouls and change the 
        /// "ShouldEqual(0)" within this test to the respective number.
        /// </summary>
        [Test]
        public void CanListSouls() {
            ViewResult result = controller.Index(null,null,null).AssertViewRendered();

            result.ViewData.Model.ShouldNotBeNull();
            (result.ViewData.Model as List<Soul>).Count.ShouldEqual(0);
        }

        [Test]
        public void CanShowSoul() {
            ViewResult result = controller.Show(1).AssertViewRendered();

			result.ViewData.ShouldNotBeNull();
			
            (result.ViewData.Model as Soul).Id.ShouldEqual(1);
        }

        [Test]
        public void CanInitSoulCreation() {
            ViewResult result = controller.Create().AssertViewRendered();
            
            result.ViewData.Model.ShouldNotBeNull();
            result.ViewData.Model.ShouldBeOfType(typeof(SoulsController.SoulFormViewModel));
            (result.ViewData.Model as SoulsController.SoulFormViewModel).Soul.ShouldBeNull();
        }

        [Test]
        public void CanEnsureSoulCreationIsValid() {
            Soul SoulFromForm = new Soul();
            ViewResult result = controller.Create(SoulFromForm).AssertViewRendered();

            result.ViewData.Model.ShouldNotBeNull();
            result.ViewData.Model.ShouldBeOfType(typeof(SoulsController.SoulFormViewModel));
        }

        [Test]
        public void CanCreateSoul() {
            Soul SoulFromForm = CreateTransientSoul();
            RedirectToRouteResult redirectResult = controller.Create(SoulFromForm)
                .AssertActionRedirect().ToAction("Index");
            controller.TempData[ControllerEnums.GlobalViewDataProperty.PageMessage.ToString()].ToString()
				.ShouldContain("was successfully created");
        }

        [Test]
        public void CanUpdateSoul() {
            Soul SoulFromForm = CreateTransientSoul();
            EntityIdSetter.SetIdOf<int>(SoulFromForm, 1);
            RedirectToRouteResult redirectResult = controller.Edit(SoulFromForm)
                .AssertActionRedirect().ToAction("Index");
            controller.TempData[ControllerEnums.GlobalViewDataProperty.PageMessage.ToString()].ToString()
				.ShouldContain("was successfully updated");
        }

        [Test]
        public void CanInitSoulEdit() {
            ViewResult result = controller.Edit(1).AssertViewRendered();

			result.ViewData.Model.ShouldNotBeNull();
            result.ViewData.Model.ShouldBeOfType(typeof(SoulsController.SoulFormViewModel));
            (result.ViewData.Model as SoulsController.SoulFormViewModel).Soul.Id.ShouldEqual(1);
        }

        [Test]
        public void CanDeleteSoul() {
            RedirectToRouteResult redirectResult = controller.DeleteConfirmed(1)
                .AssertActionRedirect().ToAction("Index");
            
            controller.TempData[ControllerEnums.GlobalViewDataProperty.PageMessage.ToString()].ToString()
				.ShouldContain("was successfully deleted");
        }

		#region Create Mock Soul Repository

        private INHibernateQueryRepository<Soul> CreateMockSoulRepository() {

            INHibernateQueryRepository<Soul> mockedRepository = MockRepository.GenerateMock<INHibernateQueryRepository<Soul>>();
            long outres;
            mockedRepository.Expect(mr => mr.GetAll(0, 0, out outres, null)).IgnoreArguments().Return(CreateSouls());
            mockedRepository.Expect(mr => mr.Get(1)).IgnoreArguments().Return(CreateSoul());
            mockedRepository.Expect(mr => mr.SaveOrUpdate(null)).IgnoreArguments().Return(CreateSoul());
            mockedRepository.Expect(mr => mr.Delete(null)).IgnoreArguments();

			IDbContext mockedDbContext = MockRepository.GenerateStub<IDbContext>();
			mockedDbContext.Stub(c => c.CommitChanges());
			mockedRepository.Stub(mr => mr.DbContext).Return(mockedDbContext);
            
            return mockedRepository;
        }

        private Soul CreateSoul() {
            Soul Soul = CreateTransientSoul();
            EntityIdSetter.SetIdOf<int>(Soul, 1);
            return Soul;
        }

          private List<Soul> CreateSouls() {
              List<Soul> Souls = new List<Soul>();

            // Create a number of domain object instances here and add them to the list

            return Souls;
        }
        
        #endregion

        /// <summary>
        /// Creates a valid, transient Soul; typical of something retrieved back from a form submission
        /// </summary>
        private Soul CreateTransientSoul() {
            Soul Soul = new Soul() {
                name = "An User",
                gravatar = "12345678901234567890123456789012",
                point = 0,
            };
            
            return Soul;
        }

        private SoulsController controller;
    }
}
