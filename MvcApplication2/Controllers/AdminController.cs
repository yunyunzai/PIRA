using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcApplication2.Filters;
using MvcApplication2.Models;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
namespace MvcApplication2.Controllers
{
    public class AdminController : Controller
    {
       
        
        private DISpecialistContext db = new DISpecialistContext();
        private LoggingContext dbl = new LoggingContext();
        private UsersContext dbUser = new UsersContext();
        //
        // GET: /Admin
        public ActionResult Admin(string searchString)
        {
            var key = from m in db.Keywords select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                key = key.Where(s => s.Keyword.Contains(searchString));
            }
            
            
            return View(key);
        }
        //GET: /Admin
      
        


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
            var userViewRequest = (from m in db.Requests
                                   join q in db.Questions on m.RequestId equals q.RequestId
                                   join qk in db.QuestionKeywords on q.QuestionId equals qk.QuestionId
                                   join uv in db.UserViewRequest on m.RequestId equals uv.RequestId
                                   select uv).ToList();
            var userCreateRequest = (from m in db.Requests
                                     join q in db.Questions on m.RequestId equals q.RequestId
                                     join qk in db.QuestionKeywords on q.QuestionId equals qk.QuestionId
                                     join uv in db.UserCreateRequest on m.RequestId equals uv.RequestId
                                     select uv).ToList();
            var userCompleteRequest = (from m in db.Requests
                                       join q in db.Questions on m.RequestId equals q.RequestId
                                       join qk in db.QuestionKeywords on q.QuestionId equals qk.QuestionId
                                       join uv in db.UserCompleteRequest on m.RequestId equals uv.RequestID
                                       select uv).ToList();
            var userEditRequest = (from m in db.Requests
                                   join q in db.Questions on m.RequestId equals q.RequestId
                                   join qk in db.QuestionKeywords on q.QuestionId equals qk.QuestionId
                                   join uv in db.UserEditRequest on m.RequestId equals uv.RequestId
                                   select uv).ToList();

            var userExportRequest = (from m in db.Requests
                                     join q in db.Questions on m.RequestId equals q.RequestId
                                     join qk in db.QuestionKeywords on q.QuestionId equals qk.QuestionId
                                     join uv in db.UserExportRequests on m.RequestId equals uv.RequestId
                                     select uv).ToList();

            if (!String.IsNullOrEmpty(key))
            {
                userViewRequest = (from m in db.Requests
                       join q in db.Questions on m.RequestId equals q.RequestId
                       join qk in db.QuestionKeywords on q.QuestionId equals qk.QuestionId
                       join uv in db.UserViewRequest on m.RequestId equals uv.RequestId
                       where qk.Keyword.Equals(key)
                       select uv).ToList();
                userCreateRequest = (from m in db.Requests
                                     join q in db.Questions on m.RequestId equals q.RequestId
                                     join qk in db.QuestionKeywords on q.QuestionId equals qk.QuestionId
                                     join uv in db.UserCreateRequest on m.RequestId equals uv.RequestId
                                     where qk.Keyword.Equals(key)
                                     select uv).ToList();
                userCompleteRequest = (from m in db.Requests
                                       join q in db.Questions on m.RequestId equals q.RequestId
                                       join qk in db.QuestionKeywords on q.QuestionId equals qk.QuestionId
                                       join uv in db.UserCompleteRequest on m.RequestId equals uv.RequestID
                                       where qk.Keyword.Equals(key)
                                       select uv).ToList();
                userEditRequest = (from m in db.Requests
                                   join q in db.Questions on m.RequestId equals q.RequestId
                                   join qk in db.QuestionKeywords on q.QuestionId equals qk.QuestionId
                                   join uv in db.UserEditRequest on m.RequestId equals uv.RequestId
                                   where qk.Keyword.Equals(key)
                                   select uv).ToList();
                userExportRequest = (from m in db.Requests
                                   join q in db.Questions on m.RequestId equals q.RequestId
                                   join qk in db.QuestionKeywords on q.QuestionId equals qk.QuestionId
                                   join uv in db.UserExportRequests on m.RequestId equals uv.RequestId
                                   where qk.Keyword.Equals(key)
                                   select uv).ToList();
            }
            
            

            List<ViewModels.LoggingModel> logmodel = new List<ViewModels.LoggingModel>();
            foreach (var ureq in userViewRequest)
            {
                logmodel.Add(new ViewModels.LoggingModel
                {
                    UserId = ureq.UserId,
                    PatientId = Convert.ToInt64(0),
                    RequestId = ureq.RequestId,
                    time = ureq.ViewTime,
                    Action = "Viewed request"

                });
            }

            foreach (var creq in userCreateRequest)
            {
                logmodel.Add(new ViewModels.LoggingModel
                {
                    UserId = creq.UserId,
                    PatientId = Convert.ToInt64(0),
                    RequestId = creq.RequestId,
                    time = creq.TimeCreated,
                    Action = "Created request"

                });
            }

            foreach (var compreq in userCompleteRequest)
            {
                logmodel.Add(new ViewModels.LoggingModel
                {
                    UserId = compreq.UserID,
                    PatientId = Convert.ToInt64(0),
                    RequestId = compreq.RequestID,
                    time = compreq.CompletionTime,
                    Action = "Completed request"
                });
            }

            foreach (var edreq in userEditRequest)
            {
                logmodel.Add(new ViewModels.LoggingModel
                {
                    UserId = edreq.UserId,
                    PatientId = Convert.ToInt64(0),
                    RequestId = edreq.RequestId,
                    time = edreq.FinishTime,
                    Action = "Edited request"

                });
            }
            foreach (var exreq in userExportRequest)
            {
                logmodel.Add(new ViewModels.LoggingModel
                {
                    UserId = exreq.UserId,
                    PatientId = Convert.ToInt64(0),
                    RequestId = exreq.RequestId,
                    time = exreq.time,
                    Action = "Exported request"

                });

            }
            var sortedList = logmodel.OrderByDescending(si => si.time).ToList();
            ViewBag.SortedList = sortedList;
            

            return View();
        }

        public ActionResult FieldManagement()
        {
            return View();
        }

        public ActionResult TumorGroup(string searchString)
        {
            var key = from m in db.TumorGroups select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                key = key.Where(s => s.Name.Contains(searchString));
            }


            return View(key);
        }

        
        public ActionResult AddTumorGroup()
        {
            return View();
        }


        [HttpPost]
        public ActionResult AddTumorGroup(TumorGroup model)
        {

            if (ModelState.IsValid)
            {
                db.TumorGroups.Add(model);
                db.SaveChanges();
                return RedirectToAction("TumorGroup");
            }
            return View();
        }

   /*     public ActionResult DeleteTumorGroup(string Abbreviate)
        {
            TumorGroup TumorGroup = db.TumorGroups.Find(Abbreviate);
            if (TumorGroup == null)
            {
                return HttpNotFound();
            }
            return View(TumorGroup);
        }



        [HttpPost, ActionName("DeleteTumorGroup")]
        public ActionResult DeleteTumorGroup2(string Abbreviate)
        {
            TumorGroup userprofile = db.TumorGroups.Find(Abbreviate);
            db.TumorGroups.Remove(userprofile);
            db.SaveChanges();
            return RedirectToAction("TumorGroup");
        }   */
        public ActionResult ActivateTumorGroup(string id)
        {
            TumorGroup tg = db.TumorGroups.Find(id);
            if (tg == null)
            {
                return HttpNotFound();

            }
            return View(tg);
        }
        [HttpPost, ActionName("ActivateTumorGroup")]
        public ActionResult ActivateTumorGroup2(string id)
        {
            TumorGroup tg = db.TumorGroups.Find(id);
            tg.IsActive = 1;
            db.SaveChanges();
            return RedirectToAction("TumorGroup");
        }

        public ActionResult ActivateQuestionType(string id)
        {
            QuestionType tg = db.QuestionTypes.Find(id);
            if (tg == null)
            {
                return HttpNotFound();

            }
            return View(tg);
        }
        [HttpPost, ActionName("ActivateQuestionType")]
        public ActionResult ActivateQuestionType2(string id)
        {
            QuestionType tg = db.QuestionTypes.Find(id);
            tg.IsActive = 1;
            db.SaveChanges();
            return RedirectToAction("QuestionType");
        }

        public ActionResult ActivateUserGroup(string abbr)
        {
            UserGroup tg = db.UserGroups.Find(abbr);
            if (tg == null)
            {
                return HttpNotFound();

            }
            return View(tg);
        }
        [HttpPost, ActionName("ActivateUserGroup")]
        public ActionResult ActivateUserGroup2(string abbr)
        {
            UserGroup tg = db.UserGroups.Find(abbr);
            tg.IsActive = 1;
            db.SaveChanges();
            return RedirectToAction("UserGroup");
        }

        public ActionResult DeactivateUserGroup(string abbr)
        {
            UserGroup tg = db.UserGroups.Find(abbr);
            if (tg == null)
            {
                return HttpNotFound();

            }
            return View(tg);
        }
        [HttpPost, ActionName("DeactivateUserGroup")]
        public ActionResult DeactivateUserGroup2(string abbr)
        {
            UserGroup tg = db.UserGroups.Find(abbr);
            tg.IsActive = 0;
            db.SaveChanges();
            return RedirectToAction("UserGroup");
        }


        public ActionResult DeactivateQuestionType(string id)
        {
            QuestionType tg = db.QuestionTypes.Find(id);
            if (tg == null)
            {
                return HttpNotFound();

            }
            return View(tg);
        }
        [HttpPost, ActionName("DeactivateQuestionType")]
        public ActionResult DeactivateQuestionType2(string id)
        {
            QuestionType tg = db.QuestionTypes.Find(id);
            tg.IsActive = 0;
            db.SaveChanges();
            return RedirectToAction("QuestionType");
        }

        public ActionResult DeactivateTumorGroup(string id)
        {
            TumorGroup tg = db.TumorGroups.Find(id);
            if (tg == null)
            {
                return HttpNotFound();

            }
            return View(tg);
        }
        [HttpPost, ActionName("DeactivateTumorGroup")]
        public ActionResult DeactivateTumorGroup2(string id)
        {
            TumorGroup tg = db.TumorGroups.Find(id);
            tg.IsActive = 0;
            db.SaveChanges();
            return RedirectToAction("TumorGroup");
        }
        public ActionResult QuestionType(string searchString)
        {
            var key = from m in db.QuestionTypes select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                key = key.Where(s => s.Name.Contains(searchString));
            }


            return View(key);
        }


        public ActionResult AddQuestionType()
        {
            return View();
        }


        [HttpPost]
        public ActionResult AddQuestionType(QuestionType model)
        {

            if (ModelState.IsValid)
            {
                db.QuestionTypes.Add(model);
                db.SaveChanges();
                return RedirectToAction("QuestionType");
            }
            return View();
        }
        //GET
      //  [HttpGet]
        public ActionResult ActivateCallerType(string id)
        {
            CallerType ct = db.Callertypes.Find(id);
            if (ct == null)
            {
                return HttpNotFound();
            }
            return View(ct);
        }
        [HttpPost, ActionName("ActivateCallerType")]
        public ActionResult ActivateCallerType2(string id)
        {
            CallerType ct = db.Callertypes.Find(id);
            ct.IsActive = 1;
            db.SaveChanges();

            return RedirectToAction("CallerType");
        }

        //GET
       // [HttpGet]
        public ActionResult DeactivateCallerType(string id)
        {
            CallerType ct = db.Callertypes.Find(id);
            if (ct == null)
            {
                return HttpNotFound();
            }
            return View(ct);
        }
        [HttpPost, ActionName("DeactivateCallerType")]
        public ActionResult DeactivateCallerType2(string id)
        {
            CallerType ct = db.Callertypes.Find(id);
            ct.IsActive = 0;
            db.SaveChanges();

            return RedirectToAction("CallerType");
        }


        public ActionResult DeleteQuestionType(string id)
        {
            QuestionType userprofile = db.QuestionTypes.Find(id);
            if (userprofile == null)
            {
                return HttpNotFound();
            }
            return View(userprofile);
        }


        [HttpPost, ActionName("DeleteQuestionType")]
        public ActionResult DeleteQuestionType2(string id)
        {
            QuestionType userprofile = db.QuestionTypes.Find(id);
            db.QuestionTypes.Remove(userprofile);
            db.SaveChanges();
            return RedirectToAction("QuestionType");
        }

        public ActionResult UserGroup(string searchString)
        {
            var key = from m in dbUser.UserGroup select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                key = key.Where(s => s.Name.Contains(searchString));
            }


            return View(key);
        }


        public ActionResult AddUserGroup()
        {
            return View();
        }


        [HttpPost]
        public ActionResult AddUserGroup(UserGroup model)
        {

            if (ModelState.IsValid)
            {
                dbUser.UserGroup.Add(model);
                dbUser.SaveChanges();
                return RedirectToAction("UserGroup");
            }
            return View();
        }

        public ActionResult DeleteUserGroup(string Abbreviate)
        {
            UserGroup userprofile = dbUser.UserGroup.Find(Abbreviate);
            if (userprofile == null)
            {
                return HttpNotFound();
            }
            return View(userprofile);
        }


        [HttpPost, ActionName("DeleteUserGroup")]
        public ActionResult DeleteUserGroup2(string Abbreviate)
        {
            UserGroup userprofile = dbUser.UserGroup.Find(Abbreviate);
            dbUser.UserGroup.Remove(userprofile);
            dbUser.SaveChanges();
            return RedirectToAction("UserGroup");
        }

        public ActionResult CallerType(string searchString)
        {
            var key = from m in db.Callertypes select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                key = key.Where(s => s.Name.Contains(searchString));
            }


            return View(key);
        }


        public ActionResult AddCallerType()
        {
            return View();
        }


        [HttpPost]
        public ActionResult AddCallerType(CallerType model)
        {

            if (ModelState.IsValid)
            {
                db.Callertypes.Add(model);
                db.SaveChanges();
                return RedirectToAction("CallerType");
            }
            return View();
        }

        public ActionResult DeleteCallerType(string id)
        {
            CallerType userprofile = db.Callertypes.Find(id);
            if (userprofile == null)
            {
                return HttpNotFound();
            }
            return View(userprofile);
        }


        [HttpPost, ActionName("DeleteCallerType")]
        public ActionResult DeleteCallerType2(string id)
        {
            CallerType userprofile = db.Callertypes.Find(id);
            db.Callertypes.Remove(userprofile);
            db.SaveChanges();
            return RedirectToAction("CallerType");
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
            Keywords keyword = db.Keywords.Where(i => i.Keyword == key).Single();
            if (keyword == null)
            {
                return HttpNotFound();
            }
            ViewBag.keyID = keyword.Keyword;
            ViewBag.keyIsActive = keyword.IsActive;
            return View();
        }

        //
        // POST: /Edit Keyword

        [HttpPost]
        public ActionResult EditKeyword(String key, FormCollection collection)
        {

           
            if (ModelState.IsValid)
            {
             //   db.Entry(keyword).State = EntityState.Modified;
             //   db.SaveChanges();
               
                bool flag = false;
                if (Convert.ToInt16(collection["activationStatus"]) == 1)
                {
                    flag = true;

                }
                
                            

                Keywords k1 = db.Keywords.Single(dbk => dbk.Keyword == key);
                k1.Keyword = key;
                k1.IsActive = flag;
                db.SaveChanges();
                return RedirectToAction("Admin");
            }
            return View();
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

