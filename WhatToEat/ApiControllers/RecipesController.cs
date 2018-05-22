using System;
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
using System.Web.Http.Routing;
using WhatToEat.Core.Extensions;
using WhatToEat.Core.Helpers;
using WhatToEat.Domain.Commands.Recipe;
using WhatToEat.Domain.Helpers;
using WhatToEat.Domain.Models;
using WhatToEat.Domain.Models.DTO;
using WhatToEat.Domain.Services;

namespace WhatToEat.ApiControllers
{
    [Authorize]
    public class CompactRecipe
    {
        public CompactRecipe(Recipe recipe)
        {

            Id = recipe.Id;
            Title = recipe.Name;
            Image = recipe.Images.Count > 0
                ? recipe.Images.FirstOrDefault()?.Path?.ToAbsoluteUrl()
                : "";
            AverageRate = recipe.AverageRate;
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public double AverageRate { get; set; }
    }


    public class Carousel
    {
        public Carousel()
        {
            Recipes = new List<CompactRecipe>();
        }

        public Carousel(string name, IEnumerable<Recipe> recipes) : base()
        {
            Name = name;
            Recipes = recipes.Select(x => new CompactRecipe(x));
        }

        public string Name { get; set; }

        public IEnumerable<CompactRecipe> Recipes { get; set; }

    }


    public class RecipesController : ApiController
    {
        private readonly IRecipesService _recipesService;

        public RecipesController()
        {
            _recipesService = new RecipesService(new AppDb());
        }



        /// <summary>
        /// Metoda zwracająca przepis na podstawie categorii, szukanego tesktu lub obu parametrów.
        /// </summary>
        /// <param name="category">Oznacza nr kategorii typu int</param>
        /// <param name="search">Oznacza dowolny szukany tekst dla tytułu</param>
        /// <returns>Zwraca kolekcje przepisów na podstawie modelu CompactRecipe i typu JSON.</returns>
        [ResponseType(typeof(IEnumerable<CompactRecipe>))]
        [AllowAnonymous]
        [Route("api/recipes")]
        public async Task<IHttpActionResult> GetRecipes(int? category = null, string search = "")
        {

            IEnumerable<Recipe> recipes;

            if (!string.IsNullOrEmpty(search))
            {
                recipes = await _recipesService.GetRecipesForTitle(search);
            }
            else if (category != null)
            {
                recipes = await _recipesService.GetRecipesByFilters(category);
            }
            else
            {
                recipes = await _recipesService.ListAsync();
            }

            var output = recipes.Select(x => new CompactRecipe(x));
            return Ok(output);
        }

        /// <summary>
        /// Metoda zwracająca liste przepisów do akceptacji.
        /// </summary>
        /// <returns>Zwaraca przepisy do akceptacji o modelu Recipe typu JSON.</returns>
        [HttpGet]
        [Route("api/recipes/getRecipesToAccept")]
        [ResponseType(typeof(Recipe))]
        public async Task<IHttpActionResult> GetRecipesToAccept()
        {
            var list = await _recipesService.GetRecipesToAccept();

            return Json(list);
        }


        /// <summary>
        /// Metoda zwracająca przepis na podstawie jego id.
        /// </summary>
        /// <param name="id">Oznacza id przepisu</param>
        /// <returns>Zwraca przepis na podtawie moedelu Recipe typu JSON</returns>
        [ResponseType(typeof(Recipe))]
        public async Task<IHttpActionResult> GetRecipe(int id)
        {
            var x = await _recipesService.GetRecipeForPreviewAsync(id);
            
            var recipe =
                new GetRecipeDto()
                {
                    Id = x.Id,
                    Products = x.Products.Select(y => new GetRecipeDtoProduct
                    {
                        Id = y.Id,
                        Name = y.Product.Name,
                        Unit = new GetRecipeDtoUnit()
                        {
                            Id = y.Unit.Id,
                            Label = y.Unit.Name
                        },
                        Amount = y.NumberOfUnit
                    }).ToList(),
                    Images = x.Images.Select(y => new UploadRecipeImagesResult()
                    {
                        Id = y.Id,
                        RelativeUrl = y.Path,
                        AbsoluteUrl = Url.Content(y.Path ?? "~")
                    }).ToList(),
                    Title = x.Name,
                    Description = x.Description,
                    Difficulty = x.Difficulty,
                    TimeToPrepare = x.TimeToPrepare,
                    Tags = x.Tags.Select(y => new GetRecipeDtoTag { Id = y.Id, Name = y.Name }).ToList(),
                    EstimatedCost = x.EstimatedCost,
                    PortionCount = x.PortionCount,
                    Category = new GetRecipeDto.CategoryDto(x.Category),
                    AverageRate = x.AverageRate,
                    CanVote = UserHelper.IsUserLoggedIn() && x.CanUserVote(UserHelper.GetCurrentUserId()),
                    IsInFavorites = !UserHelper.IsUserLoggedIn() ? null : (bool?)x.IsUserFavourite(UserHelper.GetCurrentUserId())
                };

            return Ok(recipe);
        }

        // POST: api/GetRecipesByProducts
        [HttpPost]
        [Route("api/recipes/getRecipesByProducts")]
        [ResponseType(typeof(IEnumerable<CompactRecipe>))]
        //[HttpOptions]
        public async Task<IHttpActionResult> GetRecipesByProducts(List<int> productIds)
        {
            var recipes = await _recipesService.GetRecipesByProductsAsync(productIds);
            return Ok(recipes.Select(x => new CompactRecipe(x)));
        }

        // PUT: api/Recipes/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutRecipe(int id, UpdateCommand recipe)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != recipe.Id)
            {
                return BadRequest();
            }

            var updated = await _recipesService.UpdateRecipeAsync(recipe);
            return Ok(updated.Id);
        }

        // POST: api/Recipes
        [ResponseType(typeof(int))]
        [HttpPost]
        [Route("api/recipes")]
        public async Task<IHttpActionResult> PostRecipe(CreateCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdRecipe = await _recipesService.CreateRecipeAsync(command);
            return Ok(createdRecipe.Id);
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
                RelativeUrl = x,
                AbsoluteUrl = Url.Content(x)
            }
            ));
        }

        [HttpGet]
        [Authorize]
        [Route("api/user/recipes")]
        [ResponseType(typeof(IEnumerable<CompactRecipe>))]
        public async Task<IHttpActionResult> GetMyRecipes()
        {
            var list = await _recipesService.GetMyRecipes();
            return Ok(list.Select(x => new CompactRecipe(x)));
        }

        [HttpGet]
        [Route("api/carousels")]
        [ResponseType(typeof(IEnumerable<Carousel>))]
        public async Task<IHttpActionResult> GetCarousels()
        {

            var carousels =
                new List<Carousel>
                {
                    new Carousel("Najnowsze przepisy", await _recipesService.GetNewestRecipesAsync()),
                    new Carousel("Najlepsze zupy", await _recipesService.GetRecipesByCategoryAsync(3)), // todo: dodać stałe do kategorii
                    new Carousel("Najlepsze desery", await _recipesService.GetRecipesByCategoryAsync(4)) // todo: dodać stałe do kategorii
                };

            return Ok(carousels);
        }

        [HttpPost]
        [Route("api/recipes/{id}/{rate}")]
        public async Task<IHttpActionResult> RateRecipe(int id, int rate)
        {
            try
            {
                double avgRate = await _recipesService.RateRecipeAsync(id, rate);
                return Ok(avgRate);
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }

        [HttpGet]
        [Route("api/recipes/random")]
        [ResponseType(typeof(int))]
        public async Task<IHttpActionResult> GetRandomRecipeId()
        {
            try
            {
                int recipeId = await _recipesService.GetRandomRecipeIdAsync();
                return Ok(recipeId);
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [HttpPost]
        [Route("api/recipes/import")]
        public async Task<IHttpActionResult> ImportRecipe(RecipeImport recipe)
        {
            try
            {
                await _recipesService.ImportRecipeAsync(recipe);
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }

            return Ok("Import poprawny.");
        }

        [HttpPost]
        [Route("api/recipes/favorite/add")]
        public async Task<IHttpActionResult> AddRecipeToFavorite(int id)
        {
            try
            {
                var result = await _recipesService.AddRecipeToFavorite(id);
                if (result == 1)
                    return Ok(true);
                else if(result == 2)
                    return Ok(false);
                else
                    return InternalServerError();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }

        }

        [HttpPost]
        [Route("api/recipes/favorite/remove")]
        public async Task<IHttpActionResult> RemoveRecipeFromFavorite(int id)
        {
            try
            {
                var result = await _recipesService.RemoveRecipeFromFavorite(id);
                if (result == 1)
                    return Ok(true);
                else if (result == 2)
                    return Ok(false);
                else
                    return InternalServerError();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }

        }

        [HttpGet]
        [Route("api/recipes/favorites")]
        [ResponseType(typeof(IEnumerable<CompactRecipe>))]
        public async Task<IHttpActionResult> GetFavouriteRecipes()
        {
            var list = await _recipesService.GetUserFavouriteRecipesAsync();

            // kiedy jest niezalogowany
            if (list == null)
                return null;


            return Ok(list.Select(x => new CompactRecipe(x)));
        }

    }
}