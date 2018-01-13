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


        /* - Dawid
            ExceptionMessage: "Self referencing loop detected for property 'Recipe' with type 'System.Data.Entity.DynamicProxies.Recipe_56F77C94E6E095D7461EDA0DD92F855A80F721CEBB0FFF163B390BED5EC5700F'. Path '[0].Images[0]'."
        // GET: api/Recipes
        public IEnumerable<Recipe> GetRecipes()
        {
            return db.Recipes.ToList();
        }
        */

        // GET: api/Recipes
        [ResponseType(typeof(Recipe))]
        public IHttpActionResult GetRecipes()
        {
            var recipes = db.Recipes.Select(x =>
                new
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

                }).ToList();

            if (recipes == null)
            {
                return NotFound();
            }

            return Ok(recipes);
        }

        // GET: api/Recipes/5
        [ResponseType(typeof(Recipe))]
        public IHttpActionResult GetRecipe(int id)
        {
            //Recipe recipe = db.Recipes.Find(id);
            var recipe = db.Recipes.Select(x =>
                new GetRecipeDTO()
                {
                    id = x.Id,
                    products = x.Products.Select(y => new GetRecipeDTOProduct
                    {
                        id = y.ProductId,
                        name = y.Product.Name,
                        unit = new GetRecipeDTOUnit()
                        {
                            id = y.Unit.Id,
                            label = y.Unit.Name
                        },
                        amount = y.NumberOfUnit
                    }).ToList(),
                    images = x.Images.Select(y => y.Path).ToList(),
                    title = x.Name,
                    description = x.Description,
                    difficulty = x.Difficulty,
                    timeToPrepare = x.TimeToPrepare,
                    tags = x.Tags.Select(y => new GetRecipeDTOTag { id= y.Id, name = y.Name }).ToList(),
                    estimatedCost = x.EstimatedCost,
                    portionCount = x.PortionCount
                }).SingleOrDefault(x => x.id == id);

            if (recipe == null)
            {
                return NotFound();
            }

            return Ok(recipe);
        }

        public void Options()
        {

        }

        // POST: api/GetRecipesByProducts
        [HttpPost]
        [Route("api/recipes/getRecipesByProducts")]
        [ResponseType(typeof(Recipe))]
        //[HttpOptions]
        public IHttpActionResult GetRecipesByProducts(List<int> productIds)
        {
            var recipes = db.Recipes.Include(x => x.Products).Where(x => x.Products.Any(y => productIds.Any(z => z == y.ProductId))).ToList();
            //var recipes = db.Recipes.ToList();
            return Ok(recipes.Select(x => new
            {
                id = x.Id,
                title = x.Name,
                image = ((x.Images.Count > 0) ? x.Images.FirstOrDefault().Path : "")
            }));
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