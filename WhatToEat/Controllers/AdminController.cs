using System.Linq;
using System.Web.Mvc;
using WhatToEat.Domain.Models;

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
        //Dodaj Recipe
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

        public ActionResult Products()
        {
            var products = db
                .Products
                .OrderBy(x => x.Id)
                .ToList();

            return View(products);
        }
    }
}