using AIMS.Data.DataContext.DataContext.MemberDataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SAAS_AIMS.Controllers
{
    public class MemberController : Controller
    {
        
        private MemberDataContext _memberDataContext;
        
        public MemberController()
        {
            _memberDataContext = new MemberDataContext();
        }

        //
        // GET: /Member/
        public ActionResult Index()
        {
            var member = from m in _memberDataContext.Members
                             select m;
            return View(member);
        }
	}
}