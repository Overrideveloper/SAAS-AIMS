using AIMS.Data.DataContext.DataContext.IncomeDataContext;
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
    public class IncomeItemController : BaseController
    {
        private readonly IncomeDataContext _incomeDataContext;
        string category = string.Empty;

        #region constructor
        public IncomeItemController()
        {
            _incomeDataContext = new IncomeDataContext();
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
        public ActionResult Create(IncomeItem item)
        {
            if (ModelState.IsValid)
            {
                var itemVar = new IncomeItem
                {
                    Title = item.Title,
                    IncomeCategoryID = Convert.ToInt64(Session["categoryid"]),
                    Amount = item.Amount,
                    CreatedBy = Convert.ToInt64(Session["UserID"]),
                    DateCreated = DateTime.Now,
                    DateLastModified = DateTime.Now,
                    LastModifiedBy = Convert.ToInt64(Session["UserID"])
                };

                _incomeDataContext.IncomeItem.Add(itemVar);
                _incomeDataContext.SaveChanges();
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
        public ActionResult Edit(IncomeItem item)
        {
            if (ModelState.IsValid)
            {
                item.DateLastModified = DateTime.Now;
                item.LastModifiedBy = Convert.ToInt64(Session["UserID"]);

                _incomeDataContext.Entry(item).State = EntityState.Modified;
                _incomeDataContext.SaveChanges();
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
            TempData["Success"] = "Income category successfully deleted for " + GetCategoryName();
            TempData["NotificationType"] = NotificationType.Delete.ToString();
            return RedirectToAction("Index", new { categoryid = Convert.ToInt64(Session["categoryid"]) });
        }
        #endregion
	}
}