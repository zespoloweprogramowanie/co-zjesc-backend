using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

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

        public static string GetContentUrl(string relativeUrl)
        {
            var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            return urlHelper.Content(relativeUrl);
        }
    }
}
