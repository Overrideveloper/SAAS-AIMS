using AIMS.Data.DataContext.DataContext.DuesDataContext;
using AIMS.Data.DataObjects.Entities.Dues;
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
    public class DuesController : Controller
    {
        private DuesDataContext _duesdatacontext;

        #region constructor
        private DuesController()
        {
            _duesdatacontext = new DuesDataContext();
        }
        #endregion

        #region member's dues index
        //
        // GET: /Dues/
        [HttpGet]
        public ActionResult Index(long memberid)
        {
            Session["memberid"] = memberid;
            var duesList = from m in _duesdatacontext.Dues.Where(dues => dues.MemberID == memberid)
                       select m;
            return View(duesList);
        }
        #endregion

        #region create member's dues
        //
        // GET: /Dues/Create
        [HttpGet]
        public ActionResult Create()
        {
            var due = new Dues();
            return PartialView("Create", due);
        }

        //
        // POST: /Dues/Create
        [HttpPost]
        public ActionResult Create(Dues due)
        {
            if(ModelState.IsValid)
            {
                var dueObj = new Dues
                {
                    Title = due.Title,
                    MemberID = Convert.ToInt64(Session["memberid"]),
                    Level = due.Level,
                    Amount = due.Amount,
                    DateCreated = DateTime.Now,
                    DateLastModified = DateTime.Now,
                    CreatedBy = Convert.ToInt64(Session["UserID"]),
                    LastModifiedBy = Convert.ToInt64(Session["UserID"])
                };
                _duesdatacontext.Dues.Add(dueObj);
                _duesdatacontext.SaveChanges();

                TempData["Success"] = "Member's dues successfully added!";
                TempData["NotificationType"] = NotificationType.Create;
                return Json(new { success = true });
            }
            return PartialView("Create", due);
        }
        #endregion

        #region edit member's dues
        //
        // GET: /Dues/Edit/id
        [HttpGet]
        public ActionResult Edit(long id)
        {
            var due = _duesdatacontext.Dues.Find(id);
            if(due == null)
            {
                return HttpNotFound();
            }
            return PartialView("Edit", due);
        }

        [HttpPost]
        public ActionResult Edit(Dues due)
        {
            if (ModelState.IsValid)
            {
                due.DateLastModified = DateTime.Now;
                due.LastModifiedBy = Convert.ToInt64(Session["UserID"]);

                _duesdatacontext.Entry(due).State = EntityState.Modified;
                _duesdatacontext.SaveChanges();

                TempData["Success"] = "Member's dues successfully modified";
                TempData["NotificationType"] = NotificationType.Edit;
                return Json(new { success = true });
            }
            return PartialView("Edit", due);
        }
        #endregion

        #region delete member's dues
        //
        // GET: Dues/Delete
        public async Task<ActionResult> Delete(long id)
        {
            var due = await _duesdatacontext.Dues.FindAsync(id);
            _duesdatacontext.Dues.Remove(due);
            await _duesdatacontext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion
    }
}