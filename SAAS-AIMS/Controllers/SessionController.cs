using AIMS.Data.DataContext.DataContext.SessionDataContext;
using AIMS.Data.DataObjects.Entities.Session;
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
    public class SessionController : Controller
    {
        private SessionDataContext _sessionDataContext;

        public SessionController()
        {
            _sessionDataContext = new SessionDataContext();
        }

        //
        // GET: /Session/
        public ActionResult Index()
        {
            var session = from m in _sessionDataContext.Sessions
                          select m;

            return View(session);
        }

        //
        // GET: /Session/Create
        public ActionResult Create()
        {
            var session = new Session();
            return PartialView("Create", session);
        }

        //
        // POST: /Session/Create
        [HttpPost]
        public ActionResult Create(Session session)
        {
            if (ModelState.IsValid)
            {
                var sessionvar = new Session
                {
                    Title = session.Title,
                    StartDate = session.StartDate,
                    EndDate = session.EndDate,
                    CreatedBy = Convert.ToInt64(Session["UserID"]),
                    DateCreated = DateTime.Now,
                    DateLastModified = DateTime.Now,
                    LastModifiedBy = Convert.ToInt64(Session["UserID"])
                };

                _sessionDataContext.Sessions.Add(sessionvar);
                _sessionDataContext.SaveChanges();

                TempData["Success"] = "Academic session successfully created! ";
                TempData["NotificationType"] = NotificationType.Create.ToString();
                return Json(new { success = true });
            }

            return PartialView("Create", session);
        }

        //
        // GET: /Session/Edit
        [HttpGet]
        public async Task<ActionResult> Edit(long id)
        {
            var session = await _sessionDataContext.Sessions.FindAsync(id);
            if (session == null) 
            {
                return HttpNotFound();
            }
            return PartialView("Edit", session);
        }

        //
        // POST: /Session/Edit
        [HttpPost]
        public ActionResult Edit(Session session)
        {
            if (ModelState.IsValid)
            {
                session.DateLastModified = DateTime.Now;
                session.LastModifiedBy = Convert.ToInt64(Session["UserID"]);

                _sessionDataContext.Entry(session).State = EntityState.Modified;
                _sessionDataContext.SaveChanges();

                TempData["Success"] = " Academic session successfully modified! ";
                TempData["NotificationType"] = NotificationType.Edit.ToString();
                return Json(new { success = true});
            }
            return PartialView("Edit", session);
        }

        public async Task<ActionResult> Delete(long id)
        {
            var session = await _sessionDataContext.Sessions.FindAsync(id);
            _sessionDataContext.Sessions.Remove(session);
            _sessionDataContext.SaveChanges();
            TempData["Success"] = "Academic session successfully deleted!";
            TempData["NotificationType"] = NotificationType.Delete.ToString();
            return RedirectToAction("Index");
        }
	}
}