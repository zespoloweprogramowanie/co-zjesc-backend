using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using WhatToEat.Domain.Models;

namespace WhatToEat.Domain.Repositories
{
    public class AuthRepository : IDisposable
    {
        private AppDb _ctx;

        private UserManager<User> _userManager;

        public AuthRepository()
        {
            _ctx = new AppDb();
            _userManager = new UserManager<User>(new UserStore<User>(_ctx));
        }
        /// <summary>
        /// Rejestruje użytkownika
        /// </summary>
        /// <param name="userModel">Dane użytkownika</param>
        /// <returns>Obiekt identity</returns>
        public async Task<IdentityResult> RegisterUser(UserModel userModel)
        {
            User user = new User
            {
                UserName = userModel.UserName
            };

            var result = await _userManager.CreateAsync(user, userModel.Password);

            return result;
        }

        /// <summary>
        /// Szuka użytkownika w bazie danych
        /// </summary>
        /// <param name="userName">Login użytkownika</param>
        /// <param name="password">Hasło użytkownika</param>
        /// <returns>Model użytkownika</returns>
        public async Task<User> FindUser(string userName, string password)
        {
            User user = await _userManager.FindAsync(userName, password);

            return user;
        }

        public void Dispose()
        {
            _ctx.Dispose();
            _userManager.Dispose();

        }
    }
}