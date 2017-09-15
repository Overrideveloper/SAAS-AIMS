using AIMS.Data.DataContext.DataContext.IncomeDataContext;
using AIMS.Data.DataContext.DataContext.SessionDataContext;
using AIMS.Data.DataObjects.Entities.Income;
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
    public class IncomeCategoryController : BaseController
    {
        private readonly IncomeDataContext _incomeDataContext;
        private readonly SessionDataContext _sessionDataContext;
        private readonly AppUserDataContext _appUserDataContext;
        string sessionname = string.Empty;

        #region constructor
        public IncomeCategoryController()
        {
            _incomeDataContext = new IncomeDataContext();
            _sessionDataContext = new SessionDataContext();
            _appUserDataContext = new AppUserDataContext();
        }
        #endregion

        #region get session name
        public string GetSessionName()
        {
            var sess = _sessionDataContext.Sessions.Find(Convert.ToInt64(Session["sessionid"]));
            sessionname = sess.Title;
            return sessionname;
        }
        #endregion

        #region academic session's income category list
        //
        // GET: /IncomeCategory/
        [HttpGet]
        [Authorize]
        public ActionResult Index(long sessionid)
        {
            Session["sessionid"] = sessionid;
            var category = _incomeDataContext.IncomeCategory.Where(s => s.SessionID == sessionid).ToList();
            var sess = _sessionDataContext.Sessions.Find(sessionid);
            TempData["SessTitle"] = sess.Title;
            return View(category.OrderBy(s => s.Title));
        }
        #endregion

        #region create income category
        //
        // GET: /IncomeCategory/Create
        [HttpGet]
        [Authorize]
        public ActionResult Create()
        {
            var category = new IncomeCategory();
            return PartialView("Create", category);
        }

        //
        // POST: /IncomeCategory/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(IncomeCategory category)
        {
            if (ModelState.IsValid)
            {
                var categoryVar = new IncomeCategory
                {
                    Title = category.Title,
                    SessionID = Convert.ToInt64(Session["sessionid"]),

                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    DateLastModified = DateTime.Now,
                    LastModifiedBy = User.Identity.Name
                };

                _incomeDataContext.IncomeCategory.Add(categoryVar);
                _incomeDataContext.SaveChanges();

                var message = new MailMessage();
                message.Priority = MailPriority.High;
                message.From = new MailAddress("no-reply@override.dev", "Override");
                message.Subject = "Income Category Created";

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
                    "</strong> created an income category on " + DateTime.Now.ToLongDateString() + " at " + DateTime.Now.ToLongTimeString() + ".</h4>" +
                    "<p style='font-size: 18px; text-align:justify;'>The income category details are as follows: </p>" +
                    "<ul style='font-size: 18px; text-align:justify;'>" +
                        "<li>Title: " + categoryVar.Title + "</li>" +
                        "<li>Session: " + GetSessionName() + "</li>" +
                "<footer style='font-size: 18px; text-align:center;'>" +
                    "<p>&copy;" + DateTime.Now.Year + " Override.</p></footer></div>";

                message.Body = string.Format(emailBody);
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    await smtp.SendMailAsync(message);
                }

                TempData["Success"] = "Income category successfully created for " + GetSessionName();
                TempData["NotificationType"] = NotificationType.Create.ToString();
                return Json(new { success = true });
            }
            return PartialView("Create", category);
        }
        #endregion

        #region edit income category
        //
        // GET: /IncomeCategory/Edit/id
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Edit(long id)
        {
            var category = await _incomeDataContext.IncomeCategory.FindAsync(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return PartialView("Edit", category);
        }

        //
        // POST: /IncomeCategory/Edit/id
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(IncomeCategory category)
        {
            if (ModelState.IsValid)
            {
                category.DateLastModified = DateTime.Now;
                category.LastModifiedBy = User.Identity.Name;

                _incomeDataContext.Entry(category).State = EntityState.Modified;
                _incomeDataContext.SaveChanges();

                var message = new MailMessage();
                message.Priority = MailPriority.High;
                message.From = new MailAddress("no-reply@override.dev", "Override");
                message.Subject = "Income Category Modified";

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
                    "</strong> modified an income category on " + DateTime.Now.ToLongDateString() + " at " + DateTime.Now.ToLongTimeString() + ".</h4>" +
                    "<p style='font-size: 18px; text-align:justify;'>The modified income category details are as follows: </p>" +
                    "<ul style='font-size: 18px; text-align:justify;'>" +
                        "<li>Title: " + category.Title + "</li>" +
                        "<li>Session: " + GetSessionName() + "</li>" +
                "<footer style='font-size: 18px; text-align:center;'>" +
                    "<p>&copy;" + DateTime.Now.Year + " Override.</p></footer></div>";

                message.Body = string.Format(emailBody);
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    await smtp.SendMailAsync(message);
                }

                TempData["Success"] = "Income category successfully modified for " + GetSessionName();
                TempData["NotificationType"] = NotificationType.Edit.ToString();
                return Json(new { success = true });
            }
            return PartialView("Edit", category);
        }
        #endregion

        #region delete income category
        //
        // DELETE: /IncomeCategory/Delete/id
        [Authorize]
        public async Task<ActionResult> Delete(long id)
        {
            var category = await _incomeDataContext.IncomeCategory.FindAsync(id);
            _incomeDataContext.IncomeCategory.Remove(category);
            await _incomeDataContext.SaveChangesAsync();

            var message = new MailMessage();
            message.Priority = MailPriority.High;
            message.From = new MailAddress("no-reply@override.dev", "Override");
            message.Subject = "Income Category Deleted";

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
                "</strong> deleted an income category on " + DateTime.Now.ToLongDateString() + " at " + DateTime.Now.ToLongTimeString() + ".</h4>" +
                "<p style='font-size: 18px; text-align:justify;'>The deleted income category details are as follows: </p>" +
                "<ul style='font-size: 18px; text-align:justify;'>" +
                    "<li>Title: " + category.Title + "</li>" +
                    "<li>Session: " + GetSessionName() + "</li>" +
            "<footer style='font-size: 18px; text-align:center;'>" +
                "<p>&copy;" + DateTime.Now.Year + " Override.</p></footer></div>";

            message.Body = string.Format(emailBody);
            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                await smtp.SendMailAsync(message);
            }

            TempData["Success"] = "Income category successfully deleted for " + GetSessionName();
            TempData["NotificationType"] = NotificationType.Delete.ToString();
            return RedirectToAction("Index", new { sessionid = Convert.ToInt64(Session["sessionid"]) });
        }
        #endregion
    }
}