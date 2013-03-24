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
            //System.Diagnostics.Debug.WriteLine(globalModel.editModel.request==null);
            System.Diagnostics.Debug.WriteLine(m.editModel.newQuestions == null);
            if (ModelState.IsValid)
            {
                //System.Diagnostics.Debug.WriteLine(m.editModel.newQuestion.QuestionContent);
                DISpecialistContext db = new DISpecialistContext();

                // Updating Caller table 
                Caller caller = null; 
                var callers = from c in db.Callers
                             where c.Name.Equals(m.editModel.caller.Name) && c.Phone.Equals(m.editModel.caller.Phone)
                             select c;
                // if this caller exists in the database, update his/her info                               
                if (callers.Count() != 0)
                {
                    caller = callers.First();
                    caller.Email = m.editModel.caller.Email == null ? caller.Email : m.editModel.caller.Email;
                    caller.Region = m.editModel.caller.Region;
                    caller.TypeAbbreviate = m.editModel.caller.TypeAbbreviate;
                }
                // otherwise add a new caller in the database
                else
                {
                    caller = m.editModel.caller;
                    db.Callers.Add(caller);
                }
                db.SaveChanges();

                // Updating Patient table
                Patient patient=null;
                var patients = from p in db.Patients
                          where p.Name.Equals(m.editModel.patient.Name) && p.AgencyID == m.editModel.patient.AgencyID && p.Gender.Equals(m.editModel.patient.Gender)
                          select p;
                // if the user entered valid patient Name and agency ID modify or update the patient info
                if (m.editModel.patient.Name != null && m.editModel.patient.AgencyID != null)
                {
                    // if this patient exists in the database, update his/her info
                    if (patients.Count() != 0)
                    {
                        patient = patients.First();
                        patient.Name = m.editModel.patient.Name == null ? patient.Name : m.editModel.patient.Name;
                        patient.AgencyID = m.editModel.patient.AgencyID == null ? patient.AgencyID : m.editModel.patient.AgencyID;
                        patient.Age = m.editModel.patient.Age == null ? patient.Age : m.editModel.patient.Age;
                        patient.Gender = m.editModel.patient.Gender == null ? patient.Gender : m.editModel.patient.Gender;
                    }
                    // otherwise add a new patient in the database
                    else
                    {
                        patient = m.editModel.patient;
                        db.Patients.Add(m.editModel.patient);
                    }
                }
                db.SaveChanges();

                // Updating the request table
                Request request = null;
                // if the user is currently editing the request
                if (globalModel.editModel.request != null)
                {
                    request = db.Requests.Find(globalModel.editModel.request.RequestId);
                    request.Name = caller.Name;
                    request.Phone = caller.Phone;
                    if (patient == null)
                        request.PatientId = null;
                    else
                        request.PatientId = patient.PatientId; 
                }
                else
                {
                    request = new Request();
                    request.IsActive = true;
                    request.TotalTimeSpent = 0;
                    request.Name = m.editModel.caller.Name;
                    request.Phone = m.editModel.caller.Phone;
                    if (patient==null)
                        request.PatientId = null;
                    else
                        request.PatientId = patient.PatientId;

                    db.Requests.Add(request);
                }
                db.SaveChanges();

                //// Updating Question table
                //Question question=null;
                //var questions = (from qs in db.Questions
                //             where qs.QuestionContent.Equals(m.editModel.newQuestion.QuestionContent) && qs.RequestId==request.RequestId
                //             select qs);
                //// question exists in the database
                //if (questions.Count()!=0)
                //{
                //    question = questions.First();
                //    question.QuestionContent = m.editModel.newQuestion.QuestionContent;
                //    question.Response = m.editModel.newQuestion.Response;
                //    question.Severity = m.editModel.newQuestion.Severity;
                //    question.Probability = m.editModel.newQuestion.Probability;
                //    question.TumorTypeAbbreviate = m.editModel.newQuestion.TumorTypeAbbreviate;
                //    question.QuestionTypeAbbreviate = m.editModel.newQuestion.QuestionTypeAbbreviate;
                    
                //}
                //else
                //{
                //    question = m.editModel.newQuestion;
                //    question.RequestId = request.RequestId;
                //    db.Questions.Add(question);
                //}
                //System.Diagnostics.Debug.WriteLine("QID: "+question.QuestionId);
                //db.SaveChanges();


                //// update keyword tables
                //var keywords = from k in db.Keywords
                //         where k.Keyword.Equals(m.editModel.newKeyword.Keyword)
                //         select k;

                
                //Keywords keyword = null;
                //// if the keyword exists in the database
                //if (keywords.Count() != 0)
                //{
                //    keyword = keywords.First();                    
                //}
                //// if the keyword is not in the database, add the keyword to the database
                //else 
                //{                    
                //    keyword = new Keywords();
                //    keyword.Keyword = m.editModel.newKeyword.Keyword;
                //    keyword.IsActive = true;
                //    db.Keywords.Add(keyword);
                //    db.SaveChanges();
                //}

                //// add the keyword reference 
                //QuestionKeyword keywordRef=null;
                //var keywordRefs = from k in db.QuestionKeywords
                //         where k.Keyword.Equals(keyword.Keyword) && k.QuestionId==question.QuestionId
                //         select k;
                //// if the keyword reference exists in the db
                //if (keywordRefs.Count() != 0)
                //{
                //    keywordRef = keywordRefs.First();
                //}
                //// otherwise create new keyword reference
                //else 
                //{
                //    keywordRef = new QuestionKeyword();
                //    keywordRef.Keyword = m.editModel.newKeyword.Keyword;
                //    keywordRef.QuestionId = question.QuestionId;
                //    db.QuestionKeywords.Add(keywordRef);
                //}
                //// otherwise do nothing
                //db.SaveChanges();

                //// update reference tables
                //Reference reference=null;
                
                //if (m.editModel.newReference.ReferenceContent!=null)
                //{

                //    var refs = from r in db.References
                //               where r.ReferenceContent.Equals(m.editModel.newReference.ReferenceContent)
                //               select r;
                //    // if the reference exists in the db
                //    if (refs.Count() != 0)
                //    {
                //        reference = refs.First();
                //    }
                //    // otherwise add new reference
                //    else
                //    {
                //        // add reference content 
                //        reference = new Reference();
                //        reference.ReferenceContent = m.editModel.newReference.ReferenceContent;
                //        db.References.Add(reference);
                //        db.SaveChanges();                        
                //    }


                //    // add question reference if it does not exist in the db
                //    QuestionReference qrf = null;
                //    var qrfs = from r in db.QuestionReferences
                //               where r.ReferenceId==reference.ReferenceId && r.QuestionId==question.QuestionId
                //               select r;
                //    // if the reference does not exists in the db add one!
                //    if (qrfs.Count() == 0)
                //    {
                //        qrf = new QuestionReference();
                //        qrf.QuestionId = question.QuestionId;
                //        qrf.ReferenceId = reference.ReferenceId;
                //        db.QuestionReferences.Add(qrf);
                //    }
                //    // else do nothing                
                //    db.SaveChanges();
                //}


                // remove all the old questions relationship
                
                var questions = from qs in db.Questions
                                 where qs.RequestId == request.RequestId
                                 select qs;
                List<Question> tempQs=questions.ToList();
                foreach (Question q in tempQs)
                {
                    // remove keyword refs
                    var keywordRefs = from k in db.QuestionKeywords
                                      where k.QuestionId == q.QuestionId
                                      select k;
                    foreach (QuestionKeyword qk in keywordRefs)
                    {
                        db.QuestionKeywords.Remove(qk);
                    }
                    db.SaveChanges();

                    // remove reference refs
                    var qrfs = from r in db.QuestionReferences
                               where r.QuestionId == q.QuestionId
                               select r;
                    foreach (QuestionReference qrf in qrfs)
                    {
                        db.QuestionReferences.Remove(qrf);
                    }
                    db.SaveChanges();

                    db.Questions.Remove(q);
                }
                db.SaveChanges();
                // update all questions
                for (int numQuestion = 0; numQuestion < m.editModel.newQuestions.Count(); numQuestion++)                
                {               
                    // Updating Question table
                    Question question = null;
                    Question tempQ=m.editModel.newQuestions[numQuestion];
                    questions = (from qs in db.Questions
                                     where qs.QuestionContent.Equals(tempQ.QuestionContent) && qs.RequestId == request.RequestId
                                     select qs);
                    // question exists in the database
                    if (questions.Count() != 0)
                    {
                        question = questions.First();
                        question.QuestionContent = m.editModel.newQuestions[numQuestion].QuestionContent;
                        question.Response = m.editModel.newQuestions[numQuestion].Response;
                        question.Severity = m.editModel.newQuestions[numQuestion].Severity;
                        question.Probability = m.editModel.newQuestions[numQuestion].Probability;
                        question.TumorTypeAbbreviate = m.editModel.newQuestions[numQuestion].TumorTypeAbbreviate;
                        question.QuestionTypeAbbreviate = m.editModel.newQuestions[numQuestion].QuestionTypeAbbreviate;

                    }
                    else
                    {
                        question = m.editModel.newQuestions[numQuestion];
                        question.RequestId = request.RequestId;
                        db.Questions.Add(question);
                    }
                   // System.Diagnostics.Debug.WriteLine("QID: " + question.QuestionId);
                    db.SaveChanges();

                    string keywordsComposite = m.editModel.newKeywords[numQuestion];
                    string[] separator = {"|"};
                    string[] userKeywords = keywordsComposite.Split(separator,StringSplitOptions.RemoveEmptyEntries);

                    foreach (string newKeyword in userKeywords)
                    {
                        // update keyword tables
                        var keywords = from k in db.Keywords
                                       where k.Keyword.Equals(newKeyword)
                                       select k;


                        Keywords keyword = null;
                        // if the keyword exists in the database
                        if (keywords.Count() != 0)
                        {
                            keyword = keywords.First();
                        }
                        // if the keyword is not in the database, add the keyword to the database
                        else
                        {
                            keyword = new Keywords();
                            keyword.Keyword = newKeyword;
                            keyword.IsActive = true;
                            db.Keywords.Add(keyword);
                            db.SaveChanges();
                        }

                        // add the keyword reference 
                        QuestionKeyword keywordRef = null;
                        var keywordRefs = from k in db.QuestionKeywords
                                          where k.Keyword.Equals(keyword.Keyword) && k.QuestionId == question.QuestionId
                                          select k;
                        // if the keyword reference exists in the db
                        if (keywordRefs.Count() != 0)
                        {
                            keywordRef = keywordRefs.First();
                        }
                        // otherwise create new keyword reference
                        else
                        {
                            keywordRef = new QuestionKeyword();
                            keywordRef.Keyword = newKeyword;
                            keywordRef.QuestionId = question.QuestionId;
                            db.QuestionKeywords.Add(keywordRef);
                        }
                        // otherwise do nothing
                        db.SaveChanges();
                    }


                    // update reference tables
                    Reference reference = null;

                    if (m.editModel.newReferences[numQuestion].ReferenceContent != null)
                    {
                        Reference tempRef=m.editModel.newReferences[numQuestion];
                        var refs = from r in db.References
                                   where r.ReferenceContent.Equals(tempRef.ReferenceContent)
                                   select r;
                        // if the reference exists in the db
                        if (refs.Count() != 0)
                        {
                            reference = refs.First();
                        }
                        // otherwise add new reference
                        else
                        {
                            // add reference content 
                            reference = new Reference();
                            reference.ReferenceContent = m.editModel.newReferences[numQuestion].ReferenceContent;
                            db.References.Add(reference);
                            db.SaveChanges();
                        }


                        // add question reference if it does not exist in the db
                        QuestionReference qrf = null;
                        var qrfs = from r in db.QuestionReferences
                                   where r.ReferenceId == reference.ReferenceId && r.QuestionId == question.QuestionId
                                   select r;
                        // if the reference does not exists in the db add one!
                        if (qrfs.Count() == 0)
                        {
                            qrf = new QuestionReference();
                            qrf.QuestionId = question.QuestionId;
                            qrf.ReferenceId = reference.ReferenceId;
                            db.QuestionReferences.Add(qrf);
                        }
                        // else do nothing                
                        db.SaveChanges();
                    }
                }


                // logging
                // if the user is creating request
                if (globalModel.editModel.request == null)
                {
                    UserCreateRequest ucr = new UserCreateRequest();
                    ucr.RequestId = request.RequestId;
                    ucr.UserId = long.Parse(Membership.GetUser().ProviderUserKey.ToString());
                    ucr.TimeCreated = DateTime.Now;
                    db.UserCreateRequest.Add(ucr);
                }
                // otherwise the user is editing and we update the editing logging table
                else
                {
                    UserEditRequest uer = null;
                    // find the previously unfinished edit
                    long uid=long.Parse(Membership.GetUser().ProviderUserKey.ToString());
                    var uers = from u in db.UserEditRequest
                               where u.RequestId == request.RequestId && u.UserId == uid && u.FinishTime == null
                               select u;
                    uer = uers.First();
                    uer.RequestId = request.RequestId;
                    uer.UserId = int.Parse(Membership.GetUser().ProviderUserKey.ToString());
                    uer.FinishTime= DateTime.Now;
                    //db.UserEditRequest.Add(uer);
                }
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
            globalModel.editModel.newQuestions=new List<Question>();
            globalModel.editModel.newQuestions.Add(new Question());
            globalModel.editModel.newReferences = new List<Reference>();
            globalModel.editModel.newReferences.Add(new Reference());
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
            //m.editModel.newQuestion = (from q in db.Questions
            //                           where q.RequestId == m.editModel.request.RequestId
            //                           select q).First();

            var qs = from q in db.Questions
                                       where q.RequestId == m.editModel.request.RequestId
                                       select q;
            
            m.editModel.newQuestions = new List<Question>();
            m.editModel.newReferences = new List<Reference>();
            m.editModel.newKeywords = new List<string>();
            foreach (Question q in qs)
            {
                m.editModel.newQuestions.Add(q);
                
            }
            foreach (Question q in m.editModel.newQuestions)
            {
                var refs = from qr in db.QuestionReferences
                           join r in db.References on qr.ReferenceId equals r.ReferenceId
                       where qr.QuestionId==q.QuestionId 
                       select r;
                m.editModel.newReferences.Add(refs.Count()==0?new Reference():refs.First());
                var ks = from qk in db.QuestionKeywords
                           join k in db.Keywords on qk.Keyword equals k.Keyword
                           where qk.QuestionId == q.QuestionId
                           select k;

                string keys = "";
                foreach (Keywords k in ks)
                {
                    //m.editModel.newKeywords.Add(k);
                    keys += k.Keyword + "|";
                } 
                m.editModel.newKeywords.Add(keys);
               
            }

            

            //System.Diagnostics.Debug.WriteLine(m.editModel.caller.Name);
             //System.Diagnostics.Debug.WriteLine(m.editModel.request.RequestId);

            // log this edit history
            long uid = long.Parse(Membership.GetUser().ProviderUserKey.ToString()); 
            UserEditRequest uer = null;
            // find previously opened edit log
            var uers = from u in db.UserEditRequest
                       where u.RequestId == rid && u.UserId == uid && u.FinishTime == null
                       select u;
            // if the un finished log for edit exists
            if (uers.Count() != 0)
                uer = uers.First();
            else
            {
                uer = new UserEditRequest();
                uer.RequestId = rid;
                uer.UserId = uid;
                uer.StartTime = DateTime.Now;
                uer.FinishTime = null;
                db.UserEditRequest.Add(uer);
                db.SaveChanges();
            }


            globalModel.editModel = m.editModel;
            globalModel.isEditorOpen = true;
            return View("DISpecialist", globalModel);
        }

        public ActionResult cancel(DISpecialistModel m)
        {
            DISpecialistContext db = new DISpecialistContext();
            // delete the edit log if the user cancels the edit action
            if (globalModel.editModel.request != null)
            {
                long uid = long.Parse(Membership.GetUser().ProviderUserKey.ToString());
                var uers = from u in db.UserEditRequest
                           where u.RequestId == globalModel.editModel.request.RequestId && u.UserId ==uid  && u.FinishTime == null
                           select u;
                db.UserEditRequest.Remove(uers.First());
                db.SaveChanges();
            }
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
                if (i.requests.IsActive)
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

        public ActionResult addQuestion(DISpecialistModel m)
        {
            
            globalModel.editModel.newQuestions.Add(new Question());
            globalModel.editModel.newReferences.Add(new Reference());
           // globalModel.editModel.newKeywords.Add(new Keywords());
            return View("DISpecialist", globalModel);
        }

        public ActionResult removeQuestion(DISpecialistModel m)
        {
            if (globalModel.editModel.newQuestions.Count() - 1 > 0)
            {
                int removeIndex = globalModel.editModel.newQuestions.Count() - 1;
                globalModel.editModel.newQuestions.RemoveAt(removeIndex);
                globalModel.editModel.newReferences.RemoveAt(removeIndex);
                globalModel.editModel.newKeywords.RemoveAt(removeIndex);
            }
            return View("DISpecialist", globalModel);
        }

    }
}