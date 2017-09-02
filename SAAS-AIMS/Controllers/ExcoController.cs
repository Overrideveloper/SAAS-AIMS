using AIMS.Data.DataContext.DataContext.ExcoDataContext;
using AIMS.Data.DataContext.DataContext.MemberDataContext;
using AIMS.Data.DataContext.DataContext.SessionDataContext;
using AIMS.Data.DataObjects.Entities.Exco;
using AIMS.Data.Enums.Enums.NotificationType;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SAAS_AIMS.Controllers
{
    public class ExcoController : BaseController
    {
        private readonly ExcoDataContext _excoDataContext;
        private readonly SessionDataContext _sessionDataContext;
        private readonly MemberDataContext _memberDataContext;
        private string sessionname = string.Empty;

        #region constructor
        public ExcoController()
        {
            _excoDataContext = new ExcoDataContext();
            _sessionDataContext = new SessionDataContext();
            _memberDataContext = new MemberDataContext();
        }
        #endregion

        #region get session name
        public string GetSessionName()
        {
            var session = _sessionDataContext.Sessions.Find(Convert.ToInt64(Session["sessionid"]));
            sessionname = session.Title.ToString();
            return sessionname;
        }
        #endregion

        #region academic session's excos
        //
        // GET: /Exco/
        [HttpGet]
        [Authorize]
        public ActionResult Index(long sessionid )
        {
            Session["sessionid"] = sessionid;
            var exco = _excoDataContext.Exco.Where(var => var.SessionID == sessionid);
            var sess = _sessionDataContext.Sessions.Find(sessionid);
            TempData["sess"] = sess.Title;
            return View(exco.OrderBy(var => var.LastName));
        }
        #endregion

        #region get member details
        public JsonResult ExcoDetails(string matric)
        {
            var exco = _memberDataContext.Members.Where(s => s.MatricNumber == matric);
            return Json(exco.SingleOrDefault(), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region create exco
        //
        // GET: /Exco/Create
        [HttpGet]
        [Authorize]
        public ActionResult Create()
        {
            var exco = new Exco();
            return PartialView("Create", exco);
        }

        //
        // POST: /Exco/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Exco exco)
        {
            if(ModelState.IsValid)
            {
                var excoVar = new Exco
                {
                    MatricNo = exco.MatricNo,
                    LastName = exco.LastName,
                    FirstName = exco.FirstName,
                    Post = exco.Post,
                    SessionID = Convert.ToInt64(Session["sessionid"]),
                    
                    CreatedBy = Convert.ToInt64(Session["UserID"]),
                    DateCreated = DateTime.Now,
                    DateLastModified = DateTime.Now,
                    LastModifiedBy = Convert.ToInt64(Session["UserID"])
                };

                _excoDataContext.Exco.Add(excoVar);
                _excoDataContext.SaveChanges();
                TempData["Success"] = "Exco added for " + GetSessionName();
                TempData["NotificationType"] = NotificationType.Create.ToString();
                return Json(new { success = true});
            }
            return PartialView("Create", exco);
        }
        #endregion

        #region edit exco
        //
        // GET: /Exco/Edit/id
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Edit(long id)
        {
            var exco = await _excoDataContext.Exco.FindAsync(id);
            if (exco == null)
            {
                return HttpNotFound();
            }
            return PartialView("Edit", exco);
        }

        //
        // POST: /Exco/Edit/id
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Exco exco)
        {
            if(ModelState.IsValid)
            {
                exco.DateLastModified = DateTime.Now;
                exco.LastModifiedBy = Convert.ToInt64(Session["UserID"]);
                exco.SessionID = Convert.ToInt64(Session["sessionid"]);

                _excoDataContext.Entry(exco).State = EntityState.Modified;
                _excoDataContext.SaveChanges();

                TempData["Success"] = "Exco modified for " + GetSessionName();
                TempData["NotificationType"] = NotificationType.Edit.ToString();
                return Json(new { success = true });
            }
            return PartialView("Edit", exco);
        }

        #endregion

        #region delete exco
        //
        // DELETE: /Event/Delete/id
        [Authorize]
        public async Task<ActionResult> Delete(long id)
        {
            var events = await _excoDataContext.Exco.FindAsync(id);
            _excoDataContext.Exco.Remove(events);
            await _excoDataContext.SaveChangesAsync();

            TempData["Success"] = "Exco deleted for " + GetSessionName();
            TempData["NotificationType"] = NotificationType.Delete.ToString();
            return RedirectToAction("Index", new { sessionid = Convert.ToInt64(Session["sessionid"]) });
        }
        #endregion
	}
}