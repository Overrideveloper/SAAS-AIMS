using AIMS.Data.DataContext.DataContext.MeetingDataContext;
using AIMS.Data.DataContext.DataContext.SessionDataContext;
using AIMS.Data.DataObjects.Entities.Meeting;
using AIMS.Data.Enums.Enums.NotificationType;
using AIMS.Data.Enums.Enums.UploadType;
using AIMS.Services.FIleUploader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SAAS_AIMS.Controllers
{
    public class MeetingController : Controller
    {
        private readonly MeetingDataContext _meetingdatacontext;
        private readonly SessionDataContext _sessiondatacontext;
        private static string directory = "../UploadedFiles/";
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
            string fileName = string.Empty, actualFileName = string.Empty; bool flag = false;

            HttpFileCollection fileRequested = System.Web.HttpContext.Current.Request.Files;
            if (fileRequested != null)
            {
                for (int i = 0; i < fileRequested.Count; i++)
                {
                    var file = Request.Files[i];
                    actualFileName = file.FileName;
                    fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    int size = file.ContentLength;
                    Session["filename"] = fileName;

                    try
                    {
                        file.SaveAs(Path.Combine(Server.MapPath(directory + UploadType.Minutes), fileName));
                        flag = true;
                    }
                    catch (Exception ex)
                    {
                        ex.ToString();
                    }
                }
            }

            if (ModelState.IsValid)
            {
                var meetingVar = new Meeting
                {
                    Title = meeting.Title,
                    Date = meeting.Date,
                    Venue = meeting.Venue,
                    SessionID = Convert.ToInt64(Session["sessionid"]),
                    FileUpload = fileRequested != null ? Convert.ToString(Session["filename"]) : null,
                    
                    CreatedBy = Convert.ToInt64(Session["UserID"]),
                    DateCreated = DateTime.Now,
                    DateLastModified = DateTime.Now,
                    LastModifiedBy = Convert.ToInt64(Session["UserID"])
                };

                _meetingdatacontext.Meetings.Add(meetingVar);
                _meetingdatacontext.SaveChanges();
                TempData["Success"] = "Meeting entry successfully created for " + GetSessionName();
                TempData["NotificationType"] = NotificationType.Create.ToString();
                return Json(new { success = true });
            }
            return PartialView("Create", meeting);
        }
        #endregion

    }
}