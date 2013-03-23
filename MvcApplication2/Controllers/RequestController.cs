using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MvcApplication2.Models;
using MvcApplication2.Filters;

namespace MvcApplication2.Controllers
{
    [Authorize]
    [Role(Roles="Reporter")]
    public class RequestController : Controller
    {
        private DISpecialistContext db = new DISpecialistContext();

        //
        // GET: /Request/

        public ActionResult Index()
        {
      // 1.
            //var requests = db.UserCompleteRequests.Include(ucr => ucr.Requests)
            //                                      .Max(ucr => ucr.CompletionTime);

      // 2.
            var requests = from r in db.Requests
                           join ucr in db.UserCompleteRequest on r.RequestId equals ucr.RequestID
                           select r;

            //var final = from r in requests
            //            group r by r.RequestId into g
            //            select new 
            //            {
            //                RequestId = g.Key,
            //                UserCompleteRequest
            //            };

            var final = requests.OrderByDescending(r => r.RequestId);

            return View(requests.ToList());
        }

        //
        // GET: /Request/Details/5

        public ActionResult Details(int id = 0)
        {
            Request request = db.Requests.Find(id);
            if (request == null)
            {
                return HttpNotFound();
            }
            return View(request);
        }

        //
        // GET: /Request/Create

        public ActionResult Create()
        {
            //ViewBag.PatientId = new SelectList(db.Patients, "PatientId", "PatientId");
            //ViewBag.Name = new SelectList(db.Callers, "Name", "Email");
            //return View();

            ViewBag.PatientId = new SelectList(db.Patients, "PatientId", "PatientId");
            ViewBag.Name = new SelectList(db.Callers, "Name", "Email");
            //ViewBag.Gender = new SelectList("Female","Male","Gender");
            return View();
        }

        //
        // POST: /Request/Create

        [HttpPost]
        public ActionResult Create(Request request)
        {
            if (ModelState.IsValid)
            {
                db.Requests.Add(request);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PatientId = new SelectList(db.Patients, "PatientId", "Name", request.PatientId);
            ViewBag.Name = new SelectList(db.Callers, "Name", "Email", request.Name);
            return View(request);
        }

        //
        // GET: /Request/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Request request = db.Requests.Find(id);
            if (request == null)
            {
                return HttpNotFound();
            }
            ViewBag.PatientId = new SelectList(db.Patients, "PatientId", "PatientId", request.PatientId);
            ViewBag.Name = new SelectList(db.Callers, "Name", "Email", request.Name);
            return View(request);
        }

        //
        // POST: /Request/Edit/5

        [HttpPost]
        public ActionResult Edit(Request request)
        {
            if (ModelState.IsValid)
            {
                db.Entry(request).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PatientId = new SelectList(db.Patients, "PatientId", "PatientId", request.PatientId);
            ViewBag.Name = new SelectList(db.Callers, "Name", "Email", request.Name);
            return View(request);
        }

        //
        // GET: /Request/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Request request = db.Requests.Find(id);
            if (request == null)
            {
                return HttpNotFound();
            }
            return View(request);
        }

        //
        // POST: /Request/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Request request = db.Requests.Find(id);
            db.Requests.Remove(request);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}