using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcApplication2;
using MvcApplication2.Controllers;
using MvcApplication2.Models;

namespace UnitTestProject1.ControllerTests
{
    [TestClass]
    public class UsersControllerTest
    {
        public static int USERID = -1;

        [TestMethod]
        public void TestMethod1()
        {

            // Arrange
            var controller = new UsersController();

            var result = controller.Index() as ViewResult;

            var userprofiles = (List<UserProfile>)result.ViewData.Model;

            // Assert
            Assert.IsNotNull(result, "Should have returned a ViewResult");

            foreach (var userprofile in userprofiles)
            {

                Assert.IsNotNull(userprofile.UserId, "userId is null?, not possible! somethings wrong");
                //Assert.AreEqual("Index", result.ViewName, "View name should have been {0}", "Index");
            }
        }


        [TestMethod]
        public void TestMethod2()
        {
            // Arrange
            var controller = new UsersController();


            USERID = 5;
            var userprofile = (UserProfile)((controller.Details(5) as ViewResult).ViewData.Model);

            Assert.IsNotNull(userprofile);
            Assert.AreEqual(userprofile.UserId, USERID);
            USERID = -1;


        }

        [TestMethod]
        public void TestMethod3()
        {
            // Arrange
            var controller = new UsersController();
            controller.ViewData.ModelState.AddModelError("key", "ErrorMessage");


            UserProfile temp = new UserProfile
            {
                UserId = 9876,
                UserName = "testCharles",
                IsActivated = true,
                Email = null,
                FirstName = "Charles",
                LastName = "test"
            };

            // act
            var userprofile = (UserProfile)((controller.Create(temp) as ViewResult).ViewData.Model);

            // assert
            Assert.IsNotNull(userprofile);
            Assert.AreEqual(userprofile, temp);
        }


        [TestMethod]
        public void TestMethod4()
        {
            // Arrange
            var controller = new UsersController();


            var temp = controller.Edit(1) as ViewResult;
            var userprofile = (UserProfile)((temp).ViewData.Model);

            Assert.IsNotNull(userprofile);
            Assert.AreEqual(userprofile.UserId, 1);

            UsersContext db = new UsersContext();
            // how to test a viewbag property in this case...?
            //Assert.IsNotNull(,temp.ViewData["AllAssignedGroups"]);



           // // Arrange
           // string[] selectedRoles = new string[4] {"Admin", "Viewer", "DISpecialist","Reporter"}; 
           // string[] selectedGroups = new string[2] {"UserGroup1", "UserGroup0"}; 
           // FormCollection form = CreateProductTestFormCollection();
           // controller.ValueProvider = form.ToValueProvider();
           // controller.ControllerContext = new ControllerContext();

           // // Act
           // var result = controller.Edit(1, form, selectedRoles, selectedGroups);


           // UserProfile final = db.UserProfiles.Find(1); 

           // UsersInRoles a = new UsersInRoles{ UserId = 1, RoleId = 2};
           // UsersInRoles b = new UsersInRoles { UserId = 1, RoleId = 3 };
           // UsersInRoles c = new UsersInRoles { UserId = 1, RoleId = 4 };
           // UsersInRoles d = new UsersInRoles { UserId = 1, RoleId = 5 };
           // Assert.AreEqual(true,final.UsersInRoles.Contains(a));
           // Assert.AreEqual(true, final.UsersInRoles.Contains(b));
           // Assert.AreEqual(true, final.UsersInRoles.Contains(c));
           // Assert.AreEqual(true, final.UsersInRoles.Contains(d));

           // // clean
           // selectedRoles = new string[2] { "Admin", "Viewer" };
           // selectedGroups = new string[2] { "UserGroup1", "UserGroup0" }; 
           //result = controller.Edit(1, null, selectedRoles, selectedGroups);
        }


        /*
         * for the purpose of mocking a formcollection object
         */
        private static FormCollection CreateProductTestFormCollection()
        {

            FormCollection form = new FormCollection();
            form.Add("anykey", "anything");
            return form;
        }

        //[TestMethod]
        //public void TestMethod5()    // the deleteconfirmed action should work right?? implementation erro?
        //{
        //    // Arrange
        //    var controller = new UsersController();


        //    // act
        //    var actionresult = (RedirectToRouteResult)controller.DeleteConfirmed(5);

        //    // assert
        //    Assert.AreEqual("Users", actionresult.RouteValues["controller"]);
        //    Assert.AreEqual("Index", actionresult.RouteValues["action"]);

        //    // need to recover@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        //}










    }





}

