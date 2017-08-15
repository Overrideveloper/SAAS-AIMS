using AIMS.Data.DataContext.DataContext.DuesDataContext;
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
        private readonly MemberDataContext _memberdatacontext;
        private readonly DuesDataContext _duesdatacontext;
        private readonly SessionDataContext _sessiondatacontext;

        #region constructor
        public DashboardController()
        {
            _memberdatacontext = new MemberDataContext();
            _sessiondatacontext = new SessionDataContext();
            _duesdatacontext = new DuesDataContext();
        }
        #endregion

        #region jsonresults
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
            var currentmember = _memberdatacontext.Members.Where(s => s.YearOfAdmission == Convert.ToString(DateTime.Now.Year) || s.YearOfAdmission == Convert.ToString((DateTime.Now.Year - 1)));
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

        public JsonResult TotalDues()
        {
            var dues = _duesdatacontext.Dues.Sum(due => due.Amount);
            return Json(new { dues = dues }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        //
        // GET: /Dashboard/
        public ActionResult Index()
        {
            return View();
        }
	}
}