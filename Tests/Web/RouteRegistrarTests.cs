using NUnit.Framework;
using CodeChirp.Controllers;
using System.Web.Routing;
using MvcContrib.TestHelper;
using CodeChirp.ApplicationServices;
using CodeChirp;
using CodeChirp.Config;

namespace Tests.CodeChirp
{
    [TestFixture]
    public class RouteRegistrarTests
    {
        [SetUp]
        public void SetUp()
        {
            RouteTable.Routes.Clear();
            RouteRegistrar.RegisterRoutesTo(RouteTable.Routes);
        }

        [Test]
        public void CanVerifyRouteMaps()
        {
            "~/".Route().ShouldMapTo<HomeController>(x => x.Index(null));
            "~/OpenId".Route().ShouldMapTo<OpenIdController>(x => x.Index());
            "~/Account/LogOn".Route().ShouldMapTo<AccountController>(x => x.LogOn());
            "~/Account/LogOff".Route().ShouldMapTo<AccountController>(x => x.LogOff());
            "~/MembershipAdministration".Route().ShouldMapTo<MembershipAdministrationController>(x => x.Index(null,null));
        }
    }
}
