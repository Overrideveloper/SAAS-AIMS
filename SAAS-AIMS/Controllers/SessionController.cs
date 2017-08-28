using AIMS.Data.DataContext.DataContext.SessionDataContext;
using AIMS.Data.DataObjects.Entities.Session;
using AIMS.Data.Enums.Enums.NotificationType;
using SAAS_AIMS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SAAS_AIMS.Controllers
{
    public class SessionController : BaseController
    {
        private readonly SessionDataContext _sessionDataContext;
        private readonly AppUserDataContext _appUserDataContext;

        #region constructor
        public SessionController()
        {
            _sessionDataContext = new SessionDataContext();
            _appUserDataContext = new AppUserDataContext();
        }
        #endregion

        #region academic session index
        //
        // GET: /Session/
        [HttpGet]
        [Authorize]
        public ActionResult Index()
        {
            var session = from m in _sessionDataContext.Sessions
                          select m;

            return View(session.OrderBy(order => order.ID));
        }
        #endregion

        #region create academic session
        //
        // GET: /Session/Create
        [HttpGet]
        [Authorize]
        public ActionResult Create()
        {
            var session = new Session();
            return PartialView("Create", session);
        }

        //
        // POST: /Session/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Session session)
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

                var message = new MailMessage();
                message.Priority = MailPriority.High;
                message.From = new MailAddress("no-reply@override.dev", "Override");

                var superusers = _appUserDataContext.Users.Include(s => s.Role).Where(s => s.Role.Title == "Superuser").ToList();

                foreach(var superuser in superusers)
                {
                    message.Bcc.Add(new MailAddress(superuser.Email));
                }
                              
                var emailBody = "<strong><h1> Association Information Management System</h1> by Override</strong><br/><br/><br/>";
                message.Subject = "New Academic Session Created";
                var text = "<p> The user with the user name <strong>" + User.Identity.Name + "</strong> added a new academic session to the association management suite on " + DateTime.Now.ToLongDateString() + " at " + DateTime.Now.ToLongTimeString() + ".</p> <br/>"; 
                var details = "<p> <strong>Session Details: </strong></p> " + "Title: " + sessionvar.Title + "<br/> Start Date: " + sessionvar.StartDate.ToLongDateString() + "<br/> End Date: " + sessionvar.EndDate.ToLongDateString() + " </p>";
                message.Body = string.Format(emailBody + text + details);
                message.IsBodyHtml = true;
                
                using (var smtp = new SmtpClient())
                {
                    await smtp.SendMailAsync(message);
                } 

                TempData["Success"] = "Academic session successfully created! ";
                TempData["NotificationType"] = NotificationType.Create.ToString();
                return Json(new { success = true });
            }

            return PartialView("Create", session);
        }
#endregion

        #region edit academic session
        //
        // GET: /Session/Edit
        [HttpGet]
        [Authorize]
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
        [Authorize]
        [ValidateAntiForgeryToken]
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
#endregion

        #region delete academic session
        //
        // DELETE: /Session/Delete/id
        [Authorize]
        public async Task<ActionResult> Delete(long id)
        {
            var session = await _sessionDataContext.Sessions.FindAsync(id);
            _sessionDataContext.Sessions.Remove(session);
            await _sessionDataContext.SaveChangesAsync();
            TempData["Success"] = "Academic session successfully deleted!";
            TempData["NotificationType"] = NotificationType.Delete.ToString();
            return RedirectToAction("Index");
        }
        #endregion
    }
}