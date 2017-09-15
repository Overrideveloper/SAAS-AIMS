using AIMS.Data.DataContext.DataContext.EventDataContext;
using AIMS.Data.DataContext.DataContext.SessionDataContext;
using AIMS.Data.DataObjects.Entities.Event;
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
    public class EventController : BaseController
    {
        private readonly EventDataContext _eventdatacontext;
        private readonly SessionDataContext _sessiondatacontext;
        private readonly AppUserDataContext _appUserDataContext;
        private string sessionname = null;

        #region constructor
        public EventController()
        {
            _eventdatacontext = new EventDataContext();
            _sessiondatacontext = new SessionDataContext();
            _appUserDataContext = new AppUserDataContext();
        }
        #endregion

        #region get session name
        public string GetSessionName()
        {
            var session = _sessiondatacontext.Sessions.Find(Convert.ToInt64(Session["sessionid"]));
            sessionname = session.Title.ToString();
            return sessionname;
        }
        #endregion

        #region academic session's event index
        //
        // GET: /Event/
        [HttpGet]
        [Authorize]
        public ActionResult Index(long sessionid )
        {
            Session["sessionid"] = sessionid;
            var events = _eventdatacontext.Event.Where(var => var.SessionID == sessionid);
            return View(events.OrderBy(var => var.Date));
        }
        #endregion

        #region create event
        //
        // GET: /Event/Create
        [HttpGet]
        [Authorize]
        public ActionResult Create(long sessionid)
        {
            var events = new Event();
            return PartialView("Create", events);
        }

        //
        // POST: /Event/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Event events)
        {
            if(ModelState.IsValid)
            {
                var eventVar = new Event
                {
                    Title = events.Title,
                    Venue = events.Venue,
                    SessionID = Convert.ToInt64(Session["sessionid"]),
                    Semester = events.Semester,
                    Date = events.Date,

                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    DateLastModified = DateTime.Now,
                    LastModifiedBy = User.Identity.Name
                };

                _eventdatacontext.Event.Add(eventVar);
                _eventdatacontext.SaveChanges();

                var message = new MailMessage();
                message.Priority = MailPriority.High;
                message.From = new MailAddress("no-reply@override.dev", "Override");
                message.Subject = "Event Created";

                var superusers = _appUserDataContext.Users.Include(s => s.Role).Where(s => s.Role.Title == "Superuser").ToList();

                foreach (var superuser in superusers)
                {
                    message.Bcc.Add(new MailAddress(superuser.Email));
                }

                var emailBody =
                "<div>" +
                "<h3 style='font-size: 30px; text-align:center;'><strong>ASSOCIATION INFORMATION MANAGEMENT SYSTEM</strong></h3>" +
                "<div style='position: relative; min-height: 1px; padding-right: 15px; padding-left: 15px; padding-top: 5px;'>" +
                    "<h4 style='font-size: 18px; text-align:justify;'>The user: <strong>" + User.Identity.Name +
                    "</strong> created an event on " + DateTime.Now.ToLongDateString() + " at " + DateTime.Now.ToLongTimeString() + ".</h4>" +
                    "<p style='font-size: 18px; text-align:justify;'>The event details are as follows: </p>" +
                    "<ul style='font-size: 18px; text-align:justify;'>" +
                        "<li>Title: " + eventVar.Title + "</li>" +
                        "<li>Session: " + GetSessionName() + "</li>" +
                        "<li>Date: " + eventVar.Date.ToLongDateString() + "</li>" +
                        "<li>Venue: " + eventVar.Venue + "</li><ul></div><br>" +
                "<footer style='font-size: 18px; text-align:center;'>" +
                    "<p>&copy;" + DateTime.Now.Year + " Override.</p></footer></div>";

                message.Body = string.Format(emailBody);
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    await smtp.SendMailAsync(message);
                }

                TempData["Success"] = "Event entry successfully created for " + GetSessionName();
                TempData["NotificationType"] = NotificationType.Create.ToString();
                return Json(new { success = true});
            }
            return PartialView("Create", events);
        }
        #endregion

        #region edit event
        //
        // GET: /Event/Edit/id
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Edit(long id)
        {
            var events = await _eventdatacontext.Event.FindAsync(id);
            if (events == null)
            {
                return HttpNotFound();
            }
            return PartialView("Edit", events);
        }

        //
        // POST: /Event/Edit/id
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Event events)
        {
            if(ModelState.IsValid)
            {
                events.DateLastModified = DateTime.Now;
                events.LastModifiedBy = User.Identity.Name;
                events.SessionID = Convert.ToInt64(Session["sessionid"]);

                _eventdatacontext.Entry(events).State = EntityState.Modified;
                _eventdatacontext.SaveChanges();

                var message = new MailMessage();
                message.Priority = MailPriority.High;
                message.From = new MailAddress("no-reply@override.dev", "Override");
                message.Subject = "Event Modified";

                var superusers = _appUserDataContext.Users.Include(s => s.Role).Where(s => s.Role.Title == "Superuser").ToList();

                foreach (var superuser in superusers)
                {
                    message.Bcc.Add(new MailAddress(superuser.Email));
                }

                var emailBody =
                "<div>" +
                "<h3 style='font-size: 30px; text-align:center;'><strong>ASSOCIATION INFORMATION MANAGEMENT SYSTEM</strong></h3>" +
                "<div style='position: relative; min-height: 1px; padding-right: 15px; padding-left: 15px; padding-top: 5px;'>" +
                    "<h4 style='font-size: 18px; text-align:justify;'>The user: <strong>" + User.Identity.Name +
                    "</strong> modified an event on " + DateTime.Now.ToLongDateString() + " at " + DateTime.Now.ToLongTimeString() + ".</h4>" +
                    "<p style='font-size: 18px; text-align:justify;'>The modified event details are as follows: </p>" +
                    "<ul style='font-size: 18px; text-align:justify;'>" +
                        "<li>Title: " + events.Title + "</li>" +
                        "<li>Session: " + GetSessionName() + "</li>" +
                        "<li>Date: " + events.Date.ToLongDateString() + "</li>" +
                        "<li>Venue: " + events.Venue + "</li><ul></div><br>" +
                "<footer style='font-size: 18px; text-align:center;'>" +
                    "<p>&copy;" + DateTime.Now.Year + " Override.</p></footer></div>";

                message.Body = string.Format(emailBody);
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    await smtp.SendMailAsync(message);
                }

                TempData["Success"] = "Event entry successfully modified for " + GetSessionName();
                TempData["NotificationType"] = NotificationType.Edit.ToString();
                return Json(new { success = true });
            }
            return PartialView("Edit", events);
        }

        #endregion

        #region delete event
        //
        // DELETE: /Event/Delete/id
        [Authorize]
        public async Task<ActionResult> Delete(long id)
        {
            var events = await _eventdatacontext.Event.FindAsync(id);
            _eventdatacontext.Event.Remove(events);
            await _eventdatacontext.SaveChangesAsync();

            var message = new MailMessage();
            message.Priority = MailPriority.High;
            message.From = new MailAddress("no-reply@override.dev", "Override");
            message.Subject = "Event Deleted";

            var superusers = _appUserDataContext.Users.Include(s => s.Role).Where(s => s.Role.Title == "Superuser").ToList();

            foreach (var superuser in superusers)
            {
                message.Bcc.Add(new MailAddress(superuser.Email));
            }

            var emailBody =
            "<div>" +
            "<h3 style='font-size: 30px; text-align:center;'><strong>ASSOCIATION INFORMATION MANAGEMENT SYSTEM</strong></h3>" +
            "<div style='position: relative; min-height: 1px; padding-right: 15px; padding-left: 15px; padding-top: 5px;'>" +
                "<h4 style='font-size: 18px; text-align:justify;'>The user: <strong>" + User.Identity.Name +
                "</strong> deleted an event on " + DateTime.Now.ToLongDateString() + " at " + DateTime.Now.ToLongTimeString() + ".</h4>" +
                "<p style='font-size: 18px; text-align:justify;'>The deleted event details are as follows: </p>" +
                "<ul style='font-size: 18px; text-align:justify;'>" +
                    "<li>Title: " + events.Title + "</li>" +
                    "<li>Session: " + GetSessionName() + "</li>" +
                    "<li>Date: " + events.Date.ToLongDateString() + "</li>" +
                    "<li>Venue: " + events.Venue + "</li><ul></div><br>" +
            "<footer style='font-size: 18px; text-align:center;'>" +
                "<p>&copy;" + DateTime.Now.Year + " Override.</p></footer></div>";

            message.Body = string.Format(emailBody);
            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                await smtp.SendMailAsync(message);
            }

            TempData["Success"] = "Event entry successfully deleted for " + GetSessionName();
            TempData["NotificationType"] = NotificationType.Delete.ToString();
            return RedirectToAction("Index", new { sessionid = Convert.ToInt64(Session["sessionid"]) });
        }
        #endregion
    }
}