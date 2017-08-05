using AIMS.Data.DataContext.DataContext.MemberDataContext;
using AIMS.Data.DataObjects.Entities.Member;
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

        public JsonResult IsMatricNoAvailable(string MatricNumber) {
            return Json(!_memberDataContext.Members.Any(member => member.MatricNumber == MatricNumber), JsonRequestBehavior.AllowGet);
        }

        // GET: /Member/Create
        public ActionResult Create()
        {
            var member = new Member();
            return PartialView("Create", member);
        }

        // POST: /Member/Create
        [HttpPost]
        public ActionResult Create(Member member)
        {
            if (ModelState.IsValid)
            {
                if (_memberDataContext.Members.Any(record => record.MatricNumber == member.MatricNumber))
                {
                    ModelState.AddModelError("Matric", "Matric. number is already is use");
                }
                else
                {
                    var membervar = new Member
                    {
                        FirstName = member.FirstName,
                        Gender = member.Gender,
                        LevelOfAdmission = member.LevelOfAdmission,
                        MatricNumber = member.MatricNumber,
                        MidName = member.MidName,
                        StateOfOrigin = member.StateOfOrigin,
                        Surname = member.Surname,
                        YearOfAdmission = member.YearOfAdmission,

                        CreatedBy = Convert.ToInt64(Session["UserID"]),
                        DateCreated = DateTime.Now,
                        DateLastModified = DateTime.Now,
                        LastModifiedBy = Convert.ToInt64(Session["UserID"])
                    };

                    _memberDataContext.Members.Add(membervar);
                    _memberDataContext.SaveChanges();
                    return Json(new { success = true });
                }
            }
            return PartialView("Create", member);
        }
	}
}