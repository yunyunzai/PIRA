using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcApplication2.Models;

using System.IO;

using System.Web.Security;
using System.Xml.Serialization;

namespace MvcApplication2.Controllers
{
    public class DISpecialistController : Controller
    {
        
        
        public ActionResult search()
        {

            DISpecialistContext db = new DISpecialistContext();
            var rid = (from t1 in db.Requests
                       join t2 in db.Questions on t1.RequestId equals t2.RequestId
                       join t3 in db.QuestionKeywords on t2.QuestionId equals t3.QuestionId
                       select t1).Distinct().OrderByDescending(t1 => t1.RequestId);
            // @model IEnumerable<MvcApplication2.Models.Keywords>
            return PartialView(rid);
            
        }


        [HttpPost]
        public ActionResult search(DISpecialistModel m)
        {

            DISpecialistContext db = new DISpecialistContext();


            //var rid = (from t1 in db.Requests
            //       join t2 in db.Questions on t1.RequestId equals t2.RequestId
            //       join t3 in db.QuestionKeywords on t2.QuestionId equals t3.QuestionId
            //       where t3.Keyword.Equals(m.SearchModel.searchKey)
            //       select t1).Distinct().OrderByDescending(t1 => t1.RequestId);

            var rid = (from t1 in db.Requests
                       join t2 in db.Questions on t1.RequestId equals t2.RequestId
                       join t3 in db.QuestionKeywords on t2.QuestionId equals t3.QuestionId
                       select t1).Distinct().OrderByDescending(t1 => t1.RequestId);
            return PartialView(rid);
        }



    }
}