using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using WhatToEat.Domain.Models;
using WhatToEat.Domain.Services;

namespace WhatToEat.ApiControllers
{
    public class ProductsController : ApiController
    {
        private IProductsService _productsService;
        //private AppDb db = new AppDb();

        public ProductsController()
        {
            _productsService = new ProductsService(new AppDb());
        }

        /// <summary>
        /// Metoda zwraca liste wszystich produktów.
        /// </summary>
        /// <returns>Zwraca model ProductDTO typu JSON.</returns>
        // GET: api/Products
        public IQueryable<ProductDTO> GetProducts()
        {
            return _productsService.GetProducts();                           
        }


        /// <summary>
        /// Metoda zwraca produkt na podstawie id produktu.
        /// </summary>
        /// <param name="id">Oznacza id produktu</param>
        /// <returns>Zwraca model Product typu JSON.</returns>
        // GET: api/Products/5
        //[ResponseType(typeof(Product))]
        public IHttpActionResult GetProduct(int id)
        {
            return Ok(_productsService.GetProduct(id));
        }

        /// <summary>
        /// Metoda aktualizująca produkt na podstawie jego id.
        /// </summary>
        /// <param name="id">Oznacza id produktu</param>
        /// <param name="product">Zawiera model Product</param>
        /// <returns>Zwraca status 400 gdy id jest różne niż id w modelu Product, status 204 jeśli powodzenie lub status 404 w przeciwnych wypadkach.</returns>
        // PUT: api/Products/5
        //[ResponseType(typeof(void))]
        public IHttpActionResult PutProduct(int id, Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != product.Id)
            {
                return BadRequest();
            }

            var result = _productsService.PutProduct(id, product);

            if(result == 1)
            {
                return StatusCode(HttpStatusCode.NoContent);
            }

            return NotFound();
        }

        /// <summary>
        /// Metoda dodająca produkt do bazy.
        /// </summary>
        /// <param name="product">Zawiera model Product</param>
        /// <returns>Zwraca status 400 gdy model jest błędny lub id produktu i model Product.</returns>
        // POST: api/Products
        [ResponseType(typeof(Product))]
        public IHttpActionResult PostProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Product prod = _productsService.PostProduct(product);

            return CreatedAtRoute("DefaultApi", new { id = prod.Id }, prod);
        }

        /// <summary>
        /// Metoda usuwa produkt z bazy na podstawie jego id.
        /// </summary>
        /// <param name="id">Oznacza id produktu</param>
        /// <returns>Zwraca 204 jeśli powodzenie lub 404 jeśli niepowodzenie.</returns>
        // DELETE: api/Products/5
        [ResponseType(typeof(Product))]
        public IHttpActionResult DeleteProduct(int id)
        {
            int result = _productsService.DeleteProduct(id);
            if(result == 1)
            {
                return StatusCode(HttpStatusCode.NoContent);
            }

            return NotFound();
        }

        /// <summary>
        /// Metoda zwraca tagi.
        /// </summary>
        /// <returns>Zwraca liste modelu RecipeTag</returns>
        [Route("api/tags")]
        public IHttpActionResult GetTags()
        {
            return Ok(_productsService.GetTags());
        }
        
    }
}