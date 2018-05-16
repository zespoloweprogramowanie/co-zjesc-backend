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
using Microsoft.Ajax.Utilities;
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
        Task<double> RateRecipeAsync(int id, int rate);
        Task<int> GetRandomRecipeIdAsync();
        Task<Recipe> ImportRecipeAsync(RecipeImport recipe);
        Task<int> AddRecipeToFavorite(int id);
        Task<int> RemoveRecipeFromFavorite(int id);
        Task<IEnumerable<Recipe>> GetUserFavouriteRecipesAsync();
    }

    public class RecipesService : EntityService<Recipe>, IRecipesService
    {
        private ILogger _logger;
        private new readonly IContext _db;

        private readonly IProductsService _productsService;
        private readonly ITagsService _tagsService;
        private readonly IRecipeCategoriesService _categoriesService;
        private readonly IUnitsService _unitsService;

        public RecipesService(IContext context) : base(context)
        {
            _db = context;
            _dbset = _db.Set<Recipe>();
            _logger = new DbLogger(new AppDb());
            _productsService = new ProductsService(new AppDb());
            _tagsService = new TagsService(new AppDb());
            _categoriesService = new RecipeCategoriesService(new AppDb());
            _unitsService = new UnitsService(new AppDb());
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
                .Include(x => x.FavouriteRecipes)
                .FirstOrDefaultAsync(x => x.Id == recipeId);
        }

        public async Task<IEnumerable<Recipe>> GetRecipesByProductsAsync(List<int> productIds)
        {
            var recipes = await _dbset
                .Include(x => x.Products)
                .Where(x => x.Products.Any(y => productIds.Any(z => z == y.ProductId)))
                .ToListAsync();

            //if (recipes.Count == 0) // jeśli żaden z przepisów nie zawiera składnika i lista przepisów jest pusta
            //{
            //    List<int> categoryIds = new List<int>();

            //    foreach (var cat in _db.RecipeCategories.AsNoTracking().ToList())
            //    {
            //        categoryIds.Add(cat.Id);
            //    }             

            //    foreach (var item in _dbset.Where(x => categoryIds.Any(z => z == x.CategoryId)).GroupBy(x => x.CategoryId, x => x, (key, rec) => new { CatId = key, Recipe = rec.OrderByDescending(z => z.AverageRate) }))
            //    {
            //        foreach (var recip in item.Recipe.Take(2))
            //        {
            //            recipes.Add(recip); //wrzucamy po 2 przepisy z każdej kategorii z najwyższą średnią oceną
            //        }
            //    }                                  
                    
            //}

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
            var current = await _dbset
                .Include(x => x.Images)
                .Include(x => x.Products)
                .Include("Products.Product")
                .Include(x => x.Tags)
                .FirstOrDefaultAsync(x => x.Id == command.Id);

            if (current == null)
                throw new NullReferenceException("Przepis nie istnieje!");

            // produkty
            foreach (var updatedProduct in command.Products)
            {
                var dbRecipeProduct = await _db.RecipeProducts.FirstOrDefaultAsync(x => x.Product.Name == updatedProduct.Name);
                if (dbRecipeProduct != null)
                {
                    int productId = dbRecipeProduct.ProductId;
                    var dbProduct = await _db.Products.FindAsync(dbRecipeProduct.ProductId);
                    if (dbProduct.Name != updatedProduct.Name)
                    {
                        var properProduct = await _productsService.GetOrCreateProductByNameAsync(updatedProduct.Name);
                        productId = properProduct.Id;
                    }

                    _db.Entry(dbRecipeProduct).CurrentValues.SetValues(new RecipeProduct
                    {
                        Id = dbRecipeProduct.Id,
                        NumberOfUnit = updatedProduct.Amount,
                        ProductId = productId,
                        RecipeId = dbRecipeProduct.RecipeId,
                        UnitId = updatedProduct.Unit
                    });
                }
                else
                {
                    var properProduct = await _productsService.GetOrCreateProductByNameAsync(updatedProduct.Name);

                    RecipeProduct recipeProduct = new RecipeProduct
                    {
                        NumberOfUnit = updatedProduct.Amount,
                        ProductId = properProduct.Id,
                        UnitId = updatedProduct.Unit,
                        Product = new Models.Product()
                        {
                            Name = updatedProduct.Name
                        }
                    };
                    current.Products.Add(recipeProduct);
                }
            }

            foreach (var product in current.Products.ToList())
            {
                var detachedProduct = command.Products.FirstOrDefault(x => x.Name == product.Product.Name);
                if (detachedProduct == null && product.Id > 0)
                {
                    current.Products.Remove(product);
                    _db.RecipeProducts.Remove(product);
                }
            }


            // obrazki
            foreach (var updatedImage in command.Images)
            {
                var dbRecipeImage = await _db.RecipeImages.FindAsync(updatedImage.Id);
                if (dbRecipeImage != null)
                    continue;

                current.Images.Add(new RecipeImage
                {
                    Path = updatedImage.RelativeUrl,
                    RecipeId = command.Id
                });
            }


            foreach (var image in current.Images.ToList())
            {
                var detachedImage = command.Images.Find(x => x.Id == image.Id);
                if (detachedImage == null && image.Id > 0)
                {
                    current.Images.Remove(image);
                    _db.RecipeImages.Remove(image);
                }
            }

            //tagi
            // todo: tagi jak produkty

            var tagsToRemove = new List<RecipeTag>();

            foreach (var tag in current.Tags)
            {
                if (command.Tags.All(x => x.ToLower() != tag.Name.ToLower()))
                    tagsToRemove.Add(tag);
            }

            foreach (var tag in tagsToRemove)
                current.Tags.Remove(tag);


            foreach (var tag in command.Tags)
            {
                bool exists = _db.RecipeTags.Any(x => x.Name.ToLower() == tag.ToLower());
                if (!exists || current.Tags.All(x => x.Name.ToLower() != tag.ToLower()))
                {
                    //var properTag = await _tagsService.GetOrCreateTagAsync(tag);

                    var properTag = await _db.RecipeTags
                        .FirstOrDefaultAsync(x => x.Name.ToLower() == tag.ToLower());
                    if (properTag == null)
                    {

                        properTag = await _tagsService.CreateAsync(new RecipeTag()
                        {
                            Name = tag
                        });
                    }
                    current.Tags.Add(properTag);
                }
            }


            //current.Tags = command.Tags.Select(x => new RecipeTag
            //{
            //    Name = x
            //}).ToList();


            current.Name = command.Title;
            current.Description = command.Description;
            current.Difficulty = command.Difficulty;
            current.TimeToPrepare = command.TimeToPrepare;
            current.EstimatedCost = command.EstimatedCost;
            current.PortionCount = command.PortionCount;
            current.CategoryId = command.Category;
            //current.Images = command.Images.Select(x => new RecipeImage()
            //{
            //    Path = x.Path
            //}).ToList();




            var updatedRecipe = await UpdateAsync(current);
            return updatedRecipe;


            //List<RecipeProduct> recipeProducts = await _db.RecipeProducts.Where(x => x.RecipeId == command.Id).ToListAsync();

            //foreach (var product in command.Products)
            //{
            //    var properProduct = await _productsService.GetOrCreateProductByNameAsync(product.Name);

            //    RecipeProduct recipeProduct = new RecipeProduct();
            //    recipeProduct.NumberOfUnit = product.Amount;
            //    recipeProduct.ProductId = properProduct.Id;
            //    recipeProduct.UnitId = product.Unit;

            //    //recipeProducts.Add(recipeProduct);
            //}

            //current.Products = new List<RecipeProduct>();



            //foreach (var product in command.Products)
            //{
            //    if (product.Id != null)
            //    {
            //        var productInDb = await _db.RecipeProducts.FirstOrDefaultAsync(x => x.Id == product.Id);

            //        if (productInDb == null)
            //            continue;

            //        current.Products.Add(new RecipeProduct
            //        {
            //            Id = (int)product.Id,
            //            NumberOfUnit = product.Amount,
            //            ProductId = productInDb.ProductId,
            //            RecipeId = productInDb.RecipeId,
            //            UnitId = product.Unit
            //        });
            //    }
            //    else
            //    {
            //        var properProduct = await _productsService.GetOrCreateProductByNameAsync(product.Name);

            //        RecipeProduct recipeProduct = new RecipeProduct
            //        {
            //            NumberOfUnit = product.Amount,
            //            ProductId = properProduct.Id,
            //            UnitId = product.Unit
            //        };

            //        current.Products.Add(recipeProduct);
            //    }
            //}





            //return null;
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

        public async Task<double> RateRecipeAsync(int id, int rate)
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

            recipe.AverageRate = CalculateAverageRate(recipe);//(recipe.AverageRate + rate) / recipe.Rates.Count; 

            await _db.SaveChangesAsync();
            return recipe.AverageRate;
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

            var existing = await _dbset.FirstOrDefaultAsync(x => x.Name.ToLower() == command.Title.ToLower());
            var category = await _categoriesService.ImportCategoryAsync(command.Category);

            if (existing != null)
            {
                return await UpdateRecipeAsync(new UpdateCommand()
                {
                    Id = existing.Id,
                    Category = category.Id,
                    Description = command.Description,
                    Difficulty = command.Difficulty,
                    EstimatedCost = command.EstimatedCost,
                    PortionCount = command.PortionCount,
                    Title = command.Title,
                    TimeToPrepare = command.TimeToPrepare,
                    Products = command.Products.Select(x => new UpdateCommand.Product()
                    {
                        Name = x.Name,
                        Unit = _unitsService.GetOrCreateUnitByName(x.Unit).Id,
                        Amount = x.Amount
                    }).ToList(),
                    Images = command.Images.Select(x => new UpdateCommand.Image()
                    {
                        Id = null,
                        RelativeUrl = x
                    }).ToList(),
                    Tags = command.Tags
                });
            }


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

        public async Task<int> AddRecipeToFavorite(int id)
        {
            string userId = UserHelper.GetCurrentUserId();
            if (userId != null)
            {

                var favoriteRecipeUser = await _db.UserFavouriteRecipe.SingleOrDefaultAsync(x => x.UserId == userId && x.RecipeId == id);
                if (favoriteRecipeUser == null)
                {

                    UserFavouriteRecipe favoriteRecipe = new UserFavouriteRecipe();
                    favoriteRecipe.UserId = userId;
                    favoriteRecipe.RecipeId = id;

                    _db.UserFavouriteRecipe.Add(favoriteRecipe);
                    await _db.SaveChangesAsync();

                    return 1;
                }

                return 2;
            }

            return 3;
        }

        public async Task<int> RemoveRecipeFromFavorite(int id)
        {
            string userId = UserHelper.GetCurrentUserId();
            if (userId != null)
            {
                var favoriteRecipeUser = await _db.UserFavouriteRecipe.SingleOrDefaultAsync(x => x.UserId == userId && x.RecipeId == id);
                if (favoriteRecipeUser != null)
                {
                    _db.UserFavouriteRecipe.Remove(favoriteRecipeUser);
                    await _db.SaveChangesAsync();

                    return 1;
                }

                return 2;
            }

            return 3;
        }

        public async Task<IEnumerable<Recipe>> GetUserFavouriteRecipesAsync()
        {
            string userId = UserHelper.GetCurrentUserId();

            if (String.IsNullOrEmpty(userId))
                return null;

            var query = _dbset
                .Include(x => x.Products)
                .Include(x => x.Images)
                .Include(x => x.Tags)
                .Where(x => x.FavouriteRecipes.Any(y => y.UserId == userId));

            return await query.ToListAsync();
        }
    }
}
