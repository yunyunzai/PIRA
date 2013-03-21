using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcApplication2.Controllers;
using System.Web.Mvc;
namespace DIRA.Test.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Index() as ViewResult;
            Assert.AreEqual("Welcome to the Drug Information Request Application.",result.ViewBag.Messages);
        }
    }
}
