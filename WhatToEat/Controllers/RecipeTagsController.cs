using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WhatToEat.Core.Extensions;
using WhatToEat.Domain.Models;

namespace WhatToEat.Controllers
{
    public class RecipeTagsController : Controller
    {
        private AppDb db = new AppDb();

        // GET: RecipeTags
        public async Task<ActionResult> Index()
        {
            return View(await db.RecipeTags.ToListAsync());
        }

        // GET: RecipeTags/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RecipeTag recipeTag = await db.RecipeTags.FindAsync(id);
            if (recipeTag == null)
            {
                return HttpNotFound();
            }
            return View(recipeTag);
        }

        // GET: RecipeTags/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RecipeTags/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name")] RecipeTag recipeTag)
        {
            if (ModelState.IsValid)
            {
                db.RecipeTags.Add(recipeTag);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(recipeTag);
        }

        // GET: RecipeTags/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RecipeTag recipeTag = await db.RecipeTags.FindAsync(id);
            if (recipeTag == null)
            {
                return HttpNotFound();
            }
            return View(recipeTag);
        }

        // POST: RecipeTags/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name")] RecipeTag recipeTag)
        {
            if (ModelState.IsValid)
            {
                db.Entry(recipeTag).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(recipeTag);
        }

        // GET: RecipeTags/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RecipeTag recipeTag = await db.RecipeTags.FindAsync(id);
            if (recipeTag == null)
            {
                return HttpNotFound();
            }
            return View(recipeTag);
        }

        // POST: RecipeTags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            RecipeTag recipeTag = await db.RecipeTags.FindAsync(id);
            db.RecipeTags.Remove(recipeTag);
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
