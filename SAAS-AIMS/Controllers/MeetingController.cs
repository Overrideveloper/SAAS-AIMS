using AIMS.Data.DataContext.DataContext.MeetingDataContext;
using AIMS.Data.DataContext.DataContext.SessionDataContext;
using AIMS.Data.DataObjects.Entities.Meeting;
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
    public class MeetingController : BaseController
    {
        private readonly MeetingDataContext _meetingdatacontext;
        private readonly SessionDataContext _sessiondatacontext;
        private string sessionname;

        #region constructor
        public MeetingController()
        {
            _meetingdatacontext = new MeetingDataContext();
            _sessiondatacontext = new SessionDataContext();
        }
        #endregion

        #region academic session's meetings index
        //
        // GET: /Meeting/
        [HttpGet]
        [Authorize]
        public ActionResult Index(long sessionid)
        {
            sessionid = Convert.ToInt64(Session["sessionid"]);
            var meetings = _meetingdatacontext.Meetings.Where(meeting => meeting.SessionID == sessionid);
            return View(meetings.OrderBy(meeting => meeting.Date));
        }
        #endregion

        #region get session name
        public string GetSessionName()
        {
            var session = _sessiondatacontext.Sessions.Find(Convert.ToInt64(Session["sessionid"]));
            sessionname = session.Title.ToString();
            return sessionname;                                                                   
        }
        #endregion

        #region create meeting
        //
        // GET: /Meeting/Create
        [HttpGet]
        [Authorize]
        public ActionResult Create(long sessionid) 
        {
            var meeting = new Meeting();
            return View("Create", meeting);
        }

        //
        // POST: /Meeting/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Meeting meeting)
        {
            var file = Request.Files["file"];

            if (ModelState.IsValid)
            {
                var meetingVar = new Meeting
                {
                    Title = meeting.Title,
                    Date = meeting.Date,
                    Venue = meeting.Venue,
                    Semester = meeting.Semester,
                    SessionID = Convert.ToInt64(Session["sessionid"]),
                    FileUpload = file != null && file.FileName != ""
                            ? new FileUploader().UploadFile(file, UploadType.Minutes)
                            : null,

                    CreatedBy = Convert.ToInt64(Session["UserID"]),
                    DateCreated = DateTime.Now,
                    DateLastModified = DateTime.Now,
                    LastModifiedBy = Convert.ToInt64(Session["UserID"])
                };

                _meetingdatacontext.Meetings.Add(meetingVar);
                _meetingdatacontext.SaveChanges();
                TempData["Success"] = "Meeting entry successfully created for " + GetSessionName();
                TempData["NotificationType"] = NotificationType.Create.ToString();
                return RedirectToAction("Index", new { sessionid = Convert.ToInt64(Session["sessionid"])});
            }
            return View("Create", meeting);
        }
        #endregion

        #region edit meeting
        //
        // GET: /Meeting/Edit/id
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Edit(long id)
        {
            var meeting = await _meetingdatacontext.Meetings.FindAsync(id);
            if (meeting == null)
            {
                return HttpNotFound();
            }
            return View("Edit", meeting);
        }

        //
        // POST: /Meeting/Edit/id
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Meeting meeting)
        {
            var file = Request.Files["file"];
            if (ModelState.IsValid)
            {
                meeting.DateLastModified = DateTime.Now;
                meeting.LastModifiedBy = Convert.ToInt64(Session["UserID"]);
                
                if(file != null && file.FileName != ""){
                    meeting.FileUpload = new FileUploader().UploadFile(file, UploadType.Minutes);
                }

                _meetingdatacontext.Entry(meeting).State = EntityState.Modified;
                await _meetingdatacontext.SaveChangesAsync();

                TempData["Success"] = "Meeting entry successfully modified for " + GetSessionName();
                TempData["NotificationType"] = NotificationType.Edit.ToString();
                return RedirectToAction("Index", new { sessionid = Convert.ToInt64(Session["sessionid"]) });
            }
            return View("Edit", meeting);
        }
        #endregion

        #region delete meeting
        //
        // DELETE: /Event/Delete/id
        public async Task<ActionResult> Delete(long id)
        {
            var meeting = await _meetingdatacontext.Meetings.FindAsync(id);
            if (meeting == null)
            {
                return HttpNotFound();
            }

            _meetingdatacontext.Meetings.Remove(meeting);
            await _meetingdatacontext.SaveChangesAsync();
            TempData["Success"] = "Meeting entry successfully deleted for " + GetSessionName();
            TempData["NotificationType"] = NotificationType.Delete.ToString();
            return RedirectToAction("Index", new { sessionid = Convert.ToInt64(Session["sessionid"]) });
        }
        #endregion
    }
}