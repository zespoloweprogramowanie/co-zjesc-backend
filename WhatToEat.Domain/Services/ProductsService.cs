using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using WhatToEat.Core;
using WhatToEat.Core.Helpers;
using WhatToEat.Domain.Exceptions;
using WhatToEat.Domain.Models;

namespace WhatToEat.Domain.Services
{
    public interface IProductsService : IEntityService<Product>
    {
        Task<Product> CreateProductAsync(Product product, HttpPostedFileBase image);
        Task<Product> EditProductAsync(Product product, HttpPostedFileBase image);
    }

    public class ProductsService : EntityService<Product>, IProductsService
    {
        private ILogger _logger;
        private new readonly IContext _db;

        public ProductsService(IContext context) : base(context)
        {
            _db = context;
            _dbset = _db.Set<Product>();
            _logger = new DbLogger(new AppDb());
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
    }
}
