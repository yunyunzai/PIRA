using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcApplication2.Filters;
using MvcApplication2.Models;
using System.Data;
using System.Data.SqlClient;
namespace MvcApplication2.Controllers
{
    public class AdminController : Controller
    {
       
        //
        // GET: /Admin
        private DISpecialistContext db = new DISpecialistContext();
        public ActionResult Admin(string searchString)
        {
            var key = from m in db.Keywords select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                key = key.Where(s => s.Keyword.Contains(searchString));
            }
            
            
            return View(key);
        }

        public ActionResult RequestManagement(String key)
        {
            
            var rid = (from m in db.Requests
                      join q in db.Questions on m.RequestId equals q.RequestId
                      join qk in db.QuestionKeywords on q.QuestionId equals qk.QuestionId
                       select m).Distinct().OrderByDescending(m => m.RequestId);
            if (String.IsNullOrEmpty(key))
            {
                
                return View(rid);
            }
            rid = (from m in db.Requests
                  join q in db.Questions on m.RequestId equals q.RequestId
                  join qk in db.QuestionKeywords on q.QuestionId equals qk.QuestionId
                  where qk.Keyword.Equals(key)
                  select m).Distinct().OrderByDescending(m=>m.RequestId);
                       
            return View(rid);
        }

        public ActionResult ViewHistory(String key)
        {
            var rid = (from m in db.Requests
                       join q in db.Questions on m.RequestId equals q.RequestId
                       join qk in db.QuestionKeywords on q.QuestionId equals qk.QuestionId
                       select m).Distinct().OrderByDescending(m => m.RequestId);
            if (String.IsNullOrEmpty(key))
                return View(rid);

            rid = (from m in db.Requests
                   join q in db.Questions on m.RequestId equals q.RequestId
                   join qk in db.QuestionKeywords on q.QuestionId equals qk.QuestionId
                   where qk.Keyword.Equals(key)
                   select m).Distinct().OrderByDescending(m => m.RequestId);

            return View(rid);
        }

        //
        // Get: /Keyword

        public ActionResult AddKeyword()
        {
            return View();
        }

        //
        // POST: /Keyword

        [HttpPost]
        public ActionResult AddKeyword(Keywords model)
        {
            
            if (ModelState.IsValid)
            {
                db.Keywords.Add(model);
                db.SaveChanges();
                return RedirectToAction("Admin");
            }
            return View();
        }

        //
        // Get: /Edit Keyword

        public ActionResult EditKeyword(String key)
        {
            Keywords keyword = db.Keywords.Find(key);
            if (keyword == null)
            {
                return HttpNotFound();
            }
            return View(keyword);
        }

        //
        // POST: /Edit Keyword

        [HttpPost]
        public ActionResult EditKeyword(String key, Keywords keyword)
        {
            keyword = db.Keywords.Find(key);
            if (ModelState.IsValid)
            {
                db.Entry(keyword).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Admin");
            }
            return View(keyword);
        }

        //
        // Get: /Edit Request

        public ActionResult EditRequest(int id)
        {
            Request request = db.Requests.Find(id);
            if (request == null)
            {
                return View();
            }
            return View(request);
        }

        //
        // POST: /Edit Request

        [HttpPost]
        public ActionResult EditRequest(Request request)
        {
            if (ModelState.IsValid)
            {
                db.Entry(request).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("RequestManagement");
            }
            return View(request);
        }



    }
}

