using AIMS.Data.DataContext.DataContext.IncomeDataContext;
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
    public class IncomeItemController : BaseController
    {
        private readonly IncomeDataContext _incomeDataContext;
        private readonly AppUserDataContext _appUserDataContext;
        string category = string.Empty;

        #region constructor
        public IncomeItemController()
        {
            _incomeDataContext = new IncomeDataContext();
            _appUserDataContext = new AppUserDataContext();
        }
        #endregion

        #region get category name
        public string GetCategoryName()
        {
            var sess = _incomeDataContext.IncomeCategory.Find(Convert.ToInt64(Session["categoryid"]));
            category = sess.Title;
            return category;
        }
        #endregion

        #region income item list
        //
        // GET: /IncomeItem/
        [HttpGet]
        [Authorize]
        public ActionResult Index(long categoryid)
        {
            Session["categoryid"] = categoryid;
            var item = _incomeDataContext.IncomeItem.Where(s => s.IncomeCategoryID == categoryid).ToList();
            var category = _incomeDataContext.IncomeCategory.Find(categoryid);
            Session["sessionid"] = category.SessionID;
            TempData["category"] = category.Title;
            return View(item.OrderBy(s => s.Title));
        }
        #endregion

        #region create income item
        //
        // GET: /IncomeItem/Create
        [HttpGet]
        [Authorize]
        public ActionResult Create()
        {
            var item = new IncomeItem();
            return PartialView("Create", item);
        }

        //
        // POST: /IncomeCategory/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(IncomeItem item)
        {
            if (ModelState.IsValid)
            {
                var itemVar = new IncomeItem
                {
                    Title = item.Title,
                    IncomeCategoryID = Convert.ToInt64(Session["categoryid"]),
                    Amount = item.Amount,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    DateLastModified = DateTime.Now,
                    LastModifiedBy = User.Identity.Name
                };

                _incomeDataContext.IncomeItem.Add(itemVar);
                _incomeDataContext.SaveChanges();

                var message = new MailMessage();
                message.Priority = MailPriority.High;
                message.From = new MailAddress("no-reply@override.dev", "Override");
                message.Subject = "Income Item Created";

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
                    "</strong> created an income item on " + DateTime.Now.ToLongDateString() + " at " + DateTime.Now.ToLongTimeString() + ".</h4>" +
                    "<p style='font-size: 18px; text-align:justify;'>The income item details are as follows: </p>" +
                    "<ul style='font-size: 18px; text-align:justify;'>" +
                        "<li>Title: " + itemVar.Title + "</li>" +
                        "<li>Income Category: " + GetCategoryName() + "</li>" +
                        "<li>Amount: N " + itemVar.Amount.ToString("#,##") + "</li>"+
                "<footer style='font-size: 18px; text-align:center;'>" +
                    "<p>&copy;" + DateTime.Now.Year + " Override.</p></footer></div>";

                message.Body = string.Format(emailBody);
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    await smtp.SendMailAsync(message);
                }

                TempData["Success"] = "Income item successfully created for " + GetCategoryName();
                TempData["NotificationType"] = NotificationType.Create.ToString();
                return Json(new { success = true });
            }
            return PartialView("Create", item);
        }
        #endregion

        #region edit income item
        //
        // GET: /IncomeItem/Edit/id
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Edit(long id)
        {
            var item = await _incomeDataContext.IncomeItem.FindAsync(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return PartialView("Edit", item);
        }

        //
        // POST: /IncomeItem/Edit/id
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(IncomeItem item)
        {
            if (ModelState.IsValid)
            {
                item.DateLastModified = DateTime.Now;
                item.LastModifiedBy = User.Identity.Name;

                _incomeDataContext.Entry(item).State = EntityState.Modified;
                _incomeDataContext.SaveChanges();

                var message = new MailMessage();
                message.Priority = MailPriority.High;
                message.From = new MailAddress("no-reply@override.dev", "Override");
                message.Subject = "Income Item Modified";

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
                    "</strong> modified an income item on " + DateTime.Now.ToLongDateString() + " at " + DateTime.Now.ToLongTimeString() + ".</h4>" +
                    "<p style='font-size: 18px; text-align:justify;'>The modified income item details are as follows: </p>" +
                    "<ul style='font-size: 18px; text-align:justify;'>" +
                        "<li>Title: " + item.Title + "</li>" +
                        "<li>Income Category: " + GetCategoryName() + "</li>" +
                        "<li>Amount: N " + item.Amount.ToString("#,##") + "</li>" +
                "<footer style='font-size: 18px; text-align:center;'>" +
                    "<p>&copy;" + DateTime.Now.Year + " Override.</p></footer></div>";

                message.Body = string.Format(emailBody);
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    await smtp.SendMailAsync(message);
                }
                TempData["Success"] = "Income item successfully modified for " + GetCategoryName();
                TempData["NotificationType"] = NotificationType.Edit.ToString();
                return Json(new { success = true });
            }
            return PartialView("Edit", item);
        }
        #endregion

        #region delete income item
        //
        // DELETE: /IncomeItem/Delete/id
        [Authorize]
        public async Task<ActionResult> Delete(long id)
        {
            var item = await _incomeDataContext.IncomeItem.FindAsync(id);
            _incomeDataContext.IncomeItem.Remove(item);
            await _incomeDataContext.SaveChangesAsync();

            var message = new MailMessage();
            message.Priority = MailPriority.High;
            message.From = new MailAddress("no-reply@override.dev", "Override");
            message.Subject = "Income Item Deleted";

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
                "</strong> deleted an income item on " + DateTime.Now.ToLongDateString() + " at " + DateTime.Now.ToLongTimeString() + ".</h4>" +
                "<p style='font-size: 18px; text-align:justify;'>The deleted income item details are as follows: </p>" +
                "<ul style='font-size: 18px; text-align:justify;'>" +
                    "<li>Title: " + item.Title + "</li>" +
                    "<li>Income Category: " + GetCategoryName() + "</li>" +
                    "<li>Amount: N " + item.Amount.ToString("#,##") + "</li>" +
            "<footer style='font-size: 18px; text-align:center;'>" +
                "<p>&copy;" + DateTime.Now.Year + " Override.</p></footer></div>";

            message.Body = string.Format(emailBody);
            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                await smtp.SendMailAsync(message);
            }
            TempData["Success"] = "Income category successfully deleted for " + GetCategoryName();
            TempData["NotificationType"] = NotificationType.Delete.ToString();
            return RedirectToAction("Index", new { categoryid = Convert.ToInt64(Session["categoryid"]) });
        }
        #endregion
	}
}