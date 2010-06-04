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
    public class TagsControllerTests
    {
        [SetUp]
        public void SetUp() {
            ServiceLocatorInitializer.Init();
            controller = new TagsController(CreateMockTagRepository(),null);
        }

        /// <summary>
        /// Add a couple of objects to the list within CreateTags and change the 
        /// "ShouldEqual(0)" within this test to the respective number.
        /// </summary>
        [Test]
        public void CanListTags() {
            ViewResult result = controller.Index(null,null,null,null).AssertViewRendered();

            result.ViewData.Model.ShouldNotBeNull();
            (result.ViewData.Model as List<Tag>).Count.ShouldEqual(0);
        }

        [Test]
        public void CanShowTag() {
            ViewResult result = controller.Show(1,null,null).AssertViewRendered();

			result.ViewData.ShouldNotBeNull();
			
            (result.ViewData.Model as Tag).Id.ShouldEqual(1);
        }

		#region Create Mock Tag Repository

        private INHibernateQueryRepository<Tag> CreateMockTagRepository() {

            INHibernateQueryRepository<Tag> mockedRepository = MockRepository.GenerateMock<INHibernateQueryRepository<Tag>>();
            long outres;
            mockedRepository.Expect(mr => mr.GetAll(0, 0, out outres, null)).IgnoreArguments().Return(CreateTags());
            mockedRepository.Expect(mr => mr.Get(1)).IgnoreArguments().Return(CreateTag());
            mockedRepository.Expect(mr => mr.SaveOrUpdate(null)).IgnoreArguments().Return(CreateTag());
            mockedRepository.Expect(mr => mr.Delete(null)).IgnoreArguments();

			IDbContext mockedDbContext = MockRepository.GenerateStub<IDbContext>();
			mockedDbContext.Stub(c => c.CommitChanges());
			mockedRepository.Stub(mr => mr.DbContext).Return(mockedDbContext);
            
            return mockedRepository;
        }

        private Tag CreateTag() {
            Tag Tag = CreateTransientTag();
            EntityIdSetter.SetIdOf<int>(Tag, 1);
            return Tag;
        }

          private List<Tag> CreateTags() {
              List<Tag> Tags = new List<Tag>();

            // Create a number of domain object instances here and add them to the list

            return Tags;
        }
        
        #endregion

        /// <summary>
        /// Creates a valid, transient Tag; typical of something retrieved back from a form submission
        /// </summary>
        private Tag CreateTransientTag() {
            Tag Tag = new Tag() {
                name = "TheTag"
            };
            
            return Tag;
        }

        private TagsController controller;
    }
}
