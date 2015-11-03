using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using URL_Splitter.DAL;

namespace URL_Splitter.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
       
        [HttpPost]
        public ActionResult Index(string txtTagName, string txtUrl)
        {
            if (string.IsNullOrEmpty(txtTagName))
                txtTagName = "UnKnown";
            ViewBag.TagName = txtTagName;
            if (string.IsNullOrEmpty(txtUrl))
            {
                ViewBag.ControllerMessage = "Url Rejected";
                return View();
            }

            ViewBag.ControllerMessage = DBAccess.AddDocument(txtUrl, txtTagName);

            return View();
        }
    
    }
}