using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace WhatToEat.Core.Helpers
{
    public static class ServerHelper
    {
        public static string GetAbsolutePath(string relativeUrl)
        {
            if (string.IsNullOrEmpty(relativeUrl))
                throw new NullReferenceException();
            return HostingEnvironment.MapPath(relativeUrl);
        }
    }
}
