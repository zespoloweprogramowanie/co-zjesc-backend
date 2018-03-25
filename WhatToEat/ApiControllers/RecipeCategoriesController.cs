using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WhatToEat.Domain.Models;
using WhatToEat.Domain.Services;

namespace WhatToEat.ApiControllers
{
    [RoutePrefix("api/categories")]
    public class RecipeCategoriesController : ApiController
    {
        private IRecipeCategoriesService _recipeCategoriesService;

        public RecipeCategoriesController()
        {
            _recipeCategoriesService = new RecipeCategoriesService(new AppDb());
        }

        public class GetRecipeCategory
        {
            public int id { get; set; }
            public string name { get; set; }
        }

        [Route("")]
        // GET api/<controller>
        public async Task<IEnumerable<GetRecipeCategory>> Get()
        {
            var categories = await _recipeCategoriesService.ListAsync();
            return categories.Select(x => new GetRecipeCategory()
            {
                id = x.Id,
                name = x.Name
            });
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}