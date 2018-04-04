using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security.OAuth;
using WhatToEat.Domain.Models;
using WhatToEat.Domain.Repositories;

namespace WhatToEat.Domain.Providers
{
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            var db = new AppDb();

            using (AuthRepository _repo = new AuthRepository())
            {
                User user = await _repo.FindUser(context.UserName, context.Password);

                if (user == null)
                {
                    context.SetError("invalid_grant", "The user name or password is incorrect.");
                    return;
                }
            }

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim("sub", context.UserName));

            var userRole = db.Users.Include(x => x.Roles)
                .FirstOrDefault(x => x.UserName == context.UserName)
                ?.Roles
                .FirstOrDefault();

            if (userRole != null)
            {
                var role = db.Roles.FirstOrDefault(x => x.Id == userRole.RoleId);
                if (role != null)
                {
                    identity.AddClaim(new Claim("role", role.Name));
                    identity.AddClaim(new Claim("id", userRole.UserId));
                }
            }

            context.Validated(identity);

            //context.Response.Cookies.Append("access_token", "dupa", new Microsoft.Owin.CookieOptions()
            //{
            //    Domain = "127.0.0.1",//context.Request.Headers["Origin"],
            //    Expires = DateTime.Now.AddDays(300),
            //    HttpOnly = false
            //});

        }
    }
}