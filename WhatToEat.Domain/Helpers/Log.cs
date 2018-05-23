using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatToEat.Core;
using WhatToEat.Domain.Models;
using WhatToEat.Domain.Services;

namespace WhatToEat.Domain.Helpers
{
    /// <summary>
    /// Klasa statyczna / wrapper obsługująca logowanie
    /// </summary>
    public static class Log
    {
        private static readonly ILogger Logger;

        static Log()
        {
            Logger = new DbLogger(new AppDb());
        }

        /// <summary>
        /// Loguje informację 
        /// </summary>
        /// <param name="message">Treść informacji</param>
        public static void Info(string message)
        {
            Logger.Info(message);
        }

        /// <summary>
        /// Loguje błąd
        /// </summary>
        /// <param name="error">Treść błędu</param>
        public static void Error(string error)
        {
            Logger.Error(error);
        }

        /// <summary>
        /// Loguje błąd wraz z wyjątkiem
        /// </summary>
        /// <param name="error">Treść błędu</param>
        /// <param name="exception">Wyjątek</param>
        public static void Error(string error, Exception exception)
        {
            Logger.Error(error, exception);
        }
    }
}
