using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcApplication2.Models;

namespace MvcApplication2.Controllers
{
    public class QuestionController : Controller
    {
        private QuestionContext db = new QuestionContext();

        //
        // GET: /Question/

        public ActionResult Index()
        {
            var questions = db.Questions.Include(q => q.Request);//.Include(q => q.TumorTypeAbbreviate).Include(q => q.QuestionTypeAbbreviate);

            return View(questions.ToList());
        }

        ////
        //// GET: /Question/Details/5

        //public ActionResult Details(int id = 0)
        //{
        //    Question question = db.Questions.Find(id);
        //    if (question == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(question);
        //}

        //
        // GET: /Question/Create

        public ActionResult Create()
        {
            ViewBag.RequestId = new SelectList(db.Requests, "RequestId", "Name");
            return View();
        }

        //
        // POST: /Question/Create

        [HttpPost]
        public ActionResult Create(Question question)
        {
            System.Diagnostics.Debug.WriteLine("lol");

            if (ModelState.IsValid)
            {
                db.Questions.Add(question);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RequestId = new SelectList(db.Requests, "RequestId", "Name", question.RequestId);
            return View(question);
        }

        //
        // GET: /Question/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            ViewBag.RequestId = new SelectList(db.Requests, "RequestId", "Name", question.RequestId);
            return View(question);
        }

        //
        // POST: /Question/Edit/5

        [HttpPost]
        public ActionResult Edit(Question question)
        {
            if (ModelState.IsValid)
            {
                db.Entry(question).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RequestId = new SelectList(db.Requests, "RequestId", "Name", question.RequestId);
            return View(question);
        }

        ////
        //// GET: /Question/Delete/5

        //public ActionResult Delete(int id = 0)
        //{
        //    Question question = db.Questions.Find(id);
        //    if (question == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(question);
        //}

        ////
        //// POST: /Question/Delete/5

        //[HttpPost, ActionName("Delete")]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Question question = db.Questions.Find(id);
        //    db.Questions.Remove(question);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}