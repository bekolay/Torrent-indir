using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace torrentleoyunWeb
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Sitemap",
                url: "sitemap.xml",
                defaults: new { controller = "Sitemap", action = "Index" }

            );

            routes.MapRoute(
                name: "blog",
                url: "blog/{id}",
                defaults: new { controller = "blog", action = "Detay", id = UrlParameter.Optional }

            );

            routes.MapRoute(
                name: "K",
                url: "K/{id}",
                defaults: new { controller = "K", action = "Index", id = UrlParameter.Optional }

            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "torrentleoyunWeb.Controllers" }
            );
        }
    }
}
