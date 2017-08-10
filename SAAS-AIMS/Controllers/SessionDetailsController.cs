using AIMS.Data.DataContext.DataContext.MemberDataContext;
using AIMS.Data.DataContext.DataContext.SessionDataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SAAS_AIMS.Controllers
{
    public class SessionDetailsController : Controller
    {

        //
        // GET: /SessionDetails/
        public ActionResult Statistics(long sessionid)
        {
            Session["sessionid"] = sessionid;
            return View("Statistics");
        }
	}
}