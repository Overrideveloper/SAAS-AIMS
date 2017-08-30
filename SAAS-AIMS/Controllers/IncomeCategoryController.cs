using AIMS.Data.DataContext.DataContext.IncomeDataContext;
using AIMS.Data.DataContext.DataContext.SessionDataContext;
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
    public class IncomeCategoryController : BaseController
    {
        private readonly IncomeDataContext _incomeDataContext;
        private readonly SessionDataContext _sessionDataContext;
        string sessionname = string.Empty;

        #region constructor
        public IncomeCategoryController()
        {
            _incomeDataContext = new IncomeDataContext();
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
        public ActionResult Create(IncomeCategory category)
        {
            if (ModelState.IsValid)
            {
                var categoryVar = new IncomeCategory
                {
                    Title = category.Title,
                    SessionID = Convert.ToInt64(Session["sessionid"]),
                    
                    CreatedBy = Convert.ToInt64(Session["UserID"]),
                    DateCreated = DateTime.Now,
                    DateLastModified = DateTime.Now,
                    LastModifiedBy = Convert.ToInt64(Session["UserID"])
                };

                _incomeDataContext.IncomeCategory.Add(categoryVar);
                _incomeDataContext.SaveChanges();
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
        public ActionResult Edit(IncomeCategory category)
        {
            if (ModelState.IsValid)
            {
                category.DateLastModified = DateTime.Now;
                category.LastModifiedBy = Convert.ToInt64(Session["UserID"]);

                _incomeDataContext.Entry(category).State = EntityState.Modified;
                _incomeDataContext.SaveChanges();
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
            TempData["Success"] = "Income category successfully deleted for " + GetSessionName();
            TempData["NotificationType"] = NotificationType.Delete.ToString();
            return RedirectToAction("Index", new { sessionid = Convert.ToInt64(Session["sessionid"]) });
        }
        #endregion
    }
}