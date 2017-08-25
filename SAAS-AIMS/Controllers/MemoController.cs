using AIMS.Data.DataContext.DataContext.MemoDataContext;
using AIMS.Data.DataContext.DataContext.SessionDataContext;
using AIMS.Data.DataObjects.Entities.Memo;
using AIMS.Data.Enums.Enums.NotificationType;
using AIMS.Data.Enums.Enums.UploadType;
using AIMS.Services.FIleUploader;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SAAS_AIMS.Controllers
{
    public class MemoController : Controller
    {
        private readonly MemoDataContext _memoDataContext;
        private readonly SessionDataContext _sessionDataContext;

        #region constructor
        public MemoController()
        {
            _memoDataContext = new MemoDataContext();
            _sessionDataContext = new SessionDataContext();
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
        public ActionResult Create(Memo memo, HttpPostedFileBase file)
        {
            if (file == null || file.FileName == "")
            {
                ModelState.AddModelError("empty", "Select a file for upload!");
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
                    CreatedBy = Convert.ToInt64(Session["UserID"]),
                    DateLastModified = DateTime.Now,
                    LastModifiedBy = Convert.ToInt64(Session["UserID"])
                };

                _memoDataContext.Memos.Add(memoVar);
                _memoDataContext.SaveChanges();
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
        public ActionResult Edit(Memo memo, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                memo.DateLastModified = DateTime.Now;
                memo.LastModifiedBy = Convert.ToInt64(Session["sessionid"]);

                if (file != null && file.FileName != "")
                {
                    memo.FileUpload = new FileUploader().UploadFile(file, UploadType.Memos);
                }

                _memoDataContext.Entry(memo).State = EntityState.Modified;
                _memoDataContext.SaveChanges();

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
            TempData["Success"] = "Meeting entry successfully modified for " + GetSessionName();
            TempData["NotificationType"] = NotificationType.Delete.ToString();
            return RedirectToAction("Index", new { sessionid = Convert.ToInt64(Session["sessionid"]) });
        }
        #endregion
    }
}