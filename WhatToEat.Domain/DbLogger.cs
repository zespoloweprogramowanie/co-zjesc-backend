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
    public class DbLogger : ILogger
    {
        private readonly ILogService _logService;

        public DbLogger(IContext context)
        {
            _logService = new LogsService(context);
        }

        public void Info(string message)
        {
            var log = new Log()
            {
                Message = message
            };
            _logService.Create(log);
        }

        public void Error(string error)
        {
            var log = new Log()
            {
                Message = error
            };
            _logService.Create(log);
        }

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
