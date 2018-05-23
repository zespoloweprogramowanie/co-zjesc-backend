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
    public class CategoriesController : ApiController
    {
        private IRecipeCategoriesService _recipeCategoriesService;

        public CategoriesController()
        {
            _recipeCategoriesService = new RecipeCategoriesService(new AppDb());
        }

        public class GetRecipeCategory
        {
            public int id { get; set; }
            public string name { get; set; }
        }

        /// <summary>
        /// Metoda zwracająca wszystkie kategorie.
        /// </summary>
        /// <returns>Zwaraca id i nazwę kategorii typu JSON.</returns>
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

    }
}