using AIMS.Data.DataContext.DataContext.ExpenseDataContext;
using AIMS.Data.DataContext.DataContext.IncomeDataContext;
using AIMS.Data.DataObjects.Entities.Expense;
using AIMS.Data.DataObjects.Entities.Income;
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
    public class ExpenseItemController : BaseController
    {
        private readonly ExpenseDataContext _expenseDataContext;
        string category = string.Empty;

        #region constructor
        public ExpenseItemController()
        {
            _expenseDataContext = new ExpenseDataContext();
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
        public ActionResult Create(ExpenseItem item)
        {
            if (ModelState.IsValid)
            {
                var itemVar = new ExpenseItem
                {
                    Title = item.Title,
                    ExpenseCategoryID = Convert.ToInt64(Session["expenseid"]),
                    Amount = item.Amount,
                    CreatedBy = Convert.ToInt64(Session["UserID"]),
                    DateCreated = DateTime.Now,
                    DateLastModified = DateTime.Now,
                    LastModifiedBy = Convert.ToInt64(Session["UserID"])
                };

                _expenseDataContext.ExpenseItem.Add(itemVar);
                _expenseDataContext.SaveChanges();
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
        public ActionResult Edit(ExpenseItem item)
        {
            if (ModelState.IsValid)
            {
                item.DateLastModified = DateTime.Now;
                item.LastModifiedBy = Convert.ToInt64(Session["UserID"]);

                _expenseDataContext.Entry(item).State = EntityState.Modified;
                _expenseDataContext.SaveChanges();
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
            TempData["Success"] = "Expense category successfully deleted for " + GetCategoryName();
            TempData["NotificationType"] = NotificationType.Delete.ToString();
            return RedirectToAction("Index", new { categoryid = Convert.ToInt64(Session["expenseid"]) });
        }
        #endregion
	}
}