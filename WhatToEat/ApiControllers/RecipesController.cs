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
    [Authorize]
    public class RecipesController : ApiController
    {
        private IRecipesService _recipesService;
        //private AppDb db = new AppDb();

        public RecipesController()
        {
            _recipesService = new RecipesService(new AppDb());
        }

        // GET: api/Recipes
        [ResponseType(typeof(GetRecipesByProductsModel))]

        [Route("api/recipes")]
        public async Task<IHttpActionResult> GetRecipes(int? category = null)
        {


            //var list = await _recipesService.ListAsync();
            //var recipes = list.Select(x =>
            //    new
            //    {
            //        id = x.Id,
            //        products = x.Products.Select(y => y.Id).ToList(),
            //        images = x.Images.Select(y => y.Path).ToList(),
            //        title = x.Name,
            //        description = x.Description,
            //        difficulty = x.Difficulty,
            //        timeToPrepare = x.TimeToPrepare,
            //        tags = x.Tags.Select(y => y.Name).ToList(),
            //        estimatedCost = x.EstimatedCost,
            //        portionCount = x.PortionCount

            //    });

            //if (recipes == null)
            //{
            //    return NotFound();
            //}

            var recipes = await _recipesService.GetRecipesByFilters(category);
            var output = recipes.Select(x => new GetRecipesByProductsModel()
            {
                id = x.Id,
                title = x.Name,
                image = ((x.Images.Count > 0) ? Url.Content(x.Images.FirstOrDefault().Path) : "")
            });
            return Ok(output);
        }

        // GET: api/Recipes?search='fragment tytułu'
        [HttpGet]
        [AllowAnonymous]
        [ResponseType(typeof(Recipe))]
        public async Task<IHttpActionResult> GetRecipes(string search)
        {
            var list = await _recipesService.GetRecipesForTitle(search);

            return Json(list);
        }

        [HttpGet]
        [Route("api/recipes/getRecipesToAccept")]
        [ResponseType(typeof(Recipe))]
        public async Task<IHttpActionResult> GetRecipesToAccept()
        {
            var list = await _recipesService.GetRecipesToAccept();

            return Json(list);
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
                    images = x.Images.Select(y => new UploadRecipeImagesResult()
                    {
                        relativeUrl = y.Path,
                        absoluteUrl = Url.Content(y.Path)
                    }).ToList(),
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

        class GetRecipesByProductsModel
        {
            public int id { get; set; }
            public string title { get; set; }
            public string image { get; set; }
        }

        // POST: api/GetRecipesByProducts
        [HttpPost]
        [Route("api/recipes/getRecipesByProducts")]
        [ResponseType(typeof(GetRecipesByProductsModel))]
        //[HttpOptions]
        public async Task<IHttpActionResult> GetRecipesByProducts(List<int> productIds)
        {
            var recipes = await _recipesService.GetRecipesByProductsAsync(productIds);

            return Ok(recipes.Select(x => new GetRecipesByProductsModel()
            {
                id = x.Id,
                title = x.Name,
                image = ((x.Images.Count > 0) ? Url.Content(x.Images.FirstOrDefault().Path) : "")
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
        }

        // POST: api/Recipes
        [ResponseType(typeof(int))]
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

        // DELETE: api/Recipes/5
        public async Task<IHttpActionResult> DeleteRecipe(int id)
        {
            await _recipesService.DeleteAsync(id);
            return Ok();
        }

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

        [HttpGet]
        [Route("api/recipes/getMyRecipes")]
        public async Task<IHttpActionResult> GetMyRecipes()
        {
            var list = await _recipesService.GetMyRecipes();

            return Ok(list);
        }

    }
}