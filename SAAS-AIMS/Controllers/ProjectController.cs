using AIMS.Data.DataContext.DataContext.ProjectDataContext;
using AIMS.Data.DataContext.DataContext.SessionDataContext;
using AIMS.Data.DataObjects.Entities.Project;
using AIMS.Data.Enums.Enums.NotificationType;
using AIMS.Data.Enums.Enums.UploadType;
using AIMS.Services.FIleUploader;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SAAS_AIMS.Controllers
{
    public class ProjectController : BaseController
    {
        private readonly ProjectDataContext _projectDataContext;
        private readonly SessionDataContext _sessionDataContext;

        #region constructor
        public ProjectController()
        {
            _projectDataContext = new ProjectDataContext();
            _sessionDataContext = new SessionDataContext();
        }
        #endregion

        #region academic session's projects index
        //
        // GET: /Project/
        [HttpGet]
        [Authorize]
        public ActionResult Index(long sessionid)
        {
            Session["sessionid"] = sessionid;
            var project = _projectDataContext.Projects.Where(s => s.SessionID == sessionid).ToList();
            var sess = _sessionDataContext.Sessions.Find(sessionid);
            TempData["sess"] = sess.Title;
            return View(project.OrderBy(s => s.Title));
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

        #region create project entry
        //
        // GET: /Project/Create
        [HttpGet]
        [Authorize]
        public ActionResult Create(long sessionid)
        {
            Session["sessionid"] = sessionid;
            var project = new Project();
            return View("Create", project);
        }

        //
        // POST: /Project/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Project project, HttpPostedFileBase file)
        {
            var info = new FileInfo(file.FileName);

            if (file.FileName != "" || file != null)
            {
                if ((info.Extension.ToLower() != ".jpg") || (info.Extension.ToLower() != ".jpeg") || (info.Extension.ToLower() != ".gif")
                || (info.Extension.ToLower() != ".png") || (info.Extension.ToLower() != ".pdf") || (info.Extension.ToLower() != ".docx")
                || (info.Extension.ToLower() != ".txt") || (info.Extension.ToLower() != ".doc") || (info.Extension.ToLower() != ".rtf"))
                {
                    ModelState.AddModelError("incompatible", "File format not supported");
                }
            }

            if (ModelState.IsValid)
            {
                var projectVar = new Project
                {
                    Title = project.Title,
                    Description = project.Description,
                    FileUpload = file != null && file.FileName != ""
                            ? new FileUploader().UploadFile(file, UploadType.Projects)
                            : null,
                    SessionID = Convert.ToInt64(Session["sessionid"]),
                    DateCreated = DateTime.Now,
                    CreatedBy = Convert.ToInt64(Session["UserID"]),
                    DateLastModified = DateTime.Now,
                    LastModifiedBy = Convert.ToInt64(Session["UserID"])
                };

                _projectDataContext.Projects.Add(projectVar);
                _projectDataContext.SaveChanges();
                TempData["Success"] = "Project entry successfully created for " + GetSessionName();
                TempData["NotificationType"] = NotificationType.Create.ToString();
                return RedirectToAction("Index", new { sessionid = Convert.ToInt64(Session["sessionid"]) });
            }
            return View("Create", project);
        }
        #endregion

        #region edit project entry
        //
        // GET: /Project/Edit/id
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Edit(long id)
        {
            var project = await _projectDataContext.Projects.FindAsync(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View("Edit", project);
        }

        //
        // POST: /Project/Edit/id
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Project project, HttpPostedFileBase file)
        {
            var info = new FileInfo(file.FileName);

            if (file.FileName != "" || file != null)
            {
                if ((info.Extension.ToLower() != ".jpg") || (info.Extension.ToLower() != ".jpeg") || (info.Extension.ToLower() != ".gif")
                || (info.Extension.ToLower() != ".png") || (info.Extension.ToLower() != ".pdf") || (info.Extension.ToLower() != ".docx")
                || (info.Extension.ToLower() != ".txt") || (info.Extension.ToLower() != ".doc") || (info.Extension.ToLower() != ".rtf"))
                {
                    ModelState.AddModelError("incompatible", "File format not supported");
                }
            }

            if (ModelState.IsValid)
            {
                project.DateLastModified = DateTime.Now;
                project.LastModifiedBy = Convert.ToInt64(Session["sessionid"]);

                if (file != null && file.FileName != "")
                {
                    project.FileUpload = new FileUploader().UploadFile(file, UploadType.Projects);
                }

                _projectDataContext.Entry(project).State = EntityState.Modified;
                _projectDataContext.SaveChanges();

                TempData["Success"] = "Project entry successfully modified for " + GetSessionName();
                TempData["NotificationType"] = NotificationType.Edit.ToString();
                return RedirectToAction("Index", new { sessionid = Convert.ToInt64(Session["sessionid"]) });
            }
            return View("Edit", project);
        }
        #endregion

        #region delete project entry
        //
        // DELETE: /Project/Delete/id
        [Authorize]
        public async Task<ActionResult> Delete(long id)
        {
            var project = await _projectDataContext.Projects.FindAsync(id);
            _projectDataContext.Projects.Remove(project);
            await _projectDataContext.SaveChangesAsync();
            TempData["Success"] = "Project entry successfully deleted for " + GetSessionName();
            TempData["NotificationType"] = NotificationType.Delete.ToString();
            return RedirectToAction("Index", new { sessionid = Convert.ToInt64(Session["sessionid"]) });
        }
        #endregion
    }
}