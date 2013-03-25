using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcApplication2;
using MvcApplication2.Controllers;
using MvcApplication2.Models;

namespace PIRAunitTest
{
    [TestClass]
    public class AdminControllerTest
    {
        DISpecialistContext db = new DISpecialistContext();


        [TestMethod]
        public void TestMethod1() 
        {
            AdminController controller = new AdminController();

            // Act
            var temp = controller.Admin("") as ViewResult; 
            var result = (List<Keywords>)temp.ViewData.Model;

            // must be a collection of keyword
            //Assert.AreEqual(true , result.GetType == typeof());
            Assert.AreEqual(true, result.GetType() == typeof(List<Keywords>));
        }

        public void TestMethod()
        {
            AdminController controller = new AdminController();

            // Act 
            var temp = controller.RequestManagement("testkeyworda") as ViewResult;
            var result = (List<Request>)temp.ViewData.Model;

            // must be a collection of keyword
            //Assert.AreEqual(true , result.GetType == typeof());
            Assert.AreEqual(true, result.Count()!= 0);
        }

        [TestMethod]
        public void TestMethod2()
        {
            AdminController controller = new AdminController();

            // Act
            ViewResult temp = controller.ViewHistory(0) as ViewResult;

            // must be a collection of keyword
            Assert.IsNotNull(temp);
        }

        [TestMethod]
        public void TestMethod22()
        {
            AdminController controller = new AdminController();

            // Act
            var temp = controller.TumorGroup("") as ViewResult;
            var result = (List<TumorGroup>)temp.ViewData.Model;


            // must be a collection of keyword
            Assert.AreEqual(true,result.Count > 0);
        }

        [TestMethod]
        public void TestMethod3()
        {
            AdminController controller = new AdminController();

            Keywords test = new Keywords
            {
                Keyword = "testkeyok",
                IsActive = false
            };

            //db.Keywords.Remove(db.Keywords.Find("testkeyok"));
            //db.SaveChanges();
            // Act
            var temp = (RedirectToRouteResult)controller.AddKeyword(test);
            db.SaveChanges();

            db.Keywords.Remove(db.Keywords.Find("testkeyok"));
            db.SaveChanges();

            // must be a collection of keyword
            Assert.AreEqual("Admin", temp.RouteValues["controller"]);  // null? imposb!!!!!!!!! fix it plz!
            Assert.AreEqual("Admin", temp.RouteValues["action"]);
        }

        [TestMethod]
        public void TestMethod4()   // seems editkeyword does something weird 
        {
            AdminController controller = new AdminController();  

            // Act
            var temp = (HttpNotFoundResult)controller.EditKeyword("arbitarykeyword");  

            // must be a collection of keyword
            Assert.AreEqual(true, temp.GetType() == typeof(HttpNotFoundResult));
        }

        [TestMethod]
        public void TestMethod5a()
        {
            AdminController controller = new AdminController();  //
            // Arrange
            Keywords a = new Keywords
            {
                Keyword = "testkeywords",
                IsActive = false
            };
            try
            {
                db.Keywords.Remove(db.Keywords.Find("testkeywords"));
            }catch(Exception e){
            }
            db.SaveChanges();
            db.Keywords.Add(a);
            db.SaveChanges();

            // Act
            var temp = (RedirectToRouteResult)controller.EditKeyword("testkeywords", CreateProductTestFormCollection());
            db = new DISpecialistContext();

            // must be a collection of keyword
            Assert.AreEqual("Admin", temp.RouteValues["action"]);
            Assert.AreEqual(true, db.Keywords.Find("testKeywords").IsActive == true);

            // clean
            db.Keywords.Remove(db.Keywords.Find("testkeywords"));
            db.SaveChanges();
        }


        /*
         * for the purpose of mocking a formcollection object
         */ 
        private static FormCollection CreateProductTestFormCollection()
        {

            FormCollection form = new FormCollection();
            form.Add("activationStatus", "1");
            return form;
        }

    }
}

