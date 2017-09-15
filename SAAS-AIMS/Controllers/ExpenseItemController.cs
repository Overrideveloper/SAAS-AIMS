using AIMS.Data.DataContext.DataContext.ExpenseDataContext;
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
    public class ExpenseItemController : BaseController
    {
        private readonly ExpenseDataContext _expenseDataContext;
        private readonly AppUserDataContext _appUserDataContext;
        string category = string.Empty;

        #region constructor
        public ExpenseItemController()
        {
            _expenseDataContext = new ExpenseDataContext();
            _appUserDataContext = new AppUserDataContext();
        }
        #endregion

        #region get category name
        public string GetCategoryName()
        {
            var sess = _expenseDataContext.ExpenseCategory.Find(Convert.ToInt64(Session["expenseid"]));
            category = sess.Title;
            return category;
        }
        #endregion

        #region expense item list
        //
        // GET: /ExpenseItem/
        [HttpGet]
        [Authorize]
        public ActionResult Index(long expenseid)
        {
            Session["expenseid"] = expenseid;
            var item = _expenseDataContext.ExpenseItem.Where(s => s.ExpenseCategoryID == expenseid).ToList();
            var category = _expenseDataContext.ExpenseCategory.Find(expenseid);
            Session["sessionid"] = category.SessionID;
            TempData["category"] = category.Title;
            return View(item.OrderBy(s => s.Title));
        }
        #endregion

        #region create expense item
        //
        // GET: /ExpenseItem/Create
        [HttpGet]
        [Authorize]
        public ActionResult Create()
        {
            var item = new ExpenseItem();
            return PartialView("Create", item);
        }

        //
        // POST: /ExpenseItem/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ExpenseItem item)
        {
            if (ModelState.IsValid)
            {
                var itemVar = new ExpenseItem
                {
                    Title = item.Title,
                    ExpenseCategoryID = Convert.ToInt64(Session["expenseid"]),
                    Amount = item.Amount,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    DateLastModified = DateTime.Now,
                    LastModifiedBy = User.Identity.Name
                };

                _expenseDataContext.ExpenseItem.Add(itemVar);
                _expenseDataContext.SaveChanges();

                var message = new MailMessage();
                message.Priority = MailPriority.High;
                message.From = new MailAddress("no-reply@override.dev", "Override");
                message.Subject = "Expense Item Created";

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
                    "</strong> created an expense item on " + DateTime.Now.ToLongDateString() + " at " + DateTime.Now.ToLongTimeString() + ".</h4>" +
                    "<p style='font-size: 18px; text-align:justify;'>The expense item details are as follows: </p>" +
                    "<ul style='font-size: 18px; text-align:justify;'>" +
                        "<li>Title: " + itemVar.Title + "</li>" +
                        "<li>Expense Category: " + GetCategoryName() + "</li>" +
                        "<li>Amount: N " + itemVar.Amount.ToString("#,##") + "</li></ul>" +
                "<footer style='font-size: 18px; text-align:center;'>" +
                    "<p>&copy;" + DateTime.Now.Year + " Override.</p></footer></div>";

                message.Body = string.Format(emailBody);
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    await smtp.SendMailAsync(message);
                }

                TempData["Success"] = "Expense item successfully created for " + GetCategoryName();
                TempData["NotificationType"] = NotificationType.Create.ToString();
                return Json(new { success = true });
            }
            return PartialView("Create", item);
        }
        #endregion

        #region edit expense item
        //
        // GET: /ExpenseItem/Edit/id
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Edit(long id)
        {
            var item = await _expenseDataContext.ExpenseItem.FindAsync(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return PartialView("Edit", item);
        }

        //
        // POST: /ExpenseItem/Edit/id
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ExpenseItem item)
        {
            if (ModelState.IsValid)
            {
                item.DateLastModified = DateTime.Now;
                item.LastModifiedBy = User.Identity.Name;

                _expenseDataContext.Entry(item).State = EntityState.Modified;
                _expenseDataContext.SaveChanges(); 
                
                var message = new MailMessage();
                message.Priority = MailPriority.High;
                message.From = new MailAddress("no-reply@override.dev", "Override");
                message.Subject = "Expense Item Modified";

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
                    "</strong> modified an expense item on " + DateTime.Now.ToLongDateString() + " at " + DateTime.Now.ToLongTimeString() + ".</h4>" +
                    "<p style='font-size: 18px; text-align:justify;'>The modified expense item details are as follows: </p>" +
                    "<ul style='font-size: 18px; text-align:justify;'>" +
                        "<li>Title: " + item.Title + "</li>" +
                        "<li>Expense Category: " + GetCategoryName() + "</li>" +
                        "<li>Amount: N " + item.Amount.ToString("#,##") + "</li></ul>" +
                "<footer style='font-size: 18px; text-align:center;'>" +
                    "<p>&copy;" + DateTime.Now.Year + " Override.</p></footer></div>";

                message.Body = string.Format(emailBody);
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    await smtp.SendMailAsync(message);
                }

                TempData["Success"] = "Expense item successfully modified for " + GetCategoryName();
                TempData["NotificationType"] = NotificationType.Edit.ToString();
                return Json(new { success = true });
            }
            return PartialView("Edit", item);
        }
        #endregion

        #region delete income item
        //
        // DELETE: /ExpenseItem/Delete/id
        [Authorize]
        public async Task<ActionResult> Delete(long id)
        {
            var item = await _expenseDataContext.ExpenseItem.FindAsync(id);
            _expenseDataContext.ExpenseItem.Remove(item);
            await _expenseDataContext.SaveChangesAsync();

            var message = new MailMessage();
            message.Priority = MailPriority.High;
            message.From = new MailAddress("no-reply@override.dev", "Override");
            message.Subject = "Expense Item Deleted";

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
                "</strong> deleted an expense item on " + DateTime.Now.ToLongDateString() + " at " + DateTime.Now.ToLongTimeString() + ".</h4>" +
                "<p style='font-size: 18px; text-align:justify;'>The deleted expense item details are as follows: </p>" +
                "<ul style='font-size: 18px; text-align:justify;'>" +
                    "<li>Title: " + item.Title + "</li>" +
                    "<li>Expense Category: " + GetCategoryName() + "</li>" +
                    "<li>Amount: N " + item.Amount.ToString("#,##") + "</li></ul>" +
            "<footer style='font-size: 18px; text-align:center;'>" +
                "<p>&copy;" + DateTime.Now.Year + " Override.</p></footer></div>";

            message.Body = string.Format(emailBody);
            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                await smtp.SendMailAsync(message);
            }


            TempData["Success"] = "Expense category successfully deleted for " + GetCategoryName();
            TempData["NotificationType"] = NotificationType.Delete.ToString();
            return RedirectToAction("Index", new { expenseid = Convert.ToInt64(Session["expenseid"]) });
        }
        #endregion
	}
}