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
            controller = new SoulsController(CreateMockSoulRepository(),null);
        }

        /// <summary>
        /// Add a couple of objects to the list within CreateSouls and change the 
        /// "ShouldEqual(0)" within this test to the respective number.
        /// </summary>
        [Test]
        public void CanListSouls() {
            ViewResult result = controller.Index(null,null,null,null).AssertViewRendered();

            result.ViewData.Model.ShouldNotBeNull();
            (result.ViewData.Model as List<Soul>).Count.ShouldEqual(0);
        }

        [Test]
        public void CanShowSoul() {
            ViewResult result = controller.Show(1,null,null).AssertViewRendered();

			result.ViewData.ShouldNotBeNull();
			
            (result.ViewData.Model as Soul).Id.ShouldEqual(1);
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
