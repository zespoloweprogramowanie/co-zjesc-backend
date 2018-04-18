using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using WhatToEat.Core;
using WhatToEat.Core.Extensions;
using WhatToEat.Core.Helpers;
using WhatToEat.Domain.Commands.Recipe;
using WhatToEat.Domain.Exceptions;
using WhatToEat.Domain.Helpers;
using WhatToEat.Domain.Models;
using WhatToEat.Domain.Models.DTO;

namespace WhatToEat.Domain.Services
{
    public interface IRecipesService : IEntityService<Recipe>
    {
        List<string> UploadRecipeImages(IEnumerable<HttpPostedFile> files);

        Task<Recipe> GetRecipeForPreviewAsync(int recipeId);
        Task<IEnumerable<Recipe>> GetRecipesByProductsAsync(List<int> productIds);
        Task<Recipe> CreateRecipeAsync(CreateCommand command);
        Task<Recipe> UpdateRecipeAsync(UpdateCommand command);
        Task<IEnumerable<Recipe>> GetRecipesForTitle(string title);
        Task<IEnumerable<Recipe>> GetRecipesToAccept();
        Task<IEnumerable<Recipe>> GetRecipesByFilters(int? categoryId);
        Task<IEnumerable<Recipe>> GetMyRecipes();
        Task<IEnumerable<Recipe>> GetNewestRecipesAsync(int count = 15);
        Task<IEnumerable<Recipe>> GetRecipesByCategoryAsync(int categoryId);
        Task RateRecipeAsync(int id, int rate);
        Task<int> GetRandomRecipeIdAsync();
        Task<Recipe> ImportRecipeAsync(RecipeImport recipe);
    }

    public class RecipesService : EntityService<Recipe>, IRecipesService
    {
        private ILogger _logger;
        private new readonly IContext _db;

        private readonly IProductsService _productsService;
        private readonly ITagsService _tagsService;
        private readonly IRecipeCategoriesService _categoriesService;

        public RecipesService(IContext context) : base(context)
        {
            _db = context;
            _dbset = _db.Set<Recipe>();
            _logger = new DbLogger(new AppDb());
            _productsService = new ProductsService(new AppDb());
            _tagsService = new TagsService(new AppDb());
            _categoriesService = new RecipeCategoriesService(new AppDb());
        }

        public async Task<IEnumerable<Recipe>> GetRecipesByFilters(int? categoryId)
        {
            var list = await _dbset
                .Where(x => x.CategoryId == categoryId || categoryId == null)
                .ToListAsync();
            return list;
        }

        public List<string> UploadRecipeImages(IEnumerable<HttpPostedFile> files)
        {
            string directoryGuid = Guid.NewGuid().ToString();
            string directoryRelativePath = $@"~/Content/Images/Recipes/{directoryGuid}";
            string directoryAbsolutePath = ServerHelper.GetAbsolutePath(directoryRelativePath);

            Directory.CreateDirectory(directoryAbsolutePath);

            List<string> filePaths = new List<string>();

            foreach (var file in files)
            {
                string newFileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(file.FileName)}";
                string fileRelativePath =
                    $"{directoryRelativePath}/{newFileName}";
                string fileAbsolutePath = ServerHelper.GetAbsolutePath(fileRelativePath);


                file.SaveAs(fileAbsolutePath);

                filePaths.Add(fileRelativePath);
            }

            return filePaths;
        }

        public async Task<Recipe> GetRecipeForPreviewAsync(int recipeId)
        {
            return await _dbset
                .Include(x => x.Products)
                .Include("Products.Product")
                .Include("Products.Unit")
                .Include(x => x.Images)
                .Include(x => x.Tags)
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.Id == recipeId);
        }

        public async Task<IEnumerable<Recipe>> GetRecipesByProductsAsync(List<int> productIds)
        {
            var recipes = await _dbset
                .Include(x => x.Products)
                .Where(x => x.Products.Any(y => productIds.Any(z => z == y.ProductId)))
                .ToListAsync();
            return recipes;
        }

        public async Task<Recipe> CreateRecipeAsync(CreateCommand command)
        {
            //var recipeProducts = command.products.Select(x => new RecipeProduct()
            //{
            //    UnitId = x.unit
            //});

            List<RecipeProduct> recipeProducts = new List<RecipeProduct>();

            foreach (var product in command.Products)
            {
                var properProduct = await _productsService.GetOrCreateProductByNameAsync(product.Name);

                RecipeProduct recipeProduct = new RecipeProduct();
                recipeProduct.NumberOfUnit = product.Amount;
                recipeProduct.ProductId = properProduct.Id;
                recipeProduct.UnitId = product.Unit;

                recipeProducts.Add(recipeProduct);
            }

            //List<RecipeTag> tags = new List<RecipeTag>();

            //foreach (var tag in command.Tags)
            //{
            //    var properTag = await _tagsService.GetOrCreateTagAsync(tag);
            //    tags.Add(properTag);
            //}




            string userId = ClaimsPrincipal.Current.Claims.FirstOrDefault(c => c.Type == "id")?.Value;

            if (String.IsNullOrEmpty(userId))
                throw new ServiceException("Nieuatoryzowana próba dodania przepisu!");

            var recipe = new Recipe()
            {
                Name = command.Title,
                Description = command.Description,
                Difficulty = command.Difficulty,
                TimeToPrepare = command.TimeToPrepare,
                EstimatedCost = command.EstimatedCost,
                PortionCount = command.PortionCount,
                Images = command.Images.Select(x => new RecipeImage
                {
                    Path = x
                }).ToList(),
                Products = recipeProducts,
                CreatedDate = DateTime.Now,
                AuthorId = userId,
                CategoryId = command.Category,
                Tags = command.Tags.Select(x => new RecipeTag
                {
                    Name = x
                }).ToList()
            };




            return await CreateAsync(recipe);
        }

        public async Task<Recipe> UpdateRecipeAsync(UpdateCommand command)
        {
            var current = await _dbset.FindAsync(command.Id);



            List<RecipeProduct> recipeProducts = new List<RecipeProduct>();

            foreach (var product in command.Products)
            {
                var properProduct = await _productsService.GetOrCreateProductByNameAsync(product.Name);

                RecipeProduct recipeProduct = new RecipeProduct();
                recipeProduct.NumberOfUnit = product.Amount;
                recipeProduct.ProductId = properProduct.Id;
                recipeProduct.UnitId = product.Unit;

                recipeProducts.Add(recipeProduct);
            }


            current.Name = command.Title;
            current.Description = command.Description;
            current.Difficulty = command.Difficulty;
            current.TimeToPrepare = command.TimeToPrepare;
            current.EstimatedCost = command.EstimatedCost;
            current.PortionCount = command.PortionCount;
            current.Images = command.Images.Select(x => new RecipeImage()
            {
                Path = x
            }).ToList();
            current.Products = recipeProducts;
            current.CategoryId = command.Category;
            current.Tags = command.Tags.Select(x => new RecipeTag
            {
                Name = x
            }).ToList();


            var updatedRecipe = await UpdateAsync(current);
            return updatedRecipe;
        }

        public async Task<IEnumerable<Recipe>> GetRecipesForTitle(string title)
        {
            var list = await _dbset
                .Include(x => x.Products)
                .Include("Products.Product")
                .Include("Products.Unit")
                .Include(x => x.Images)
                .Include(x => x.Tags)
                .Where(x => x.Name.Contains(title)).ToListAsync();

            return list;
        }

        public async Task<IEnumerable<Recipe>> GetRecipesToAccept()
        {
            var list = await _db.Recipes.Join(_db.RecipeComment, r => r.Id, c => c.RecipeId, (r, c) => new { Recipe = r, Comment = c }).Where(x => x.Comment.Accepted == false).Select(x => x.Recipe).ToListAsync();

            return list;
        }

        public async Task<IEnumerable<Recipe>> GetMyRecipes()
        {
            string userId = UserHelper.GetCurrentUserId();//ClaimsPrincipal.Current.Claims.FirstOrDefault(c => c.Type == "id")?.Value;

            var list = await _dbset
                .Include(x => x.Products)
                .Include("Products.Product")
                .Include("Products.Unit")
                .Include(x => x.Images)
                .Include(x => x.Tags)
                .Where(x => x.AuthorId == userId).ToListAsync();

            return list;
        }

        public async Task<IEnumerable<Recipe>> GetNewestRecipesAsync(int count = 15)
        {
            var query = _dbset
                .OrderByDescending(x => x.CreatedDate)
                .Take(count);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Recipe>> GetRecipesByCategoryAsync(int categoryId)
        {
            var query = from r in _dbset
                        where r.CategoryId == categoryId
                        select r;

            return await query.ToListAsync();
        }

        public async Task RateRecipeAsync(int id, int rate)
        {
            var recipe = await _dbset
                .Include(x => x.Rates)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (recipe == null)
                throw new ServiceException("Brak podanego przepisu!");

            string userId = UserHelper.GetCurrentUserId();

            if (String.IsNullOrEmpty(userId))
                throw new ServiceException("Niezalogowani użytkownicy nie mogą oceniać przepisów!");

            if (recipe.Rates.Count(x => x.RecipeId == id && x.UserId == userId) > 0)
                throw new ServiceException("Podany użytkownik ocenił już ten przepis!");

            recipe.Rates.Add(new RecipeRate()
            {
                UserId = userId,
                RecipeId = id,
                Rate = rate
            });

            recipe.AverageRate = (recipe.AverageRate + rate) / recipe.Rates.Count; //CalculateAverageRate(recipe);

            await _db.SaveChangesAsync();
        }

        public async Task<int> GetRandomRecipeIdAsync()
        {
            var ids = await _dbset.Select(x => x.Id).ToListAsync();

            if (ids.Count == 0)
                throw new ServiceException("Brak przepisów do losowania!");

            Random r = new Random();
            int randomId = ids[r.Next(ids.Count)];
            return randomId;
        }

        public async Task<Recipe> ImportRecipeAsync(RecipeImport command)
        {
            string userId = UserHelper.GetCurrentUserId();

            if (String.IsNullOrEmpty(userId))
                throw new ServiceException("Nieuatoryzowana próba dodania przepisu!");

            var category = await _categoriesService.ImportCategoryAsync(command.Category);

            var recipe = new Recipe()
            {
                Name = command.Title,
                Description = command.Description,
                Difficulty = command.Difficulty,
                TimeToPrepare = command.TimeToPrepare,
                EstimatedCost = command.EstimatedCost,
                PortionCount = command.PortionCount,
                Images = command.Images.Select(x => new RecipeImage()
                {
                    Path = x
                }).ToList(),
                Products = await _productsService.ImportProductsAsync(command.Products),
                CreatedDate = DateTime.Now,
                AuthorId = userId,
                CategoryId = category.Id,
                Tags = command.Tags.Select(x => new RecipeTag()
                {
                    Name = x
                }).ToList(),  //await _tagsService.ImportTagsAsync(command.Tags),
                AverageRate = command.AverageRate
            };

            return await CreateAsync(recipe);
        }

        private static double CalculateAverageRate(Recipe recipe)
        {
            double avgRate = recipe.Rates.Aggregate<RecipeRate, double>(0, (current, rate) => current + rate.Rate);
            avgRate /= recipe.Rates.Count;
            return avgRate;
        }
    }
}
