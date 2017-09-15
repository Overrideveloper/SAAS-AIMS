using AIMS.Data.DataContext.DataContext.MemoDataContext;
using AIMS.Data.DataContext.DataContext.SessionDataContext;
using AIMS.Data.DataObjects.Entities.Memo;
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
    public class MemoController : BaseController
    {
        private readonly MemoDataContext _memoDataContext;
        private readonly SessionDataContext _sessionDataContext;
        private readonly AppUserDataContext _appUserDataContext;

        #region constructor
        public MemoController()
        {
            _memoDataContext = new MemoDataContext();
            _sessionDataContext = new SessionDataContext();
            _appUserDataContext = new AppUserDataContext();
        }
        #endregion

        #region Academic session's memo index
        //
        // GET: /Memo/
        [HttpGet]
        [Authorize]
        public ActionResult Index(long sessionid)
        {
            Session["sessionid"] = sessionid;
            var memo = _memoDataContext.Memos.Where(s => s.SessionID == sessionid).ToList() ;
            var sess = _sessionDataContext.Sessions.Find(sessionid);
            TempData["sess"] = sess.Title;
            return View("Index", memo.OrderBy(s => s.Date));
        }
        #endregion

        #region get session name
        public string GetSessionName()
        {
            var sess = _sessionDataContext.Sessions.Find(Convert.ToInt64(Session["sessionid"]));
            string name = sess.Title;
            return name;
        }
        #endregion

        #region create memo entry
        //
        // GET: /Memo/Create
        [HttpGet]
        [Authorize]
        public ActionResult Create(long sessionid)
        {
            Session["sessionid"] = sessionid;
            var memo = new Memo();
            return View("Create", memo);
        }

        //
        // POST: /Memo/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Memo memo, HttpPostedFileBase file)  
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
                ModelState.AddModelError("empty", "Please upload memo!");
            }

            if(ModelState.IsValid)
            {
                var memoVar = new Memo
                {
                    Type = memo.Type,
                    Description = memo.Description,
                    Date = memo.Date,
                    FileUpload = file != null && file.FileName != ""
                            ? new FileUploader().UploadFile(file, UploadType.Memos)
                            : null,
                    SessionID = Convert.ToInt64(Session["sessionid"]),
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    DateLastModified = DateTime.Now,
                    LastModifiedBy = User.Identity.Name
                };

                _memoDataContext.Memos.Add(memoVar);
                _memoDataContext.SaveChanges();

                var message = new MailMessage();
                message.Priority = MailPriority.High;
                message.From = new MailAddress("no-reply@override.dev", "Override");
                message.Subject = "Memo Entry Created";

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
                    "</strong> created a memo entry on " + DateTime.Now.ToLongDateString() + " at " + DateTime.Now.ToLongTimeString() + ".</h4>" +
                    "<p style='font-size: 18px; text-align:justify;'>The memo entry details are as follows: </p>" +
                    "<ul style='font-size: 18px; text-align:justify;'>" +
                        "<li>Description: " + memoVar.Description + "</li>" +
                        "<li>Session: " + GetSessionName() + "</li>" +
                        "<li>Date: " + memoVar.Date.ToLongDateString() + "</li></ul>" +
                "<footer style='font-size: 18px; text-align:center;'>" +
                    "<p>&copy;" + DateTime.Now.Year + " Override.</p></footer></div>";

                message.Body = string.Format(emailBody);
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    await smtp.SendMailAsync(message);
                }
                
                TempData["Success"] = "Memo entry successfully created for " + GetSessionName();
                TempData["NotificationType"] = NotificationType.Create.ToString();
                return RedirectToAction("Index", new { sessionid = Convert.ToInt64(Session["sessionid"]) });
            }
            return View("Create", memo);
        }
        #endregion

        #region edit memo entry
        //
        // GET: /Memo/Edit/id
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Edit(long id)
        {
            var memo = await _memoDataContext.Memos.FindAsync(id);
            if (memo == null)
            {
                return HttpNotFound();
            }
            return View("Edit", memo);
        }

        //
        // POST: /Memo/Edit/id
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Memo memo, HttpPostedFileBase file)
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
                memo.DateLastModified = DateTime.Now;
                memo.LastModifiedBy = User.Identity.Name;

                if (file != null && file.FileName != "")
                {
                    memo.FileUpload = new FileUploader().UploadFile(file, UploadType.Memos);
                }

                _memoDataContext.Entry(memo).State = EntityState.Modified;
                _memoDataContext.SaveChanges();

                var message = new MailMessage();
                message.Priority = MailPriority.High;
                message.From = new MailAddress("no-reply@override.dev", "Override");
                message.Subject = "Memo Entry Modified";

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
                    "</strong> modified a memo entry on " + DateTime.Now.ToLongDateString() + " at " + DateTime.Now.ToLongTimeString() + ".</h4>" +
                    "<p style='font-size: 18px; text-align:justify;'>The modified memo entry details are as follows: </p>" +
                    "<ul style='font-size: 18px; text-align:justify;'>" +
                        "<li>Description: " + memo.Description + "</li>" +
                        "<li>Session: " + GetSessionName() + "</li>" +
                        "<li>Date: " + memo.Date.ToLongDateString() + "</li></ul>" +
                "<footer style='font-size: 18px; text-align:center;'>" +
                    "<p>&copy;" + DateTime.Now.Year + " Override.</p></footer></div>";

                message.Body = string.Format(emailBody);
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    await smtp.SendMailAsync(message);
                }
                
                TempData["Success"] = "Memo entry successfully modified for " + GetSessionName();
                TempData["NotificationType"] = NotificationType.Edit.ToString();
                return RedirectToAction("Index", new { sessionid = Convert.ToInt64(Session["sessionid"]) });
            }
            return View("Edit", memo);
        }
        #endregion

        #region delete memo
        public async Task<ActionResult> Delete(long id)
        {
            var memo = await _memoDataContext.Memos.FindAsync(id);
            if (memo == null)
            {
                return HttpNotFound();
            }
            _memoDataContext.Memos.Remove(memo);
            await _memoDataContext.SaveChangesAsync();

            var message = new MailMessage();
            message.Priority = MailPriority.High;
            message.From = new MailAddress("no-reply@override.dev", "Override");
            message.Subject = "Memo Entry Deleted";

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
                "</strong> deleted a memo entry on " + DateTime.Now.ToLongDateString() + " at " + DateTime.Now.ToLongTimeString() + ".</h4>" +
                "<p style='font-size: 18px; text-align:justify;'>The deleted memo entry details are as follows: </p>" +
                "<ul style='font-size: 18px; text-align:justify;'>" +
                    "<li>Description: " + memo.Description + "</li>" +
                    "<li>Session: " + GetSessionName() + "</li>" +
                    "<li>Date: " + memo.Date.ToLongDateString() + "</li></ul>" +
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