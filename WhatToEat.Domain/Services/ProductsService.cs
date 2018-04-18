using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.ModelBinding;
using WhatToEat.Core;
using WhatToEat.Core.Helpers;
using WhatToEat.Domain.Exceptions;
using WhatToEat.Domain.Models;
using WhatToEat.Domain.Models.DTO;

namespace WhatToEat.Domain.Services
{
    public interface IProductsService : IEntityService<Product>
    {
        Task<Product> CreateProductAsync(Product product, HttpPostedFileBase image);
        Task<Product> EditProductAsync(Product product, HttpPostedFileBase image);
        IQueryable<ProductDTO> GetProducts();
        Product GetProduct(int id);
        int PutProduct(int id, Product product);
        Product PostProduct(Product product);
        int DeleteProduct(int id);
        List<Unit> GetUnits();
        List<RecipeTag> GetTags();

        Task<Product> GetOrCreateProductByNameAsync(string name);
        Task<ICollection<RecipeProduct>> ImportProductsAsync(List<RecipeImport.Product> commandProducts);
    }

    public class ProductsService : EntityService<Product>, IProductsService
    {
        private ILogger _logger;
        private new readonly IContext _db;
        private IUnitsService _unitsService;

        public ProductsService(IContext context) : base(context)
        {
            _db = context;
            _dbset = _db.Set<Product>();
            _logger = new DbLogger(new AppDb());
            _unitsService = new UnitsService(new AppDb());
        }

        public async Task<Product> CreateProductAsync(Product product, HttpPostedFileBase image)
        {
            var createdProduct = await CreateAsync(product);
            if (createdProduct == null)
                throw new ServiceException("Nie udało się utworzyć produktu!");

            _logger.Info($@"Dodano produkt o id: {createdProduct.Id}");

            if (image == null)
                return createdProduct;

            string relativeImagePath = $"~/Content/Images/Products/{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
            image.SaveAs(ServerHelper.GetAbsolutePath(relativeImagePath));
            createdProduct.Image = relativeImagePath;
            createdProduct = await UpdateAsync(createdProduct);


            return createdProduct;
        }

        public async Task<Product> EditProductAsync(Product product, HttpPostedFileBase image)
        {
            var current = await _dbset.FirstOrDefaultAsync(x => x.Id == product.Id);
            if (current == null)
                throw new NullReferenceException();

            if (image != null)
                current = OverrideCurrentProductImage(image, current);

            current.Name = product.Name;
            var updatedProduct = await UpdateAsync(current);
            return updatedProduct;
        }

        private Product OverrideCurrentProductImage(HttpPostedFileBase image, Product current)
        {
            var relativeImagePath =
                $"~/Content/Images/Products/{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
            image.SaveAs(ServerHelper.GetAbsolutePath(relativeImagePath));
            if (!string.IsNullOrEmpty(current.Image))
                File.Delete(ServerHelper.GetAbsolutePath(current.Image));
            current.Image = relativeImagePath;
            return current;
        }

        public IQueryable<ProductDTO> GetProducts()
        {
            var products = from p in _db.Products
                           select new ProductDTO()
                           {
                               Id = p.Id,
                               Name = p.Name,
                               Image = p.Image
                           };
            return products;
        }

        public Product GetProduct(int id)
        {
            Product product = _db.Products.Find(id);
            if (product == null)
            {
                return null;
            }

            return product;
        }

        public int PutProduct(int id, Product product)
        {
            var result = 0;
            
            _db.Entry(product).State = EntityState.Modified;

            try
            {
                _db.SaveChanges();
                result = 1;
            }
            catch (Exception)
            {
                if (!ProductExists(id))
                {
                    return result;
                }
                else
                {
                    throw;
                }
            }

            return result;
        }

        private bool ProductExists(int id)
        {
            return _db.Products.Count(e => e.Id == id) > 0;
        }

        public Product PostProduct(Product product)
        {
            try
            {
                _db.Products.Add(product);
                _db.SaveChanges();
            }
            catch (Exception)
            {

            }

            return product;
        }

        public int DeleteProduct(int id)
        {
            var result = 0;

            Product product = _db.Products.Find(id);
            if (product == null)
            {
                return result;
            }

            _db.Products.Remove(product);
            _db.SaveChanges();
            result = 1;
         
            return result;
        }

        public List<Unit> GetUnits()
        {
            var units = _db.Units.ToList();
            return units;
        }

        public List<RecipeTag> GetTags()
        {
            var tags = _db.RecipeTags.ToList();
            return tags;
        }

        public async Task<Product> GetOrCreateProductByNameAsync(string name)
        {
            var product = await _dbset
                .FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower());

            if (product != null)
                return product;

            product = await CreateAsync(new Product()
            {
                Name = name
            });

            return product;
        }
        


        public async Task<ICollection<RecipeProduct>> ImportProductsAsync(List<RecipeImport.Product> importProducts)
        {
            List<RecipeProduct> importedProducts = new List<RecipeProduct>();

            foreach (var importProduct in importProducts)
            {
                var product = await GetOrCreateProductByNameAsync(importProduct.Name);
                var unit = await _unitsService.GetOrCreateUnitByNameAsync(importProduct.Unit);

                RecipeProduct recipeProduct = new RecipeProduct();
                recipeProduct.NumberOfUnit = importProduct.Amount;
                recipeProduct.ProductId = product.Id;
                recipeProduct.UnitId = unit.Id;
                importedProducts.Add(recipeProduct);
            }

            return importedProducts;
        }
    }
}
