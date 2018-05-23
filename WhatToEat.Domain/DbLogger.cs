using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatToEat.Core;
using WhatToEat.Domain.Models;
using WhatToEat.Domain.Services;

namespace WhatToEat.Domain
{
    /// <summary>
    /// Klasa odpowiedzialna za logowanie do bazy danych
    /// </summary>
    public class DbLogger : ILogger
    {
        private readonly ILogService _logService;

        public DbLogger(IContext context)
        {
            _logService = new LogsService(context);
        }

        /// <summary>
        /// Loguje informację do bazy danych
        /// </summary>
        /// <param name="message">Informacja</param>
        public void Info(string message)
        {
            var log = new Log()
            {
                Message = message
            };
            _logService.Create(log);
        }

        /// <summary>
        /// Loguje błąd do bazy danych
        /// </summary>
        /// <param name="error">Błąd</param>
        public void Error(string error)
        {
            var log = new Log()
            {
                Message = error
            };
            _logService.Create(log);
        }

        /// <summary>
        /// Loguje błąd i wyjątek do bazy danych
        /// </summary>
        /// <param name="error">Błąd</param>
        /// <param name="exception">Wyjątek</param>
        public void Error(string error, Exception exception)
        {
            var log = new Log()
            {
                Message = error,
                Exception = exception.ToString()
            };
            _logService.Create(log);
        }
    }
}
