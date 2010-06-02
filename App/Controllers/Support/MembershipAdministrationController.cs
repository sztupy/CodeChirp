using System.Web.Mvc;
using CodeChirp.ApplicationServices;
using Shaml.Core;
using Shaml.Web;

namespace CodeChirp.Controllers
{
	  [Authorize( Roles="Administrator" )]
    [GenericLogger]
	  [HandleError]
    public class MembershipAdministrationController : MembershipAdministrationController_Base{}
}
