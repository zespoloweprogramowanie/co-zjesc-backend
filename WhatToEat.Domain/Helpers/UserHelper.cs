using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WhatToEat.Domain.Services;

namespace WhatToEat.Domain.Helpers
{
    public static class UserHelper
    {
        public static string GetCurrentUserId()
        {
            string userId = ClaimsPrincipal.Current.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            return userId;
        }

        public static bool IsUserLoggedIn() => !String.IsNullOrEmpty(GetCurrentUserId());
    }
}
