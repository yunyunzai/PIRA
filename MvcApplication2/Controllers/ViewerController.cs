using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcApplication2.Models;

using System.IO;
using NPOI.Util.Collections;
using MvcApplication2.Filters;
using System.Web.Security;

namespace MvcApplication2.Controllers
{
    public class ViewerController : Controller
    {

        //
        // GET: /Viewer/
        private static DISpecialistModel globalModel = new DISpecialistModel();


     
        //[ValidateAntiForgeryToken]
        //[ValidateInput(false)]
        public ActionResult Viewer()
        {
            return View(globalModel);
        }


        //[HttpPost]
        public ActionResult view(DISpecialistModel m)
        {

            DISpecialistContext db = new DISpecialistContext();

            var creater = from t1 in db.UserCreateRequest
                          join t2 in db.Users on t1.UserId equals t2.UserId
                          select new { userCreateRequest = t1, userProfile = t2 };

            var completer = from t1 in db.UserCompleteRequest
                            join t2 in db.Users on t1.UserID equals t2.UserId
                            select new { userCompleteRequest = t1, userProfile = t2 };


            var rs = (from t0 in
                          (from t1 in db.Requests
                           join t2 in creater on t1.RequestId equals t2.userCreateRequest.RequestId
                           join t8 in completer on t1.RequestId equals t8.userCompleteRequest.RequestID
                           join t3 in db.Callers on new { t1.Name, t1.Phone } equals new { t3.Name, t3.Phone }
                           join t4 in db.Patients on t1.PatientId equals t4.PatientId into patientOuterJoin
                           from poj in patientOuterJoin.DefaultIfEmpty()
                           select
                           new
                           {
                               requests = t1,
                               userCreateRequest = t2.userCreateRequest,
                               createrProfile = t2.userProfile,
                               caller = t3,
                               patient = poj == null ? null : poj
                           })
                      join t5 in completer on t0.requests.RequestId equals t5.userCompleteRequest.RequestID into completerOuterJoin
                      from coj in completerOuterJoin.DefaultIfEmpty()
                      select new
                      {
                          requests = t0.requests,
                          userCreateRequest = t0.userCreateRequest,
                          createrProfile = t0.createrProfile,
                          caller = t0.caller,
                          patient = t0.patient,
                          userCompleteRequest = coj.userCompleteRequest == null ? null : coj.userCompleteRequest,
                          completerProfile = coj.userCompleteRequest == null ? null : coj.userProfile
                      }).Distinct().OrderByDescending(t => t.userCreateRequest.TimeCreated);

            List<RequestViewModel> result = new List<RequestViewModel>();
            foreach (var i in rs)
            {
                RequestViewModel rm = new RequestViewModel();
                rm.request = i.requests;
                rm.patient = i.patient;
                rm.userCreateRequest = i.userCreateRequest;
                rm.createrProfile = i.createrProfile;


                rm.userCompleteRequest = i.userCompleteRequest;
                rm.completerProfile = i.completerProfile;


                rm.caller = i.caller;
                result.Add(rm);
            }
            globalModel.requests = result;

            return View("Viewer", globalModel);
        }

        [HttpPost]
        public ActionResult filterRequestByKeyword(DISpecialistModel m)
        {

            DISpecialistContext db = new DISpecialistContext();
            string key;
            if (m.searchModel.searchKey == null)
                key = "";
            else key = m.searchModel.searchKey;


            var creater = from t1 in db.UserCreateRequest
                          join t2 in db.Users on t1.UserId equals t2.UserId
                          select new { userCreateRequest = t1, userProfile = t2 };
            var completer = from t1 in db.UserCompleteRequest
                            join t2 in db.Users on t1.UserID equals t2.UserId
                            select new { userCompleteRequest = t1, userProfile = t2 };


            var qs = (from t0 in
                          (from t0 in
                               ((from t1 in db.Keywords
                                 where t1.Keyword.Contains(key)
                                 join t2 in db.QuestionKeywords on t1.Keyword equals t2.Keyword
                                 join t3 in db.Questions on t2.QuestionId equals t3.QuestionId
                                 join t4 in db.Requests on t3.RequestId equals t4.RequestId
                                 select new { t4.RequestId }).Distinct())
                           join t1 in db.Requests on t0.RequestId equals t1.RequestId
                           join t2 in creater on t0.RequestId equals t2.userCreateRequest.RequestId
                           join t8 in completer on t1.RequestId equals t8.userCompleteRequest.RequestID
                           join t3 in db.Callers on new { t1.Name, t1.Phone } equals new { t3.Name, t3.Phone }
                           join t4 in db.Patients on t1.PatientId equals t4.PatientId into patientOuterJoin
                           from poj in patientOuterJoin.DefaultIfEmpty()
                           select
                           new
                           {
                               requests = t1,
                               userCreateRequest = t2.userCreateRequest,
                               createrProfile = t2.userProfile,
                               caller = t3,
                               patient = poj == null ? null : poj
                           })
                      join t5 in completer on t0.requests.RequestId equals t5.userCompleteRequest.RequestID into completerOuterJoin
                      from coj in completerOuterJoin.DefaultIfEmpty()
                      select new
                      {
                          requests = t0.requests,
                          userCreateRequest = t0.userCreateRequest,
                          createrProfile = t0.createrProfile,
                          caller = t0.caller,
                          patient = t0.patient,
                          userCompleteRequest = coj.userCompleteRequest == null ? null : coj.userCompleteRequest,
                          completerProfile = coj.userCompleteRequest == null ? null : coj.userProfile
                      }).OrderByDescending(t => t.userCreateRequest.TimeCreated);



            List<RequestViewModel> result = new List<RequestViewModel>();
            foreach (var i in qs)
            {
                RequestViewModel rm = new RequestViewModel();
                rm.request = i.requests;
                rm.patient = i.patient;
                rm.userCreateRequest = i.userCreateRequest;
                rm.createrProfile = i.createrProfile;


                rm.userCompleteRequest = i.userCompleteRequest;
                rm.completerProfile = i.completerProfile;


                rm.caller = i.caller;
                result.Add(rm);
            }
            globalModel.requests = result;

            return View("Viewer", globalModel);


        }

        public ActionResult Details(DISpecialistModel m, int rid)
        {
            //globalModel.editModel = newModel;
            DISpecialistContext db = new DISpecialistContext();
            //m.isCreatingRequest = true;
            m.editModel = new RequestViewModel();
            System.Diagnostics.Debug.WriteLine(rid);
            m.editModel.request = (from r in db.Requests
                                   where r.RequestId == rid
                                   select r).First();
            m.editModel.caller = (from c in db.Callers
                                  where c.Name.Equals(m.editModel.request.Name) && c.Phone.Equals(m.editModel.request.Phone)
                                  select c).First();
            m.editModel.patient = (from p in db.Patients
                                   where p.PatientId == m.editModel.request.PatientId
                                   select p).Count() == 0 ? null : (from p in db.Patients
                                                                    where p.PatientId == m.editModel.request.PatientId
                                                                    select p).First();
            m.editModel.newQuestion = (from q in db.Questions
                                       where q.RequestId == m.editModel.request.RequestId
                                       select q).First();

            m.editModel.newKeyword = (from qk in db.QuestionKeywords
                                      join k in db.Keywords on qk.Keyword equals k.Keyword
                                      where qk.QuestionId == m.editModel.newQuestion.QuestionId
                                      select k).First();

            m.editModel.newReference = (from qr in db.QuestionReferences
                                        join r in db.References on qr.ReferenceId equals r.ReferenceId
                                        where qr.QuestionId == m.editModel.newQuestion.QuestionId
                                        select r).First();

            System.Diagnostics.Debug.WriteLine(m.editModel.caller.Name);
            //System.Diagnostics.Debug.WriteLine(m.editModel.request.RequestId);
            //System.Diagnostics.Debug.WriteLine(!m.isCreatingRequest);
            globalModel = m;
            return View("Viewer", globalModel);
        }


    }
}
