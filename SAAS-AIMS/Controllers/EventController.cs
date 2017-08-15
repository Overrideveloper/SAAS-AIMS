using AIMS.Data.DataContext.DataContext.EventDataContext;
using AIMS.Data.DataContext.DataContext.SessionDataContext;
using AIMS.Data.DataObjects.Entities.Event;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SAAS_AIMS.Controllers
{
    public class EventController : Controller
    {
        private readonly EventDataContext _eventdatacontext;
        private readonly SessionDataContext _sessiondatacontext;

        #region constructor
        public EventController()
        {
            _eventdatacontext = new EventDataContext();
            _sessiondatacontext = new SessionDataContext();
        }
        #endregion

        #region academic session's event index
        //
        // GET: /Event/
        [HttpGet]
        [Authorize]
        public ActionResult Index(long sessionid )
        {
            var events = _eventdatacontext.Event.Where(var => var.SessionID == sessionid);
            return View(events.OrderBy(var => var.Date));
        }
        #endregion

        #region create event
        //
        // GET: /Event/Create
        [HttpGet]
        [Authorize]
        public ActionResult Create()
        {
            var events = new Event();
            return PartialView("Create", events);
        }

        //
        // POST: /Event/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Event events)
        {
            if(ModelState.IsValid)
            {
                var eventVar = new Event
                {
                    Title = events.Title,
                    Venue = events.Venue,
                    SessionID = Convert.ToInt64(Session["sessionid"]),
                    Semester = events.Semester,
                    Date = events.Date,

                    CreatedBy = Convert.ToInt64(Session["UserID"]),
                    DateCreated = DateTime.Now,
                    DateLastModified = DateTime.Now,
                    LastModifiedBy = Convert.ToInt64(Session["UserID"])
                };

                _eventdatacontext.Event.Add(eventVar);
                _eventdatacontext.SaveChanges();
                return Json(new { success = true});
            }
            return PartialView("Create", events);
        }
        #endregion

        #region edit event
        //
        // GET: /Event/Edit/id
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Edit(long id)
        {
            var events = await _eventdatacontext.Event.FindAsync(id);
            if (events == null)
            {
                return HttpNotFound();
            }
            return PartialView("Edit", events);
        }

        //
        // POST: /Event/Edit/id
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Event events)
        {
            if(ModelState.IsValid)
            {
                events.DateLastModified = DateTime.Now;
                events.LastModifiedBy = Convert.ToInt64(Session["UserID"]);

                _eventdatacontext.Entry(events).State = EntityState.Modified;
                await _eventdatacontext.SaveChangesAsync();

                return Json(new { success = true });
            }
            return PartialView("Edit", events);
        }

        #endregion

        #region delete event
        //
        // DELETE: /Event/Delete
        [Authorize]
        public async Task<ActionResult> Delete(long id)
        {
            var events = await _eventdatacontext.Event.FindAsync(id);
            _eventdatacontext.Event.Remove(events);
            await _eventdatacontext.SaveChangesAsync();
            return RedirectToAction("Index", new { sessionid = Convert.ToInt64(Session["sessionid"]) });
        }
        #endregion
    }
}