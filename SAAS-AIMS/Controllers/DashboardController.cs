using AIMS.Data.DataContext.DataContext.DuesDataContext;
using AIMS.Data.DataContext.DataContext.MemberDataContext;
using AIMS.Data.DataContext.DataContext.SessionDataContext;
using AIMS.Data.DataObjects.Entities.Member;
using AIMS.Data.Enums.Enums.Gender;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AIMS.Data.ViewModels.ViewModel.Member;

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

        #region member count
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
            var currentmember = _memberdatacontext.Members.Where(s => (s.YearOfAdmission.Year == DateTime.Now.Year) || s.YearOfAdmission.Year == (DateTime.Now.Year - 1)).ToArray().Length;
            Dispose();
            return Json(new { currentmember = currentmember }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CurrentSessionMale()
        {
            var currentmale = _memberdatacontext.Members.Where(s => s.Gender == Gender.Male && (s.YearOfAdmission.Year == DateTime.Now.Year || s.YearOfAdmission.Year == (DateTime.Now.Year - 1))).ToArray().Length;
            return Json(new { currentmale = currentmale }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CurrentSessionFemale()
        {
            var currentfemale = _memberdatacontext.Members.Where(s => s.Gender == Gender.Female && s.YearOfAdmission.Year == DateTime.Now.Year || s.YearOfAdmission.Year == (DateTime.Now.Year - 1)).ToArray().Length;
            return Json(new { currentfemale = currentfemale }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region member chart
        public JsonResult MemberData()
        {
            List<MemberViewModel> Members = new List<MemberViewModel>();

            var results = _memberdatacontext.Members.ToList();
            Member member = new Member();
            MemberViewModel memberView = new MemberViewModel();

            while (results != null)
            {
                memberView.Year = member.YearOfAdmission.Year.ToString();
                memberView.Male = results.Where(s => s.Gender == Gender.Male && s.YearOfAdmission.Year == member.YearOfAdmission.Year).ToArray().Length;
                memberView.Female = results.Where(s => s.Gender == Gender.Female && s.YearOfAdmission.Year == member.YearOfAdmission.Year).ToArray().Length;
            }

            return Json(new { mem = memberView }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region index method
        //
        // GET: /Dashboard/
        public ActionResult Index()
        {
            ViewBag.Members = _memberdatacontext.Members.ToArray().Length;
            ViewBag.Male = _memberdatacontext.Members.Where(member => member.Gender == Gender.Male).ToArray().Length;
            ViewBag.Female = _memberdatacontext.Members.Where(member => member.Gender == Gender.Female).ToArray().Length;
            ViewBag.MemberCurrent = _memberdatacontext.Members.Where(s => (s.YearOfAdmission.Year == DateTime.Now.Year) || s.YearOfAdmission.Year == (DateTime.Now.Year - 1)).ToArray().Length;
            ViewBag.MaleCurrent = _memberdatacontext.Members.Where(s => s.Gender == Gender.Male && (s.YearOfAdmission.Year == DateTime.Now.Year || s.YearOfAdmission.Year == (DateTime.Now.Year - 1))).ToArray().Length;
            ViewBag.FemaleCurrent = _memberdatacontext.Members.Where(s => s.Gender == Gender.Female && (s.YearOfAdmission.Year == DateTime.Now.Year || s.YearOfAdmission.Year == (DateTime.Now.Year - 1))).ToArray().Length;
            return View();
        }
        #endregion
    }
}