﻿using System;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using WhatToEat.Models;
using WhatToEat.Repositories;

namespace WhatToEat.Controllers
{
    //[RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private readonly AuthRepository _repo = null;
        private AppDb _db;

        public AccountController()
        {
            _repo = new AuthRepository();
            _db = new AppDb();
        }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(UserModel userModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await _repo.RegisterUser(userModel);

            IHttpActionResult errorResult = GetErrorResult(result);

            if (errorResult != null)
            {
                return errorResult;
            }

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _repo.Dispose();
            }

            base.Dispose(disposing);
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }
       
        [Route("api/user")]
        [HttpGet]
        [Authorize]
        public IHttpActionResult GetName()
        {
            //var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            //var userName = ClaimsPrincipal.Current.Claims.First(c => c.Type == "sub").Value;
            //var user = new { username = userName };

            string user = ClaimsPrincipal.Current.Claims.First(c => c.Type == "sub").Value;
            string role = ClaimsPrincipal.Current.Claims.First(c => c.Type == "role").Value;

            return Json(new
            {
                login = user,
                role = role
            });
        }
    }
}
