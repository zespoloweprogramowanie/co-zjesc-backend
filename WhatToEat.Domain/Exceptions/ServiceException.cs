using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatToEat.Core;
using WhatToEat.Domain.Models;

namespace WhatToEat.Domain.Exceptions
{
    /// <summary>
    /// Klasa trzymająca informacji o wyjątku zwracanym przez warstwę serwisową
    /// </summary>
    class ServiceException : Exception
    {
        private ILogger _logger;
        
        public ServiceException(string message) : base(message)
        {
            _logger = new DbLogger(new AppDb());
            _logger.Error(message);
        }
        public ServiceException(string message, Exception exception) : base(message, exception)
        {
            _logger = new DbLogger(new AppDb());
            _logger.Error(message, exception);
        }
    }
}
