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

        /// <summary>
        /// Metoda zwracająca przepis na podstawie listy id składników.
        /// </summary>
        /// <param name="productIds">Oznacza id produktów</param>
        /// <returns>Zwraca kolekcje przepisów jako model CompactRecipe typu JSON.</returns>
        [HttpPost]
        [Route("api/recipes/getRecipesByProducts")]
        [ResponseType(typeof(IEnumerable<CompactRecipe>))]
        //[HttpOptions]
        public async Task<IHttpActionResult> GetRecipesByProducts(List<int> productIds)
        {
            var recipes = await _recipesService.GetRecipesByProductsAsync(productIds);
            return Ok(recipes.Select(x => new CompactRecipe(x)));
        }

        /// <summary>
        /// Metoda aktualizująca przepis.
        /// </summary>
        /// <param name="id">Oznacza id przepisu</param>
        /// <param name="recipe">Model przepisu zaktualizowanego</param>
        /// <returns>Zwraca id przepisu gdy sukces, status 400 jeśli model jest zły lub status 400 gdy id przepisu jest złe.</returns>
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

        /// <summary>
        /// Metoda zapisująca nowy przepis do bazy.
        /// </summary>
        /// <param name="command">Oznacza model nowo tworzonego przepisu o modelu CreateCommand</param>
        /// <returns>Zwraca id przepisy gdy sukces lub status 400 jeśli model jest zły.</returns>
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

        /// <summary>
        /// Metoda usuwa przepis na podstawie jego id.
        /// </summary>
        /// <param name="id">Oznacza id przepisu</param>
        /// <returns>Zwraca status 200.</returns>
        public async Task<IHttpActionResult> DeleteRecipe(int id)
        {
            await _recipesService.DeleteAsync(id);
            return Ok();
        }

        /// <summary>
        /// Metoda dodająca zdjęcie dla przepisu do bazy.
        /// </summary>
        /// <returns>Zwraca ścieżki i id nowo dodanych zdjęć jako model UploadRecipeImagesResult typu JSON.</returns>
        [HttpPost]
        [Route("api/recipes/uploadRecipeImages")]
        [ResponseType(typeof(IEnumerable<string>))]
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


        /// <summary>
        /// Metoda zwracająca przepisy aktualnie zalogowanego użytkownika.
        /// </summary>
        /// <returns>Zwraca kolekcje przepisów jako model CompactRecipe o typie JSON.</returns>
        [HttpGet]
        [Authorize]
        [Route("api/user/recipes")]
        [ResponseType(typeof(IEnumerable<CompactRecipe>))]
        public async Task<IHttpActionResult> GetMyRecipes()
        {
            var list = await _recipesService.GetMyRecipes();
            return Ok(list.Select(x => new CompactRecipe(x)));
        }

        /// <summary>
        /// Metoda zwracająca karuzele przepisów.
        /// </summary>
        /// <returns>Zwraca karuzele przepisów o modelu Carousel jako lista typu JSON.</returns>
        [HttpGet]
        [Route("api/carousels")]
        [ResponseType(typeof(IEnumerable<Carousel>))]
        public async Task<IHttpActionResult> GetCarousels()
        {

            var carousels =
                new List<Carousel>
                {
                    new Carousel("Najnowsze przepisy", await _recipesService.GetNewestRecipesAsync()),
                    new Carousel("Najpopularniejsze", await _recipesService.GetMostPopularRecipes()),
                    new Carousel("Najwyżej oceniane", await _recipesService.GetHighestRateRecipes()),
                    new Carousel("Najlepsze zupy", await _recipesService.GetRecipesByCategoryAsync(3)), // todo: dodać stałe do kategorii
                    new Carousel("Najlepsze desery", await _recipesService.GetRecipesByCategoryAsync(4)) // todo: dodać stałe do kategorii
                };

            if (UserHelper.IsUserLoggedIn())
            {
                var favourites = await _recipesService.GetUserFavouriteRecipesAsync();
                var enumerable = favourites.ToList();
                if (enumerable.Any())
                    carousels.Insert(0, new Carousel("Moje ulubione", enumerable));
            }

            return Ok(carousels);
        }

        /// <summary>
        /// Metoda dodająca ocenę dla przepisu po jego id.
        /// </summary>
        /// <param name="id">Oznacza id przepisu</param>
        /// <param name="rate">Oznacza ocenę dla przepisu od 1 do 5</param>
        /// <returns>Zwraca średnią ocenę dla przepisu lub status 500 w przypadku wyjątku.</returns>
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

        /// <summary>
        /// Metoda zwracająca losowe id przepisu.
        /// </summary>
        /// <returns>Zwraca id przepisu.</returns>
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

        /// <summary>
        /// Metoda importuje przepis do bazy.
        /// </summary>
        /// <param name="recipe">Oznacza model przepisu RecipeImport</param>
        /// <returns>Zwraca komunikat "Import poprawny." lub status 500 w przypadku wyjątku.</returns>
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

        /// <summary>
        /// Metoda dodająca przepis do ulubionych dla zalogowanego użytkownika.
        /// </summary>
        /// <param name="id">Oznacza id przepisu</param>
        /// <returns>Zwraca true jeśli doda przepis do ulubionych, false jeśli przepis jest już w ulubionych lub status 500 w przypadku wyjątku.</returns>
        [HttpPost]
        [Route("api/recipes/favorite/add")]
        public async Task<IHttpActionResult> AddRecipeToFavorite(int id)
        {
            try
            {
                var result = await _recipesService.AddRecipeToFavorite(id);
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

        /// <summary>
        /// Metoda usuwająca przepis z ulubionych.
        /// </summary>
        /// <param name="id">Oznacza id przepisu</param>
        /// <returns>Zwraca true jeśli usunie przepis z ulubionych, false jeśli przepis jest już usunięty lub status 500 w przypadku wyjątku.</returns>
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

        /// <summary>
        /// Metoda zwracająca ulubione przepisy zalogowanego użytkownika.
        /// </summary>
        /// <returns>Zwraca kolekcje ulubionych przepisów jako model CompactRecipe typu JSON.</returns>
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