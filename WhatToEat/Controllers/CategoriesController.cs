using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using WhatToEat.Core.Extensions;
using WhatToEat.Domain.Models;

namespace WhatToEat.Controllers
{
    public class CategoriesController : Controller
    {
        private AppDb db = new AppDb();

        // GET: RecipeCategories
        public async Task<ActionResult> Index()
        {
            var recipeCategories = db.RecipeCategories.Include(r => r.ParentCategory);
            return View(await recipeCategories.ToListAsync());
        }

        // GET: RecipeCategories/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RecipeCategory recipeCategory = await db.RecipeCategories.FindAsync(id);
            if (recipeCategory == null)
            {
                return HttpNotFound();
            }
            return View(recipeCategory);
        }

        // GET: RecipeCategories/Create
        public ActionResult Create()
        {
            ViewBag.ParentId = new SelectList(db.RecipeCategories, "Id", "Name");
            return View();
        }

        // POST: RecipeCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,ParentId")] RecipeCategory recipeCategory)
        {
            if (ModelState.IsValid)
            {
                db.RecipeCategories.Add(recipeCategory);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ParentId = new SelectList(db.RecipeCategories, "Id", "Name", recipeCategory.ParentId);
            return View(recipeCategory);
        }

        // GET: RecipeCategories/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RecipeCategory recipeCategory = await db.RecipeCategories.FindAsync(id);
            if (recipeCategory == null)
            {
                return HttpNotFound();
            }
            ViewBag.ParentId = new SelectList(db.RecipeCategories, "Id", "Name", recipeCategory.ParentId);
            return View(recipeCategory);
        }

        // POST: RecipeCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,ParentId")] RecipeCategory recipeCategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(recipeCategory).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ParentId = new SelectList(db.RecipeCategories, "Id", "Name", recipeCategory.ParentId);
            return View(recipeCategory);
        }

        // GET: RecipeCategories/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RecipeCategory recipeCategory = await db.RecipeCategories.FindAsync(id);
            if (recipeCategory == null)
            {
                return HttpNotFound();
            }
            return View(recipeCategory);
        }

        // POST: RecipeCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            RecipeCategory recipeCategory = await db.RecipeCategories.FindAsync(id);
            db.RecipeCategories.Remove(recipeCategory);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
