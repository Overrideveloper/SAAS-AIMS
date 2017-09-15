using AIMS.Data.DataContext.DataContext.ExpenseDataContext;
using AIMS.Data.DataContext.DataContext.IncomeDataContext;
using AIMS.Data.DataContext.DataContext.SessionDataContext;
using AIMS.Data.DataObjects.Entities.Expense;
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
    public class ExpenseCategoryController : BaseController
    {
        private readonly ExpenseDataContext _expenseDataContext;
        private readonly SessionDataContext _sessionDataContext;
        private readonly AppUserDataContext _appUserDataContext;
        string sessionname = string.Empty;

        #region constructor
        public ExpenseCategoryController()
        {
            _expenseDataContext = new ExpenseDataContext();
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

        #region academic session's expense category list
        //
        // GET: /ExpenseCategory/
        [HttpGet]
        [Authorize]
        public ActionResult Index(long sessionid)
        {
            Session["sessionid"] = sessionid;
            var category = _expenseDataContext.ExpenseCategory.Where(s => s.SessionID == sessionid).ToList();
            var sess = _sessionDataContext.Sessions.Find(sessionid);
            TempData["SessTitle"] = sess.Title;
            return View(category.OrderBy(s => s.Title));
        }
        #endregion

        #region create expense category
        //
        // GET: /ExpenseCategory/Create
        [HttpGet]
        [Authorize]
        public ActionResult Create()
        {
            var category = new ExpenseCategory();
            return PartialView("Create", category);
        }

        //
        // POST: /ExpenseCategory/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ExpenseCategory category)
        {
            if (ModelState.IsValid)
            {
                var categoryVar = new ExpenseCategory
                {
                    Title = category.Title,
                    SessionID = Convert.ToInt64(Session["sessionid"]),

                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    DateLastModified = DateTime.Now,
                    LastModifiedBy = User.Identity.Name
                };

                _expenseDataContext.ExpenseCategory.Add(categoryVar);
                _expenseDataContext.SaveChanges();

                var message = new MailMessage();
                message.Priority = MailPriority.High;
                message.From = new MailAddress("no-reply@override.dev", "Override");
                message.Subject = "Expense Category Created";

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
                    "</strong> created an expense category on " + DateTime.Now.ToLongDateString() + " at " + DateTime.Now.ToLongTimeString() + ".</h4>" +
                    "<p style='font-size: 18px; text-align:justify;'>The expense category details are as follows: </p>" +
                    "<ul style='font-size: 18px; text-align:justify;'>" +
                        "<li>Title: " + categoryVar.Title + "</li>" +
                        "<li>Session: " + GetSessionName() + "</li></ul>" +
                "<footer style='font-size: 18px; text-align:center;'>" +
                    "<p>&copy;" + DateTime.Now.Year + " Override.</p></footer></div>";

                message.Body = string.Format(emailBody);
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    await smtp.SendMailAsync(message);
                }

                TempData["Success"] = "Expense category successfully created for " + GetSessionName();
                TempData["NotificationType"] = NotificationType.Create.ToString();
                return Json(new { success = true });
            }
            return PartialView("Create", category);
        }
        #endregion

        #region edit expense category
        //
        // GET: /ExpenseCategory/Edit/id
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Edit(long id)
        {
            var category = await _expenseDataContext.ExpenseCategory.FindAsync(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return PartialView("Edit", category);
        }

        //
        // POST: /ExpenseCategory/Edit/id
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ExpenseCategory category)
        {
            if (ModelState.IsValid)
            {
                category.DateLastModified = DateTime.Now;
                category.LastModifiedBy = User.Identity.Name;

                _expenseDataContext.Entry(category).State = EntityState.Modified;
                _expenseDataContext.SaveChanges();


                var message = new MailMessage();
                message.Priority = MailPriority.High;
                message.From = new MailAddress("no-reply@override.dev", "Override");
                message.Subject = "Expense Category Modified";

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
                    "</strong> modified an expense category on " + DateTime.Now.ToLongDateString() + " at " + DateTime.Now.ToLongTimeString() + ".</h4>" +
                    "<p style='font-size: 18px; text-align:justify;'>The modified expense category details are as follows: </p>" +
                    "<ul style='font-size: 18px; text-align:justify;'>" +
                        "<li>Title: " + category.Title + "</li>" +
                        "<li>Session: " + GetSessionName() + "</li></ul>" +
                "<footer style='font-size: 18px; text-align:center;'>" +
                    "<p>&copy;" + DateTime.Now.Year + " Override.</p></footer></div>";

                message.Body = string.Format(emailBody);
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    await smtp.SendMailAsync(message);
                }

                TempData["Success"] = "Expense category successfully modified for " + GetSessionName();
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
            var category = await _expenseDataContext.ExpenseCategory.FindAsync(id);
            _expenseDataContext.ExpenseCategory.Remove(category);
            await _expenseDataContext.SaveChangesAsync();


            var message = new MailMessage();
            message.Priority = MailPriority.High;
            message.From = new MailAddress("no-reply@override.dev", "Override");
            message.Subject = "Expense Category Deleted";

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
                "</strong> deleted an expense category on " + DateTime.Now.ToLongDateString() + " at " + DateTime.Now.ToLongTimeString() + ".</h4>" +
                "<p style='font-size: 18px; text-align:justify;'>The deleted expense category details are as follows: </p>" +
                "<ul style='font-size: 18px; text-align:justify;'>" +
                    "<li>Title: " + category.Title + "</li>" +
                    "<li>Session: " + GetSessionName() + "</li></ul>" +
            "<footer style='font-size: 18px; text-align:center;'>" +
                "<p>&copy;" + DateTime.Now.Year + " Override.</p></footer></div>";

            message.Body = string.Format(emailBody);
            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                await smtp.SendMailAsync(message);
            }

            TempData["Success"] = "Expense category successfully deleted for " + GetSessionName();
            TempData["NotificationType"] = NotificationType.Delete.ToString();
            return RedirectToAction("Index", new { sessionid = Convert.ToInt64(Session["sessionid"]) });
        }
        #endregion
    }
}