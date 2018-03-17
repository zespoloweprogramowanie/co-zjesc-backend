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
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
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