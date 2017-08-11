using AIMS.Data.DataContext.DataContext.MeetingDataContext;
using AIMS.Data.DataContext.DataContext.SessionDataContext;
using AIMS.Data.DataObjects.Entities.Meeting;
using AIMS.Data.Enums;
using AIMS.Data.Enums.Enums.NotificationType;
using AIMS.Services.FIleUploader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SAAS_AIMS.Controllers
{
    public class MeetingController : Controller
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
            return View(meetings);
        }
        #endregion

        #region create meeting
        //
        // GET: /Meeting/Create
        [HttpGet]
        [Authorize]
        public ActionResult Create() 
        {
            var meeting = new Meeting();
            return PartialView("Create", meeting);
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
                    SessionID = Convert.ToInt64(Session["sessionid"]),
                    FileUpload = file != null && file.FileName != "" 
                                 ? new FileUploader().UploadFile(file, UploadType.Minutes) : null,
                    
                    CreatedBy = Convert.ToInt64(Session["UserID"]),
                    DateCreated = DateTime.Now,
                    DateLastModified = DateTime.Now,
                    LastModifiedBy = Convert.ToInt64(Session["UserID"])
                };

                _meetingdatacontext.Meetings.Add(meetingVar);
                _meetingdatacontext.SaveChanges();
                TempData["Success"] = "Meeting entry successfully created!";
                TempData["NotificationType"] = NotificationType.Create.ToString();
                return Json(new { success = true });
            }
            return PartialView("Create", meeting);
        }
        #endregion

    }
}