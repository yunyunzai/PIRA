﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcApplication2.Models;

using System.IO;
using NPOI.Util.Collections;


namespace MvcApplication2.Controllers
{
    public class DISpecialistController : Controller
    {
        private static DISpecialistModel globalModel=new DISpecialistModel();
        public ActionResult DISpecialist()
        {
            return View();
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

        //[HttpPost]
        //public ActionResult search(DISpecialistModel m)
        //{

        //    DISpecialistContext db = new DISpecialistContext();
        //    if (m.searchModel.searchBy.Equals("Keyword"))
        //    {
        //        string key=m.searchModel.searchKey
        //        var key = from m in db.Keywords select m;

        //        if (!String.IsNullOrEmpty(searchString))
        //        {
        //            key = key.Where(s => s.Keyword.Contains(searchString));
        //        }


            
        //        return View("DISpecialist",m);
        //        }
        //}
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
            var qs = (from t1 in db.Keywords
                       where t1.Keyword.Contains(key)
                       join t2 in db.QuestionKeywords on t1.Keyword equals t2.Keyword
                     select new { questionId = t2.QuestionId }).Distinct();
            foreach (var q in qs)
            {
                System.Diagnostics.Debug.WriteLine(q.questionId);
            }

            return View("DISpecialist", globalModel);
        }



    }
}