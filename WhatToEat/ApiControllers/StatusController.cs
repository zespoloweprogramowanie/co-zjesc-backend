using System;
using System.Web.Http;

namespace WhatToEat.ApiControllers
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