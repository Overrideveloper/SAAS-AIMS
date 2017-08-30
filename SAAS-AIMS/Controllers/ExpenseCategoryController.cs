using AIMS.Data.DataContext.DataContext.ExpenseDataContext;
using AIMS.Data.DataContext.DataContext.IncomeDataContext;
using AIMS.Data.DataContext.DataContext.SessionDataContext;
using AIMS.Data.DataObjects.Entities.Expense;
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
    public class ExpenseCategoryController : BaseController
    {
        private readonly ExpenseDataContext _expenseDataContext;
        private readonly SessionDataContext _sessionDataContext;
        string sessionname = string.Empty;

        #region constructor
        public ExpenseCategoryController()
        {
            _expenseDataContext = new ExpenseDataContext();
            _sessionDataContext = new SessionDataContext();
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
        public ActionResult Create(ExpenseCategory category)
        {
            if (ModelState.IsValid)
            {
                var categoryVar = new ExpenseCategory
                {
                    Title = category.Title,
                    SessionID = Convert.ToInt64(Session["sessionid"]),
                    
                    CreatedBy = Convert.ToInt64(Session["UserID"]),
                    DateCreated = DateTime.Now,
                    DateLastModified = DateTime.Now,
                    LastModifiedBy = Convert.ToInt64(Session["UserID"])
                };

                _expenseDataContext.ExpenseCategory.Add(categoryVar);
                _expenseDataContext.SaveChanges();
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
        public ActionResult Edit(ExpenseCategory category)
        {
            if (ModelState.IsValid)
            {
                category.DateLastModified = DateTime.Now;
                category.LastModifiedBy = Convert.ToInt64(Session["UserID"]);

                _expenseDataContext.Entry(category).State = EntityState.Modified;
                _expenseDataContext.SaveChanges();
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
            TempData["Success"] = "Expense category successfully deleted for " + GetSessionName();
            TempData["NotificationType"] = NotificationType.Delete.ToString();
            return RedirectToAction("Index", new { sessionid = Convert.ToInt64(Session["sessionid"]) });
        }
        #endregion
    }
}