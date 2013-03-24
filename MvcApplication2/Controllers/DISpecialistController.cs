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
    public class DISpecialistController : Controller
    {
        private static DISpecialistModel globalModel=new DISpecialistModel();
        public ActionResult DISpecialist()
        {
            return View(globalModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[ValidateInput(true)]
        public ActionResult requestSave(DISpecialistModel m)
        {
            System.Diagnostics.Debug.WriteLine(globalModel.editModel.request.RequestId);
           
            if (ModelState.IsValid)
            {
                System.Diagnostics.Debug.WriteLine(m.editModel.newQuestion.QuestionContent);
                DISpecialistContext db = new DISpecialistContext();

                // Updating Caller table
                var caller = from c in db.Callers
                             where c.Name == m.editModel.caller.Name && c.Phone == m.editModel.caller.Phone
                             select c;
                if (caller.Count() != 0)
                {
                    Caller c = caller.First();
                    c.Email = m.editModel.caller.Email == null ? c.Email : m.editModel.caller.Email;
                    c.Region = m.editModel.caller.Region;
                    c.TypeAbbreviate = m.editModel.caller.TypeAbbreviate;
                }
                else
                    db.Callers.Add(m.editModel.caller);
                db.SaveChanges();

                // Updating Patient table
                IQueryable<Patient> patient=null;
                if (m.editModel.patient.Name != null && m.editModel.patient.AgencyID != null)
                {
                    patient = from p in db.Patients
                                  where p.Name.Equals(m.editModel.patient.Name) && p.AgencyID == m.editModel.patient.AgencyID && p.Gender.Equals( m.editModel.patient.Gender)
                                  select p;
                    if (patient.Count() != 0)
                    {
                        Patient p = patient.First();
                        p.Name = m.editModel.patient.Name == null ? p.Name : m.editModel.patient.Name;
                        p.AgencyID = m.editModel.patient.AgencyID == null ? p.AgencyID : m.editModel.patient.AgencyID;
                        p.Age = m.editModel.patient.Age == null ? p.Age : m.editModel.patient.Age;
                        p.Gender = m.editModel.patient.Gender == null ? p.Gender : m.editModel.patient.Gender;
                    }
                    else
                        db.Patients.Add(m.editModel.patient);
                }
                db.SaveChanges();

                // Updating the request table
                Request r = null;
                if (m.editModel.request != null)
                {
                    r = db.Requests.Find(m.editModel.request.RequestId);
                    r.Name = m.editModel.caller.Name;
                    r.Phone = m.editModel.caller.Phone;
                    if (patient == null)
                        r.PatientId = null;
                    else
                        r.PatientId = patient.First().PatientId;                    
                }
                else
                {
                    r = new Request();
                    r.IsActive = true;
                    r.TotalTimeSpent = null;
                    r.Name = m.editModel.caller.Name;
                    r.Phone = m.editModel.caller.Phone;
                    if (patient==null)
                        r.PatientId = null;
                    else
                        r.PatientId = patient.First().PatientId;    
                    
                    db.Requests.Add(r);
                }
                db.SaveChanges();

                // Updating Question table
                //Question q=null;
                //if (m.editModel.mode!=null &&m.editModel.mode.Equals("edit"))
                //{
                //    q = (from qs in db.Questions
                //             where qs.QuestionId == m.editModel.newQuestion.QuestionId
                //             select qs).First();
                //    q.QuestionContent = m.editModel.newQuestion.QuestionContent;
                //    q.Response = m.editModel.newQuestion.Response;
                //    q.Severity = m.editModel.newQuestion.Severity;
                //    q.Probability= m.editModel.newQuestion.Probability;                    
                //    q.TumorTypeAbbreviate = m.editModel.newQuestion.TumorTypeAbbreviate;
                //    q.QuestionTypeAbbreviate = m.editModel.newQuestion.QuestionTypeAbbreviate;
                //}
                //else
                //{
                Question q = m.editModel.newQuestion;
                    q.RequestId = r.RequestId;
                    db.Questions.Add(q);
                //}

                db.SaveChanges();

                // update keyword tables
                var ks = from k in db.Keywords
                         where k.Keyword.Equals(m.editModel.newKeyword.Keyword)
                         select k.Keyword;
                if (ks.Count()==0)
                {
                    // add keyword if it does not exist in the db
                    Keywords key=new Keywords();
                    key.Keyword = m.editModel.newKeyword.Keyword;
                    key.IsActive = true;
                    db.Keywords.Add(key);
                    db.SaveChanges();                   
                }

                // add reference if it does not exist in the db
                var qk = from k in db.QuestionKeywords
                         where k.Keyword.Equals(m.editModel.newKeyword.Keyword)
                         select k.QuestionId;
                if (!qk.Contains(q.QuestionId))
                {
                    QuestionKeyword qks = new QuestionKeyword();
                    qks.Keyword = m.editModel.newKeyword.Keyword;
                    qks.QuestionId = q.QuestionId;
                    db.QuestionKeywords.Add(qks);
                }
                db.SaveChanges();

                // update reference tables
                if (m.editModel.newReference.ReferenceContent!=null)
                {
                    // add reference content 
                    Reference reference = new Reference();
                    reference.ReferenceContent = m.editModel.newReference.ReferenceContent;

                    db.References.Add(reference);
                    db.SaveChanges();
                

                // add question reference if it does not exist in the db
                
                    QuestionReference qrf = new QuestionReference();
                    qrf.QuestionId = q.QuestionId;
                    qrf.ReferenceId = reference.ReferenceId;
                    db.QuestionReferences.Add(qrf);
                
                    db.SaveChanges();
                }
                // logging
                //if (m.editModel.mode == null || !m.editModel.mode.Equals("edit"))
                //{
                    UserCreateRequest ucr = new UserCreateRequest();
                    ucr.RequestId = r.RequestId;
                    ucr.UserId = int.Parse(Membership.GetUser().ProviderUserKey.ToString());
                    ucr.TimeCreated = DateTime.Now;
                    db.UserCreateRequest.Add(ucr);
                //}
                db.SaveChanges();
                globalModel.isEditorOpen = false;
                globalModel.editModel = new RequestViewModel();
                return View("DISpecialist", globalModel);
            }
            //System.Diagnostics.Debug.WriteLine(globalModel.editModel.newQuestion.QuestionContent);
            return View("DISpecialist", globalModel);
        }


        public ActionResult create(DISpecialistModel m)
        {
            globalModel.editModel = new RequestViewModel();
            globalModel.isEditorOpen = true;
            return View("DISpecialist", globalModel);
        }

        public ActionResult edit(DISpecialistModel m, int rid)
        {
            //globalModel.editModel = newModel;
            DISpecialistContext db = new DISpecialistContext();
            
            m.editModel = new RequestViewModel();
            //System.Diagnostics.Debug.WriteLine(rid);
            m.editModel.request=(from r in db.Requests
                                where r.RequestId==rid
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
            System.Diagnostics.Debug.WriteLine(m.editModel.caller.Name);
             //System.Diagnostics.Debug.WriteLine(m.editModel.request.RequestId);
            
            globalModel.editModel = m.editModel;
            globalModel.isEditorOpen = true;
            return View("DISpecialist", globalModel);
        }

        public ActionResult cancel(DISpecialistModel m)
        {
            globalModel.editModel= new RequestViewModel();
            globalModel.isEditorOpen = false;
            return View("DISpecialist", globalModel);
        }

        [HttpPost]
        public ActionResult view(DISpecialistModel m)
        {

            DISpecialistContext db = new DISpecialistContext();

            var creater = from t1 in db.UserCreateRequest
                          join t2 in db.Users on t1.UserId equals t2.UserId
                          select new { userCreateRequest = t1, userProfile=t2 };
            var completer = from t1 in db.UserCompleteRequest 
                            join t2 in db.Users on t1.UserID equals t2.UserId
                            select new { userCompleteRequest = t1, userProfile = t2 };
            

            var rs = (from t0 in (from t1 in db.Requests                      
                      join t2 in creater   on t1.RequestId equals t2.userCreateRequest.RequestId 
                      join t3 in db.Callers on new { t1.Name, t1.Phone } equals new { t3.Name, t3.Phone } 
                      join t4 in db.Patients on t1.PatientId equals t4.PatientId into patientOuterJoin
                      from poj in patientOuterJoin.DefaultIfEmpty()
                      select 
                      new {
                          requests=t1, 
                          userCreateRequest=t2.userCreateRequest, 
                          createrProfile=t2.userProfile,
                          caller = t3,
                          patient = poj == null? null:poj
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
            
            List<RequestViewModel> result=new List<RequestViewModel>();
            foreach (var i in rs)
            {
                RequestViewModel rm=new RequestViewModel();
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

            return View("DISpecialist", globalModel);
        }
        
        [HttpPost]
        public ActionResult filterRequestByKeyword(DISpecialistModel m)
        {
               // if (globalModel.requests!=null)
            //System.Diagnostics.Debug.WriteLine(m.searchModel.searchKey == null);

            
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


            var qs = (from t0 in (from t0 in 
                                      ((from t1 in db.Keywords
                                       where t1.Keyword.Contains(key)
                                       join t2 in db.QuestionKeywords on t1.Keyword equals t2.Keyword
                                       join t3 in db.Questions on t2.QuestionId equals t3.QuestionId
                                       join t4 in db.Requests on t3.RequestId equals t4.RequestId
                                        select new { t4.RequestId}).Distinct())
                        join t1 in db.Requests on t0.RequestId equals t1.RequestId
                      join t2 in creater   on t0.RequestId equals t2.userCreateRequest.RequestId 
                      join t3 in db.Callers on new { t1.Name, t1.Phone } equals new { t3.Name, t3.Phone } 
                      join t4 in db.Patients on t1.PatientId equals t4.PatientId into patientOuterJoin
                      from poj in patientOuterJoin.DefaultIfEmpty()
                      select 
                      new {
                          requests=t1, 
                          userCreateRequest=t2.userCreateRequest, 
                          createrProfile=t2.userProfile,
                          caller = t3,
                          patient = poj == null? null:poj
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

            return View("DISpecialist", globalModel);
        }



    }
}