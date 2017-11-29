using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WhatToEat.Models;

namespace WhatToEat.Controllers
{
    public class RecipesController : ApiController
    {
        private AppDb db = new AppDb();

        // GET: api/Recipes
        public IEnumerable<Recipe> GetRecipes()
        {
            return db.Recipes.ToList();
        }

        // GET: api/Recipes/5
        [ResponseType(typeof(Recipe))]
        public IHttpActionResult GetRecipe(int id)
        {
            //Recipe recipe = db.Recipes.Find(id);
            var recipe = db.Recipes.Select(x =>
                new RecipeDTO()
                {
                    id = x.Id,
                    products = x.Products.Select(y => y.Id).ToList(),
                    images = x.Images.Select(y => y.Path).ToList(),
                    title = x.Name,
                    description = x.Description,
                    difficulty = x.Difficulty,
                    timeToPrepare = x.TimeToPrepare,
                    tags = x.Tags.Select(y => y.Name).ToList(),
                    estimatedCost = x.EstimatedCost,
                    portionCount = x.PortionCount

                }).SingleOrDefault(x => x.id == id);

            if (recipe == null)
            {
                return NotFound();
            }

            return Ok(recipe);
        }

        // PUT: api/Recipes/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutRecipe(int id, Recipe recipe)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != recipe.Id)
            {
                return BadRequest();
            }

            db.Entry(recipe).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecipeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Recipes
        [ResponseType(typeof(Recipe))]
        public IHttpActionResult PostRecipe(Recipe recipe)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Recipes.Add(recipe);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = recipe.Id }, recipe);
        }

        // DELETE: api/Recipes/5
        [ResponseType(typeof(Recipe))]
        public IHttpActionResult DeleteRecipe(int id)
        {
            Recipe recipe = db.Recipes.Find(id);
            if (recipe == null)
            {
                return NotFound();
            }

            db.Recipes.Remove(recipe);
            db.SaveChanges();

            return Ok(recipe);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RecipeExists(int id)
        {
            return db.Recipes.Count(e => e.Id == id) > 0;
        }
    }
}