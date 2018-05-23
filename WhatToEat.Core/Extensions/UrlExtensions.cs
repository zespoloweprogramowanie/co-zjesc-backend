using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WhatToEat.Core.Extensions
{
    public static class UrlExtensions
    {
        /// <summary>
        /// Zwraca ścieżkę absolutną na podstawie HttpContextu i ścieżki relatwynej
        /// </summary>
        /// <param name="relativeUrl">Ścieżka relatywna</param>
        /// <returns>Ścieżka absolutna</returns>
        public static string ToAbsoluteUrl(this string relativeUrl)
        {
            if (string.IsNullOrEmpty(relativeUrl))
                return relativeUrl;

            if (HttpContext.Current == null)
                return relativeUrl;

            if (relativeUrl.StartsWith("/"))
                relativeUrl = relativeUrl.Insert(0, "~");
            if (!relativeUrl.StartsWith("~/"))
                relativeUrl = relativeUrl.Insert(0, "~/");

            var url = HttpContext.Current.Request.Url;
            var port = url.Port != 80 ? (":" + url.Port) : String.Empty;

            return $"{url.Scheme}://{url.Host}{port}{VirtualPathUtility.ToAbsolute(relativeUrl)}";
        }
    }
}
