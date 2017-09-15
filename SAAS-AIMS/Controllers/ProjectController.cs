using AIMS.Data.DataContext.DataContext.ProjectDataContext;
using AIMS.Data.DataContext.DataContext.SessionDataContext;
using AIMS.Data.DataObjects.Entities.Project;
using AIMS.Data.Enums.Enums.NotificationType;
using AIMS.Data.Enums.Enums.UploadType;
using AIMS.Services.FIleUploader;
using SAAS_AIMS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SAAS_AIMS.Controllers
{
    public class ProjectController : BaseController
    {
        private readonly ProjectDataContext _projectDataContext;
        private readonly SessionDataContext _sessionDataContext;
        private readonly AppUserDataContext _appUserDataContext;

        #region constructor
        public ProjectController()
        {
            _projectDataContext = new ProjectDataContext();
            _sessionDataContext = new SessionDataContext();
            _appUserDataContext = new AppUserDataContext();
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
        public async Task<ActionResult> Create(Project project, HttpPostedFileBase file)
        {
            if (file != null)
            {
                var info = new FileInfo(file.FileName);
                if (info.Extension.ToLower() == ".jpg" || info.Extension.ToLower() == ".jpeg" || info.Extension.ToLower() == ".gif"
                || info.Extension.ToLower() == ".png" || info.Extension.ToLower() == ".pdf" || info.Extension.ToLower() == ".docx"
                || info.Extension.ToLower() == ".txt" || info.Extension.ToLower() == ".doc" || info.Extension.ToLower() == ".rtf")
                {
                }
                else
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
                    CreatedBy = User.Identity.Name,
                    DateLastModified = DateTime.Now,
                    LastModifiedBy = User.Identity.Name
                };

                _projectDataContext.Projects.Add(projectVar);
                _projectDataContext.SaveChanges();

                var message = new MailMessage();
                message.Priority = MailPriority.High;
                message.From = new MailAddress("no-reply@override.dev", "Override");
                message.Subject = "Project Entry Created";

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
                    "</strong> created a project entry on " + DateTime.Now.ToLongDateString() + " at " + DateTime.Now.ToLongTimeString() + ".</h4>" +
                    "<p style='font-size: 18px; text-align:justify;'>The project entry details are as follows: </p>" +
                    "<ul style='font-size: 18px; text-align:justify;'>" +
                        "<li>Title: " + projectVar.Title + "</li>" +
                        "<li>Session: " + GetSessionName() + "</li>" +
                        "<li>Description: " + projectVar.Description + "</li></ul>" +
                "<footer style='font-size: 18px; text-align:center;'>" +
                    "<p>&copy;" + DateTime.Now.Year + " Override.</p></footer></div>";

                message.Body = string.Format(emailBody);
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    await smtp.SendMailAsync(message);
                }
                
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
        public async Task<ActionResult> Edit(Project project, HttpPostedFileBase file)
        {
            if (file != null)
            {
                var info = new FileInfo(file.FileName);
                if (info.Extension.ToLower() == ".jpg" || info.Extension.ToLower() == ".jpeg" || info.Extension.ToLower() == ".gif"
                || info.Extension.ToLower() == ".png" || info.Extension.ToLower() == ".pdf" || info.Extension.ToLower() == ".docx"
                || info.Extension.ToLower() == ".txt" || info.Extension.ToLower() == ".doc" || info.Extension.ToLower() == ".rtf")
                {
                }
                else
                {
                    ModelState.AddModelError("incompatible", "File format not supported");
                }
            }

            if (ModelState.IsValid)
            {
                project.DateLastModified = DateTime.Now;
                project.LastModifiedBy = User.Identity.Name;

                if (file != null)
                {
                    project.FileUpload = new FileUploader().UploadFile(file, UploadType.Projects);
                }

                _projectDataContext.Entry(project).State = EntityState.Modified;
                _projectDataContext.SaveChanges();

                var message = new MailMessage();
                message.Priority = MailPriority.High;
                message.From = new MailAddress("no-reply@override.dev", "Override");
                message.Subject = "Project Entry Modified";

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
                    "</strong> modified a project entry on " + DateTime.Now.ToLongDateString() + " at " + DateTime.Now.ToLongTimeString() + ".</h4>" +
                    "<p style='font-size: 18px; text-align:justify;'>The modified project entry details are as follows: </p>" +
                    "<ul style='font-size: 18px; text-align:justify;'>" +
                        "<li>Title: " + project.Title + "</li>" +
                        "<li>Session: " + GetSessionName() + "</li>" +
                        "<li>Description: " + project.Description + "</li></ul>" +
                "<footer style='font-size: 18px; text-align:center;'>" +
                    "<p>&copy;" + DateTime.Now.Year + " Override.</p></footer></div>";

                message.Body = string.Format(emailBody);
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    await smtp.SendMailAsync(message);
                }
                
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

            var message = new MailMessage();
            message.Priority = MailPriority.High;
            message.From = new MailAddress("no-reply@override.dev", "Override");
            message.Subject = "Project Entry Deleted";

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
                "</strong> deleted a project entry on " + DateTime.Now.ToLongDateString() + " at " + DateTime.Now.ToLongTimeString() + ".</h4>" +
                "<p style='font-size: 18px; text-align:justify;'>The deleted project entry details are as follows: </p>" +
                "<ul style='font-size: 18px; text-align:justify;'>" +
                    "<li>Title: " + project.Title + "</li>" +
                    "<li>Session: " + GetSessionName() + "</li>" +
                    "<li>Description: " + project.Description + "</li></ul>" +
            "<footer style='font-size: 18px; text-align:center;'>" +
                "<p>&copy;" + DateTime.Now.Year + " Override.</p></footer></div>";

            message.Body = string.Format(emailBody);
            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                await smtp.SendMailAsync(message);
            }
                
            TempData["Success"] = "Project entry successfully deleted for " + GetSessionName();
            TempData["NotificationType"] = NotificationType.Delete.ToString();
            return RedirectToAction("Index", new { sessionid = Convert.ToInt64(Session["sessionid"]) });
        }
        #endregion
    }
}