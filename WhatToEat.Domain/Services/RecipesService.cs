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
using WhatToEat.Domain.Models;

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
    }

    public class RecipesService : EntityService<Recipe>, IRecipesService
    {
        private ILogger _logger;
        private new readonly IContext _db;

        private readonly IProductsService _productsService;

        public RecipesService(IContext context) : base(context)
        {
            _db = context;
            _dbset = _db.Set<Recipe>();
            _logger = new DbLogger(new AppDb());
            _productsService = new ProductsService(new AppDb());
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

            foreach (var product in command.products)
            {
                var properProduct = await _productsService.GetOrCreateProductByNameAsync(product.name);

                RecipeProduct recipeProduct = new RecipeProduct();
                recipeProduct.NumberOfUnit = product.amount;
                recipeProduct.ProductId = properProduct.Id;
                recipeProduct.UnitId = product.unit;

                recipeProducts.Add(recipeProduct);
            }


            var recipe = new Recipe()
            {
                Name = command.title,
                Description = command.description,
                Difficulty = command.difficulty,
                TimeToPrepare = command.timeToPrepare,
                EstimatedCost = command.estimatedCost,
                PortionCount = command.portionCount,
                Images = command.images.Select(x => new RecipeImage()
                {
                    Path = x
                }).ToList(),
                Products = recipeProducts
            };




            return await CreateAsync(recipe);
        }

        public async Task<Recipe> UpdateRecipeAsync(UpdateCommand command)
        {
            var current = await _dbset.FindAsync(command.id);



            List<RecipeProduct> recipeProducts = new List<RecipeProduct>();

            foreach (var product in command.products)
            {
                var properProduct = await _productsService.GetOrCreateProductByNameAsync(product.name);
                if (properProduct.Id <= 0)
                {
                    RecipeProduct recipeProduct = new RecipeProduct();
                    recipeProduct.NumberOfUnit = product.amount;
                    recipeProduct.ProductId = properProduct.Id;
                    recipeProduct.UnitId = product.unit;

                    recipeProducts.Add(recipeProduct);

                }
            }


            current.Name = command.title;
            current.Description = command.description;
            current.Difficulty = command.difficulty;
            current.TimeToPrepare = command.timeToPrepare;
            current.EstimatedCost = command.estimatedCost;
            current.PortionCount = command.portionCount;
            current.Images = command.images.Select(x => new RecipeImage()
            {
                Path = x
            }).ToList();
            current.Products = recipeProducts;



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
            string userId = ClaimsPrincipal.Current.Claims.FirstOrDefault(c => c.Type == "id")?.Value;

            var list = await _dbset
                .Include(x => x.Products)
                .Include("Products.Product")
                .Include("Products.Unit")
                .Include(x => x.Images)
                .Include(x => x.Tags)
                .Where(x => x.AuthorId == userId).ToListAsync();

            return list;
        }
    }
}
