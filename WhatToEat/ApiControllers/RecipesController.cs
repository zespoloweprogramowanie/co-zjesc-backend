using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.EnterpriseServices.Internal;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using WhatToEat.Core.Helpers;
using WhatToEat.Domain.Commands.Recipe;
using WhatToEat.Domain.Models;
using WhatToEat.Domain.Services;

namespace WhatToEat.ApiControllers
{
    public class RecipesController : ApiController
    {
        private IRecipesService _recipesService;
        //private AppDb db = new AppDb();

        public RecipesController()
        {
            _recipesService = new RecipesService(new AppDb());
        }

        // GET: api/Recipes
        [ResponseType(typeof(Recipe))]
        public async Task<IHttpActionResult> GetRecipes()
        {
            var list = await _recipesService.ListAsync();
            var recipes = list.Select(x =>
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

                });

            //if (recipes == null)
            //{
            //    return NotFound();
            //}

            return Ok(recipes);
        }

        // GET: api/Recipes/5
        [ResponseType(typeof(Recipe))]
        public async Task<IHttpActionResult> GetRecipe(int id)
        {
            var x = await _recipesService.GetRecipeForPreviewAsync(id);

            //Recipe recipe = db.Recipes.Find(id);
            var recipe =
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
                    tags = x.Tags.Select(y => new GetRecipeDTOTag { id = y.Id, name = y.Name }).ToList(),
                    estimatedCost = x.EstimatedCost,
                    portionCount = x.PortionCount
                };

            //if (recipe == null)
            //{
            //    return NotFound();
            //}

            return Ok(recipe);
        }

        // POST: api/GetRecipesByProducts
        [HttpPost]
        [Route("api/recipes/getRecipesByProducts")]
        [ResponseType(typeof(Recipe))]
        //[HttpOptions]
        public async Task<IHttpActionResult> GetRecipesByProducts(List<int> productIds)
        {
            //var recipes = db.Recipes.Include(x => x.Products).Where(x => x.Products.Any(y => productIds.Any(z => z == y.ProductId))).ToList();
            //var recipes = db.Recipes.ToList();
            var recipes = await _recipesService.GetRecipesByProductsAsync(productIds);
            return Ok(recipes.Select(x => new
            {
                id = x.Id,
                title = x.Name,
                image = ((x.Images.Count > 0) ? x.Images.FirstOrDefault().Path : "")
            }));
        }

        // PUT: api/Recipes/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutRecipe(int id, UpdateCommand recipe)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != recipe.id)
            {
                return BadRequest();
            }

            var updated = await _recipesService.UpdateRecipeAsync(recipe);
            return Ok(updated.Id);
            //db.Entry(recipe).State = EntityState.Modified;

            //try
            //{
            //    db.SaveChanges();
            //}
            //catch (DbUpdateConcurrencyException)
            //{
            //    if (!RecipeExists(id))
            //    {
            //        return NotFound();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}

            //return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Recipes
        [ResponseType(typeof(Recipe))]
        public async Task<IHttpActionResult> PostRecipe(CreateCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdRecipe = await _recipesService.CreateRecipeAsync(command);
            return Ok(createdRecipe.Id);
            //return CreatedAtRoute("DefaultApi", new { id = createdRecipe.Id }, createdRecipe);
        }

        //// DELETE: api/Recipes/5
        //[ResponseType(typeof(Recipe))]
        //public IHttpActionResult DeleteRecipe(int id)
        //{
        //    Recipe recipe = db.Recipes.Find(id);
        //    if (recipe == null)
        //    {
        //        return NotFound();
        //    }

        //    db.Recipes.Remove(recipe);
        //    db.SaveChanges();

        //    return Ok(recipe);
        //}

        //private bool RecipeExists(int id)
        //{
        //    return db.Recipes.Count(e => e.Id == id) > 0;
        //}

        [HttpPost]
        [Route("api/recipes/uploadRecipeImages")]
        [ResponseType(typeof(IEnumerable<string>))]
        //[HttpOptions]
        public IHttpActionResult UploadRecipeImages()
        {
            HttpResponseMessage result = null;

            var httpRequest = HttpContext.Current.Request;

            List<HttpPostedFile> files = new List<HttpPostedFile>();
            if (httpRequest.Files.Count > 0)
            {
                foreach (string file in httpRequest.Files)
                {
                    files.Add(httpRequest.Files[file]);
                }
            }

            var filePaths = _recipesService.UploadRecipeImages(files);
            return Ok(filePaths.Select(x => new UploadRecipeImagesResult()
            {
                relativeUrl = x,
                absoluteUrl = Url.Content(x)
            }
            ));
        }

        public class UploadRecipeImagesResult
        {
            public string relativeUrl { get; set; }
            public string absoluteUrl { get; set; }
        }
    }
}