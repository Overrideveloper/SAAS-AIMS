using AIMS.Data.DataContext.DataContext.MemberDataContext;
using AIMS.Data.DataContext.DataContext.SessionDataContext;
using AIMS.Data.Enums.Enums.Gender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SAAS_AIMS.Controllers
{
    public class DashboardController : BaseController
    {
        MemberDataContext _memberdatacontext;
        SessionDataContext _sessiondatacontext;

        #region constructor
        public DashboardController()
        {
            _memberdatacontext = new MemberDataContext();
            _sessiondatacontext = new SessionDataContext();
        }
        #endregion

        public JsonResult SessionCount()
        {
            var session = _sessiondatacontext.Sessions.ToArray().Length;
            return Json(new { session = session }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult MaleMembers()
        {
            var males = _memberdatacontext.Members.Where(member => member.Gender == Gender.Male).ToArray().Length;
            return Json(new { males = males }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FemaleMembers()
        {
            var females = _memberdatacontext.Members.Where(member => member.Gender == Gender.Female).ToArray().Length;
            return Json(new { females = females }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Members()
        {
            var members = _memberdatacontext.Members.ToArray().Length;
            return Json(new { members = members }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CurrentSessionMembers()
        {
            var currentmember = _memberdatacontext.Members.Where(s => Convert.ToDateTime(s.YearOfAdmission).Year == DateTime.Now.Year || s.YearOfAdmission == (DateTime.Now.Year - 1).ToString());
            return Json(new { currentmember = currentmember }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CurrentSessionMale()
        {
            var currentmale = _memberdatacontext.Members.Where(s => s.Gender == Gender.Male && s.YearOfAdmission == DateTime.Now.Year.ToString() || s.YearOfAdmission == (DateTime.Now.Year - 1).ToString());
            return Json(new { currentmale = currentmale }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CurrentSessionFemale()
        {
            var currentfemale = _memberdatacontext.Members.Where(s => s.Gender == Gender.Female && s.YearOfAdmission == DateTime.Now.Year.ToString() || s.YearOfAdmission == (DateTime.Now.Year - 1).ToString());
            return Json(new { currentfemale = currentfemale }, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Dashboard/
        public ActionResult Index()
        {
            return View();
        }
	}
}