﻿using AIMS.Data.DataContext.DataContext.SessionDataContext;
using AIMS.Data.DataObjects.Entities.Session;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SAAS_AIMS.Controllers
{
    public class SessionController : Controller
    {
        private SessionDataContext _sessionDataContext;

        public SessionController()
        {
            _sessionDataContext = new SessionDataContext();
        }

        //
        // GET: /Session/
        public ActionResult Index()
        {
            var session = from m in _sessionDataContext.Sessions
                          select m;

            return View(session);
        }

        //
        // GET: /Session/Create
        public ActionResult Create()
        {
            var session = new Session();
            return PartialView("Create", session);
        }

        //
        // POST: /Session/Create
        [HttpPost]
        public ActionResult Create(Session session)
        {
            if (ModelState.IsValid)
            {
                var sessionvar = new Session
                {
                    Title = session.Title,
                    StartDate = session.StartDate,
                    EndDate = session.EndDate,
                    CreatedBy = Convert.ToInt64(Session["UserID"]),
                    DateCreated = DateTime.Now,
                    DateLastModified = DateTime.Now
                };

                _sessionDataContext.Sessions.Add(sessionvar);
                _sessionDataContext.SaveChanges();
                return Json(new { success = true });
            }

            return PartialView("Create", session);
        }

        //
        // GET: /Session/Edit
        public ActionResult Edit(int id)
        {
            var session = _sessionDataContext.Sessions.Find(id);
            if (session == null) 
            {
                return HttpNotFound();
            }
            return PartialView("Edit", session);
        }

        //
        // POST: /Session/Edit
        public ActionResult Edit(Session session)
        {
            if (ModelState.IsValid)
            {
                var sessionvar = new Session
                {
                    CreatedBy = Convert.ToInt64(Session["UserID"]),
                    DateCreated = DateTime.Now,
                    DateLastModified = DateTime.Now
                };
                _sessionDataContext.Entry(sessionvar).State = EntityState.Modified;
                _sessionDataContext.SaveChanges();
                return Json(new { success = true});
            }
            return PartialView("Edit", session);
        }
	}
}