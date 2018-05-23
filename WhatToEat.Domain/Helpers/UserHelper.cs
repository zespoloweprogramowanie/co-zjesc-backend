using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WhatToEat.Domain.Services;

namespace WhatToEat.Domain.Helpers
{
    /// <summary>
    /// Klasa obsługująca metody związane z użytkownikiem
    /// </summary>
    public static class UserHelper
    {
        /// <summary>
        /// Pobiera Id aktualnie zalogowanego użytkownika
        /// </summary>
        /// <returns>Id zalogowanego użytkownika</returns>
        public static string GetCurrentUserId()
        {
            string userId = ClaimsPrincipal.Current.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            return userId;
        }

        /// <summary>
        /// Sprawdza czy aktualny użytkownik jest zalogowany
        /// </summary>
        /// <returns>Czy jest zalogowany</returns>
        public static bool IsUserLoggedIn() => !String.IsNullOrEmpty(GetCurrentUserId());
    }
}
