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
    public static class Log
    {
        private static readonly ILogger Logger;

        static Log()
        {
            Logger = new DbLogger(new AppDb());
        }

        public static void Info(string message)
        {
            Logger.Info(message);
        }

        public static void Error(string error)
        {
            Logger.Error(error);
        }

        public static void Error(string error, Exception exception)
        {
            Logger.Error(error, exception);
        }
    }
}
