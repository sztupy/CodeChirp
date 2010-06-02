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
    public class PostsControllerTests
    {
        [SetUp]
        public void SetUp() {
            ServiceLocatorInitializer.Init();
            controller = new PostsController(CreateMockPostRepository());
        }

        /// <summary>
        /// Add a couple of objects to the list within CreatePosts and change the 
        /// "ShouldEqual(0)" within this test to the respective number.
        /// </summary>
        [Test]
        public void CanListPosts() {
            ViewResult result = controller.Index(null,null,null).AssertViewRendered();

            result.ViewData.Model.ShouldNotBeNull();
            (result.ViewData.Model as List<Post>).Count.ShouldEqual(0);
        }

        [Test]
        public void CanShowPost() {
            ViewResult result = controller.Show(1).AssertViewRendered();

			result.ViewData.ShouldNotBeNull();
			
            (result.ViewData.Model as Post).Id.ShouldEqual(1);
        }

		#region Create Mock Post Repository

        private INHibernateQueryRepository<Post> CreateMockPostRepository() {

            INHibernateQueryRepository<Post> mockedRepository = MockRepository.GenerateMock<INHibernateQueryRepository<Post>>();
            long outres;
            mockedRepository.Expect(mr => mr.GetAll(0, 0, out outres, null)).IgnoreArguments().Return(CreatePosts());
            mockedRepository.Expect(mr => mr.Get(1)).IgnoreArguments().Return(CreatePost());
            mockedRepository.Expect(mr => mr.SaveOrUpdate(null)).IgnoreArguments().Return(CreatePost());
            mockedRepository.Expect(mr => mr.Delete(null)).IgnoreArguments();

			IDbContext mockedDbContext = MockRepository.GenerateStub<IDbContext>();
			mockedDbContext.Stub(c => c.CommitChanges());
			mockedRepository.Stub(mr => mr.DbContext).Return(mockedDbContext);
            
            return mockedRepository;
        }

        private Post CreatePost() {
            Post Post = CreateTransientPost();
            EntityIdSetter.SetIdOf<int>(Post, 1);
            return Post;
        }

          private List<Post> CreatePosts() {
              List<Post> Posts = new List<Post>();

            // Create a number of domain object instances here and add them to the list

            return Posts;
        }
        
        #endregion

        /// <summary>
        /// Creates a valid, transient Post; typical of something retrieved back from a form submission
        /// </summary>
        private Post CreateTransientPost() {
            Post Post = new Post() {
                type = PostType.answer,
                summary = "SomeTitle",
                body = "TheBody",
                community = false,
                score = 0,
                user = new Soul(),
                lastactivity = DateTime.Now
            };
            
            return Post;
        }

        private PostsController controller;
    }
}
