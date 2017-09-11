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

        #endregion

        #region index method
        //
        // GET: /Dashboard/
        public ActionResult Index()
        {
            ViewBag.Members = _memberdatacontext.Members.ToArray().Length;
            ViewBag.Male = _memberdatacontext.Members.Where(member => member.Gender == Gender.Male).ToArray().Length;
            ViewBag.Female = _memberdatacontext.Members.Where(member => member.Gender == Gender.Female).ToArray().Length;
            return View();
        }
        #endregion
    }
}