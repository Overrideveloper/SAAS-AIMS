using AIMS.Data.DataContext.DataContext.MemberDataContext;
using AIMS.Data.DataObjects.Entities.Member;
using AIMS.Data.Enums.Enums.NotificationType;
using AIMS.Data.Enums.Enums.State;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SAAS_AIMS.Controllers
{
    public class MemberController : BaseController
    {
        
        private readonly MemberDataContext _memberDataContext;

        #region constructor
        public MemberController()
        {
            _memberDataContext = new MemberDataContext();
        }
        #endregion

        #region association member index
        //
        // GET: /Member/
        [HttpGet]
        [Authorize]
        public ActionResult Index()
        {
            var member = from m in _memberDataContext.Members
                             select m;
            return View(member.OrderBy(order => order.Surname));
        }
        #endregion

        #region matric number remote validation
        [Authorize]
        public JsonResult IsMatricNoAvailable(string MatricNumber) {
            return Json(!_memberDataContext.Members.Any(member => member.MatricNumber == MatricNumber), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region create association member
        // GET: /Member/Create
        [HttpGet]
        [Authorize]
        public ActionResult Create()
        {
            var member = new Member();
            return PartialView("Create", member);
        }

        // POST: /Member/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
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
                    TempData["Success"] = "Association member successfully created! ";
                    TempData["NotificationType"] = NotificationType.Create.ToString();
                    return Json(new { success = true });
                }
            }
            return PartialView("Create", member);
        }
        #endregion

        #region edit association member
        //
        // GET: /Member/Edit/id
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Edit(long id)
        {
            var member = await _memberDataContext.Members.FindAsync(id);
            if (member == null)
            {
                return HttpNotFound();
            }
            return PartialView("Edit", member);
        }

        //
        // POST: /Member/Edit/id
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Member member)
        {
            if (ModelState.IsValid)
            {
                    member.DateLastModified = DateTime.Now;
                    member.LastModifiedBy = Convert.ToInt64(Session["UserID"]);

                    _memberDataContext.Entry(member).State = EntityState.Modified;
                    await _memberDataContext.SaveChangesAsync();

                    TempData["Success"] = "Association member successfully modified! ";
                    TempData["NotificationType"] = NotificationType.Create.ToString();
                    return Json(new { success = true });
            }
            return PartialView("Edit", member);
        }

        #endregion

        #region delete association member
        //
        // DELETE: /Member/Delete/id
        [Authorize]
        public async Task<ActionResult> Delete(long id)
        {
            var member = await _memberDataContext.Members.FindAsync(id);
            _memberDataContext.Members.Remove(member);
            await _memberDataContext.SaveChangesAsync();

            TempData["Success"] = "Association member successfully deleted!";
            TempData["NotificationType"] = NotificationType.Delete.ToString();
            return RedirectToAction("Index");
        }
        #endregion
    }
}