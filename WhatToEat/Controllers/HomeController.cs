using System.Web.Mvc;

namespace WhatToEat.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Logs");
        }
    }
}