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

    /// <summary>
    /// Serwis odpowiedzialny za obsługę logiki biznesowej dla produktów
    /// </summary>
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

        /// <summary>
        /// Dodaje produkt do bazy danych razem ze zdjęciem
        /// </summary>
        /// <param name="product">Produkt</param>
        /// <param name="image">Zdjęcie binarne</param>
        /// <returns></returns>
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

        /// <summary>
        /// Aktualizuje produkt w bazie danych razem ze zdjęciem
        /// </summary>
        /// <param name="product">Produkt</param>
        /// <param name="image">Zdjęcie binarne</param>
        /// <returns></returns>
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

        /// <summary>
        /// Aktualizuje zdjęcie produktu
        /// </summary>
        /// <param name="image">Zdjęcie</param>
        /// <param name="current">Produkt</param>
        /// <returns></returns>
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

        /// <summary>
        /// Zapytanie pobierające informacje o produktach
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Pobiera produkt na podstawie id
        /// </summary>
        /// <param name="id">id produktu</param>
        /// <returns></returns>
        public Product GetProduct(int id)
        {
            Product product = _db.Products.Find(id);
            if (product == null)
            {
                return null;
            }

            return product;
        }

        /// <summary>
        /// Aktualizuje produkt na podstawie id
        /// </summary>
        /// <param name="product"></param>
        /// <param name="image">Zdjęcie binarne</param>
        /// <returns></returns>
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

        /// <summary>
        /// Sprawdza czy produkt istnieje
        /// </summary>
        /// <param name="id">id produktu</param>
        /// <returns>Czy produkt istnieje</returns>
        private bool ProductExists(int id)
        {
            return _db.Products.Count(e => e.Id == id) > 0;
        }

        /// <summary>
        /// Dodaje produkt
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Usuwa produkt
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Pobiera jednostki
        /// </summary>
        /// <returns></returns>
        public List<Unit> GetUnits()
        {
            var units = _db.Units.ToList();
            return units;
        }

        /// <summary>
        /// Pobiera tagi
        /// </summary>
        /// <returns></returns>
        public List<RecipeTag> GetTags()
        {
            var tags = _db.RecipeTags.ToList();
            return tags;
        }

        /// <summary>
        /// Pobiera produkt o danej nazwie, a jeśli nie istnieje to go tworzy
        /// </summary>
        /// <param name="name">Nazwa produktu</param>
        /// <returns>Pobrany / utworzony produkt</returns>
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
        

        /// <summary>
        /// Metoda importująca produkt
        /// </summary>
        /// <param name="importProducts">Obiekty trzymające informacje o importowanych produktach</param>
        /// <returns>Produkty domenowe</returns>
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
