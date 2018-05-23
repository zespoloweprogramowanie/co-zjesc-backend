using System;
using System.Web.Http;

namespace WhatToEat.ApiControllers
{
    public class StatusController : ApiController
    {
        /// <summary>
        /// Metoda dająca aktualną datę i czas.
        /// </summary>
        /// <returns>Zwraca datę i czas jako tekst.</returns>
        [HttpGet]
        public String Get()
        {
            return DateTime.Now.ToLongDateString();
        }
    }
}