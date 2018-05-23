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
    public class UnitsController : ApiController
    {
        private UnitsService _unitsService;

        public UnitsController()
        {
            _unitsService = new UnitsService(new AppDb());
        }

        /// <summary>
        /// Metoda zwracająca jednostki.
        /// </summary>
        /// <returns>Zwraca kolekcje jednostek jako id i name typu JSON</returns>
        [Route("api/units")]
        public async Task<IHttpActionResult> GetUnits()
        {
            var list = await _unitsService.ListAsync();
            return Ok(list.Select(x => new
            {
                id = x.Id,
                name = x.Name
            }));
        }

    }
}