using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WhatToEat.Models;

namespace WhatToEat.Controllers
{
    public class AdminController : Controller
    {
        private AppDb db = new AppDb();

        // GET: Admin
        public ActionResult Recipe()
        {
            return View();
        }

        [HttpPost]
        //Create new Recipe
        public ActionResult Recipe(Recipe newRecipe)
        {

            if (ModelState.IsValid)
            {
                db.Recipes.Add(newRecipe);
                db.SaveChanges();

                return RedirectToAction("Recipe");
            }
            else
            {
                return View(newRecipe);
            }
        }
    }
}