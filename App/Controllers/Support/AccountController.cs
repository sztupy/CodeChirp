using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using Shaml.Core;
using Shaml.Membership;
using Shaml.Web;

namespace CodeChirp.Controllers {

    [HandleError]
    [GenericLogger]
    public class AccountController : Controller {

        // This constructor is used by the MVC framework to instantiate the controller using
        // the default forms authentication and membership providers.

        public AccountController()
            : this(null, null) {
        }

        // This constructor is not used by the MVC framework but is instead provided for ease
        // of unit testing this type. See the comments at the end of this file for more
        // information.
        public AccountController(IFormsAuthentication formsAuth, IMembershipService service) {
            FormsAuth = formsAuth ?? new FormsAuthenticationService();
            MembershipService = service ?? new AccountMembershipService();
        }

        public IFormsAuthentication FormsAuth {
            get;
            private set;
        }

        public IMembershipService MembershipService {
            get;
            private set;
        }

        public ActionResult LogOff() {

            FormsAuth.SignOut();

            return RedirectToAction("Index", "Home");
        }
    }
}
