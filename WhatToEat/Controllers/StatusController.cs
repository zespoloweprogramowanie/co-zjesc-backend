using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace WhatToEat.Controllers
{
    public class StatusController : ApiController
    {
        [HttpGet]
        public String Get()
        {
            return DateTime.Now.ToLongDateString();
        }
    }
}