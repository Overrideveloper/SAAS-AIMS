using AIMS.Data.DataContext.DataContext.MeetingDataContext;
using AIMS.Data.DataContext.DataContext.SessionDataContext;
using AIMS.Data.DataObjects.Entities.Meeting;
using AIMS.Data.Enums.Enums.NotificationType;
using AIMS.Data.Enums.Enums.UploadType;
using AIMS.Services.FIleUploader;
using SAAS_AIMS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SAAS_AIMS.Controllers
{
    public class MeetingController : BaseController
    {
        private readonly MeetingDataContext _meetingdatacontext;
        private readonly SessionDataContext _sessiondatacontext;
        private readonly AppUserDataContext _appUserDataContext;
        private string sessionname = null;

        #region constructor
        public MeetingController()
        {
            _meetingdatacontext = new MeetingDataContext();
            _sessiondatacontext = new SessionDataContext();
            _appUserDataContext = new AppUserDataContext();
        }
        #endregion

        #region academic session's meetings index
        //
        // GET: /Meeting/
        [HttpGet]
        [Authorize]
        public ActionResult Index(long sessionid)
        {
            Session["sessionid"] = sessionid;
            var meetings = _meetingdatacontext.Meetings.Where(meeting => meeting.SessionID == sessionid);
            var sess = _sessiondatacontext.Sessions.Find(sessionid);
            TempData["SessTitle"] = sess.Title;
            return View(meetings.OrderBy(meeting => meeting.Date));
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

        #region create meeting
        //
        // GET: /Meeting/Create
        [HttpGet]
        [Authorize]
        public ActionResult Create(long sessionid) 
        {
            Session["sessionid"] = sessionid;
            var meeting = new Meeting();
            return View                                                                                                                                                                                                                                                                                                                                                                                    ("Create", meeting);
        }

        //
        // POST: /Meeting/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Meeting meeting, HttpPostedFileBase file)
        {
            if (file != null)
            {
                var info = new FileInfo(file.FileName);
                if (info.Extension.ToLower() == ".jpg" || info.Extension.ToLower() == ".jpeg" || info.Extension.ToLower() == ".gif"
                || info.Extension.ToLower() == ".png" || info.Extension.ToLower() == ".pdf" || info.Extension.ToLower() == ".docx"
                || info.Extension.ToLower() == ".txt" || info.Extension.ToLower() == ".doc" || info.Extension.ToLower() == ".rtf")
                {
                }
                else
                {
                    ModelState.AddModelError("incompatible", "File format not supported");
                }
            }
            else
            {
                ModelState.AddModelError("null", "Please upload minutes!");
            }

            if (ModelState.IsValid)
            {
                var meetingVar = new Meeting
                {
                    Title = meeting.Title,
                    Date = meeting.Date,
                    Venue = meeting.Venue,
                    Semester = meeting.Semester,
                    SessionID = Convert.ToInt64(Session["sessionid"]),
                    FileUpload = file != null && file.FileName != ""
                            ? new FileUploader().UploadFile(file, UploadType.Minutes)
                            : null,

                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    DateLastModified = DateTime.Now,
                    LastModifiedBy = User.Identity.Name
                };

                _meetingdatacontext.Meetings.Add(meetingVar);
                _meetingdatacontext.SaveChanges();

                var message = new MailMessage();
                message.Priority = MailPriority.High;
                message.From = new MailAddress("no-reply@override.dev", "Override");
                message.Subject = "Meeting Entry Created";

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
                    "</strong> created a meeting entry on " + DateTime.Now.ToLongDateString() + " at " + DateTime.Now.ToLongTimeString() + ".</h4>" +
                    "<p style='font-size: 18px; text-align:justify;'>The meeting entry details are as follows: </p>" +
                    "<ul style='font-size: 18px; text-align:justify;'>" +
                        "<li>Title: " + meetingVar.Title + "</li>" +
                        "<li>Session: " + GetSessionName() + "</li>" +
                        "<li>Venue: " + meetingVar.Venue + "</li></ul>" +
                "<footer style='font-size: 18px; text-align:center;'>" +
                    "<p>&copy;" + DateTime.Now.Year + " Override.</p></footer></div>";

                message.Body = string.Format(emailBody);
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    await smtp.SendMailAsync(message);
                }
                
                TempData["Success"] = "Meeting entry successfully created for " + GetSessionName();
                TempData["NotificationType"] = NotificationType.Create.ToString();
                return RedirectToAction("Index", new { sessionid = Convert.ToInt64(Session["sessionid"])});
            }
            return View("Create", meeting);
        }
        #endregion

        #region edit meeting
        //
        // GET: /Meeting/Edit/id
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Edit(long id)
        {
            var meeting = await _meetingdatacontext.Meetings.FindAsync(id);
            if (meeting == null)
            {
                return HttpNotFound();
            }
            return View("Edit", meeting);
        }

        //
        // POST: /Meeting/Edit/id
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Meeting meeting, HttpPostedFileBase file)
        {
            if (file != null)
            {
                var info = new FileInfo(file.FileName);
                if (info.Extension.ToLower() == ".jpg" || info.Extension.ToLower() == ".jpeg" || info.Extension.ToLower() == ".gif"
                || info.Extension.ToLower() == ".png" || info.Extension.ToLower() == ".pdf" || info.Extension.ToLower() == ".docx"
                || info.Extension.ToLower() == ".txt" || info.Extension.ToLower() == ".doc" || info.Extension.ToLower() == ".rtf")
                {
                }
                else
                {
                    ModelState.AddModelError("incompatible", "File format not supported");
                }
            }

            if (ModelState.IsValid)
            {
                meeting.DateLastModified = DateTime.Now;
                meeting.LastModifiedBy = User.Identity.Name;
                
                if(file != null && file.FileName != ""){
                    meeting.FileUpload = new FileUploader().UploadFile(file, UploadType.Minutes);
                }

                _meetingdatacontext.Entry(meeting).State = EntityState.Modified;
                _meetingdatacontext.SaveChanges();

                var message = new MailMessage();
                message.Priority = MailPriority.High;
                message.From = new MailAddress("no-reply@override.dev", "Override");
                message.Subject = "Meeting Entry Modified";

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
                    "</strong> modified a meeting entry on " + DateTime.Now.ToLongDateString() + " at " + DateTime.Now.ToLongTimeString() + ".</h4>" +
                    "<p style='font-size: 18px; text-align:justify;'>The modified meeting entry details are as follows: </p>" +
                    "<ul style='font-size: 18px; text-align:justify;'>" +
                        "<li>Title: " + meeting.Title + "</li>" +
                        "<li>Session: " + GetSessionName() + "</li>" +
                        "<li>Venue: " + meeting.Venue + "</li></ul>" +
                "<footer style='font-size: 18px; text-align:center;'>" +
                    "<p>&copy;" + DateTime.Now.Year + " Override.</p></footer></div>";

                message.Body = string.Format(emailBody);
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    await smtp.SendMailAsync(message);
                }
                
                TempData["Success"] = "Meeting entry successfully modified for " + GetSessionName();
                TempData["NotificationType"] = NotificationType.Edit.ToString();
                return RedirectToAction("Index", new { sessionid = Convert.ToInt64(Session["sessionid"]) });
            }
            return View("Edit", meeting);
        }
        #endregion

        #region delete meeting
        //
        // DELETE: /Event/Delete/id
        [Authorize]
        public async Task<ActionResult> Delete(long id)
        {
            var meeting = await _meetingdatacontext.Meetings.FindAsync(id);
            if (meeting == null)
            {
                return HttpNotFound();
            }

            _meetingdatacontext.Meetings.Remove(meeting);
            await _meetingdatacontext.SaveChangesAsync();

            var message = new MailMessage();
            message.Priority = MailPriority.High;
            message.From = new MailAddress("no-reply@override.dev", "Override");
            message.Subject = "Meeting Entry Deleted";

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
                "</strong> deleted a meeting entry on " + DateTime.Now.ToLongDateString() + " at " + DateTime.Now.ToLongTimeString() + ".</h4>" +
                "<p style='font-size: 18px; text-align:justify;'>The deleted meeting entry details are as follows: </p>" +
                "<ul style='font-size: 18px; text-align:justify;'>" +
                    "<li>Title: " + meeting.Title + "</li>" +
                    "<li>Session: " + GetSessionName() + "</li>" +
                    "<li>Venue: " + meeting.Venue + "</li></ul>" +
            "<footer style='font-size: 18px; text-align:center;'>" +
                "<p>&copy;" + DateTime.Now.Year + " Override.</p></footer></div>";

            message.Body = string.Format(emailBody);
            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                await smtp.SendMailAsync(message);
            }
                
            TempData["Success"] = "Meeting entry successfully deleted for " + GetSessionName();
            TempData["NotificationType"] = NotificationType.Delete.ToString();
            return RedirectToAction("Index", new { sessionid = Convert.ToInt64(Session["sessionid"]) });
        }
        #endregion
    }
}