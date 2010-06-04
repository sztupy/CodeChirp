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
    public class ChannelsControllerTests
    {
        [SetUp]
        public void SetUp() {
            ServiceLocatorInitializer.Init();
            controller = new ChannelsController(CreateMockChannelRepository(),null,null,null);
        }

        /// <summary>
        /// Add a couple of objects to the list within CreateChannels and change the 
        /// "ShouldEqual(0)" within this test to the respective number.
        /// </summary>
        [Test]
        public void CanListChannels() {
            ViewResult result = controller.Index(null,null,null,null).AssertViewRendered();

            result.ViewData.Model.ShouldNotBeNull();
            (result.ViewData.Model as List<Channel>).Count.ShouldEqual(0);
        }

        [Test]
        public void CanShowChannel() {
            ViewResult result = controller.Show(1,null,null).AssertViewRendered();

			result.ViewData.ShouldNotBeNull();
			
            (result.ViewData.Model as Channel).Id.ShouldEqual(1);
        }

        [Test]
        public void CanInitChannelCreation() {
            ViewResult result = controller.Create().AssertViewRendered();
            
            result.ViewData.Model.ShouldNotBeNull();
            result.ViewData.Model.ShouldBeOfType(typeof(ChannelsController.ChannelFormViewModel));
            (result.ViewData.Model as ChannelsController.ChannelFormViewModel).Channel.ShouldBeNull();
        }

        [Test]
        public void CanEnsureChannelCreationIsValid() {
            Channel ChannelFromForm = new Channel();
            ViewResult result = controller.Create(ChannelFromForm).AssertViewRendered();

            result.ViewData.Model.ShouldNotBeNull();
            result.ViewData.Model.ShouldBeOfType(typeof(ChannelsController.ChannelFormViewModel));
        }

        [Test]
        public void CanCreateChannel() {
            Channel ChannelFromForm = CreateTransientChannel();
            RedirectToRouteResult redirectResult = controller.Create(ChannelFromForm)
                .AssertActionRedirect().ToAction("Index");
            controller.TempData[ControllerEnums.GlobalViewDataProperty.PageMessage.ToString()].ToString()
				.ShouldContain("was successfully created");
        }

        [Test]
        public void CanUpdateChannel() {
            Channel ChannelFromForm = CreateTransientChannel();
            EntityIdSetter.SetIdOf<int>(ChannelFromForm, 1);
            RedirectToRouteResult redirectResult = controller.Edit(ChannelFromForm)
                .AssertActionRedirect().ToAction("Index");
            controller.TempData[ControllerEnums.GlobalViewDataProperty.PageMessage.ToString()].ToString()
				.ShouldContain("was successfully updated");
        }

        [Test]
        public void CanInitChannelEdit() {
            ViewResult result = controller.Edit(1).AssertViewRendered();

			result.ViewData.Model.ShouldNotBeNull();
            result.ViewData.Model.ShouldBeOfType(typeof(ChannelsController.ChannelFormViewModel));
            (result.ViewData.Model as ChannelsController.ChannelFormViewModel).Channel.Id.ShouldEqual(1);
        }

        [Test]
        public void CanDeleteChannel() {
            RedirectToRouteResult redirectResult = controller.DeleteConfirmed(1)
                .AssertActionRedirect().ToAction("Index");
            
            controller.TempData[ControllerEnums.GlobalViewDataProperty.PageMessage.ToString()].ToString()
				.ShouldContain("was successfully deleted");
        }

		#region Create Mock Channel Repository

        private INHibernateQueryRepository<Channel> CreateMockChannelRepository() {

            INHibernateQueryRepository<Channel> mockedRepository = MockRepository.GenerateMock<INHibernateQueryRepository<Channel>>();
            long outres;
            mockedRepository.Expect(mr => mr.GetAll(0, 0, out outres, null)).IgnoreArguments().Return(CreateChannels());
            mockedRepository.Expect(mr => mr.Get(1)).IgnoreArguments().Return(CreateChannel());
            mockedRepository.Expect(mr => mr.SaveOrUpdate(null)).IgnoreArguments().Return(CreateChannel());
            mockedRepository.Expect(mr => mr.Delete(null)).IgnoreArguments();

			IDbContext mockedDbContext = MockRepository.GenerateStub<IDbContext>();
			mockedDbContext.Stub(c => c.CommitChanges());
			mockedRepository.Stub(mr => mr.DbContext).Return(mockedDbContext);
            
            return mockedRepository;
        }

        private Channel CreateChannel() {
            Channel Channel = CreateTransientChannel();
            EntityIdSetter.SetIdOf<int>(Channel, 1);
            return Channel;
        }

          private List<Channel> CreateChannels() {
              List<Channel> Channels = new List<Channel>();

            // Create a number of domain object instances here and add them to the list

            return Channels;
        }
        
        #endregion

        /// <summary>
        /// Creates a valid, transient Channel; typical of something retrieved back from a form submission
        /// </summary>
        private Channel CreateTransientChannel() {
            Channel Channel = new Channel() {
                name = "TheChannel",
                owner = new User(),
            };
            
            return Channel;
        }

        private ChannelsController controller;
    }
}
