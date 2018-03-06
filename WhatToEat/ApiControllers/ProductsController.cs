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
        private AppDb db = new AppDb();

        public ProductsController()
        {
            _productsService = new ProductsService(new AppDb());
        }

        // GET: api/Products
        public IQueryable<ProductDTO> GetProducts()
        {
            return _productsService.GetProducts();                           
        }

        // GET: api/Products/5
        //[ResponseType(typeof(Product))]
        public IHttpActionResult GetProduct(int id)
        {
            return Ok(_productsService.GetProduct(id));
        }

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

        [Route("api/units")]
        public IHttpActionResult GetUnits()
        { 
            return Ok(_productsService.GetUnits());
        }

        [Route("api/tags")]
        public IHttpActionResult GetTags()
        {
            return Ok(_productsService.GetTags());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        
    }
}